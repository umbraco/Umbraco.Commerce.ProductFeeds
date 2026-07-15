using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NPoco;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Data;
using Umbraco.Commerce.Persistence;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DbModels;
using CoreScoping = Umbraco.Cms.Core.Scoping;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.Migrations
{
    public class RunDbMigrations : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
    {
        private const string TableName = "umbracoCommerceProductFeedSetting";

        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly CoreScoping.ICoreScopeProvider _coreScopeProvider;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;
        private readonly IScopeProvider _scopeProvider;
        private readonly IUnitOfWorkProvider _uowProvider;
        private readonly INPocoDatabaseProvider _dbProvider;
        private readonly IOptionsMonitor<ConnectionStringConfig> _connectionStringConfig;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RunDbMigrations> _logger;

        public RunDbMigrations(
            IMigrationPlanExecutor migrationPlanExecutor,
            CoreScoping.ICoreScopeProvider coreScopeProvider,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState,
            IScopeProvider scopeProvider,
            IUnitOfWorkProvider uowProvider,
            INPocoDatabaseProvider dbProvider,
            IOptionsMonitor<ConnectionStringConfig> connectionStringConfig,
            IConfiguration configuration,
            ILogger<RunDbMigrations> logger)
        {
            _migrationPlanExecutor = migrationPlanExecutor;
            _coreScopeProvider = coreScopeProvider;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
            _scopeProvider = scopeProvider;
            _uowProvider = uowProvider;
            _dbProvider = dbProvider;
            _connectionStringConfig = connectionStringConfig;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Preserved for backwards compatibility with the v17.0.0 public constructor. The
        /// additional services introduced for GitHub issue #812 are resolved from the static
        /// service provider. Umbraco's DI selects the greediest resolvable constructor (the one
        /// above), so this overload is only used if the type is constructed manually.
        /// </summary>
        [Obsolete("Use the constructor that also accepts the persistence, configuration and logging services. Will be removed in v19.0.0")]
        public RunDbMigrations(
            IMigrationPlanExecutor migrationPlanExecutor,
            CoreScoping.ICoreScopeProvider coreScopeProvider,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState)
            : this(
                migrationPlanExecutor,
                coreScopeProvider,
                keyValueService,
                runtimeState,
                StaticServiceProvider.Instance.GetRequiredService<IScopeProvider>(),
                StaticServiceProvider.Instance.GetRequiredService<IUnitOfWorkProvider>(),
                StaticServiceProvider.Instance.GetRequiredService<INPocoDatabaseProvider>(),
                StaticServiceProvider.Instance.GetRequiredService<IOptionsMonitor<ConnectionStringConfig>>(),
                StaticServiceProvider.Instance.GetRequiredService<IConfiguration>(),
                StaticServiceProvider.Instance.GetRequiredService<ILogger<RunDbMigrations>>())
        {
        }

        public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }

            // Create a migration plan for a specific project/feature
            // We can then track that latest migration state/step for this project/feature
            var migrationPlan = new MigrationPlan("UmbracoCommerceProductFeeds Migration");

            // This is the steps we need to take
            // Each step in the migration adds a unique value
            migrationPlan
                .From(string.Empty)
                .To<AddTableCommerceProductFeedSetting>("1.0.0")
                .To<AlterTableUmbracoCommerceProductFeedSetting>("1.0.1")
                .To<AlterTableUmbracoCommerceProductFeedSetting>("1.0.2")
                .To<AlterTableUmbracoCommerceProductFeedSetting>("1.0.3")
                .To<AlterTableUmbracoCommerceProductFeedSetting>("1.0.4")
                .To<UpdateSchemaForV14>("1.0.5")
                .To<DummyMigrationStep_13_1_0>("13.1.0")
                .To<AddIncludeTaxInPrice_14_1_0>("14.1.0")
                .To<Add_FeedType_FeedGeneratorId_16_1_0>("16.1.0")

                // Run again because 1.0.5 was put at the wrong order;
                .To<UpdateSchemaForV14>("Migrate from aliases to ids");

            // Go and upgrade our site (Will check if it needs to do the work or not)
            // Based on the current/latest step.
            // NOTE: This uses the default Umbraco scope/connection, so the table is
            // created/updated in the main Umbraco database. When Umbraco Commerce shares that
            // database this is the correct location. When Commerce uses a separate database
            // (umbracoCommerceDbDSN) the settings need to live there instead - handled below.
            Upgrader upgrader = new(migrationPlan);
            await upgrader.ExecuteAsync(
                _migrationPlanExecutor,
                _coreScopeProvider,
                _keyValueService);

            // Ensure the settings table (and any existing data) lives in the Umbraco Commerce
            // database when a dedicated connection string is configured. See GitHub issue #812.
            await EnsureSettingsInCommerceDatabaseAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// When Umbraco Commerce is configured to use a separate database, make sure the product
        /// feed settings table exists there and move across any settings that were previously
        /// stored in the main Umbraco database. The original rows are left untouched as a safety
        /// net; the copy only happens while the Commerce table is empty so it is safe to re-run.
        /// </summary>
        private async Task EnsureSettingsInCommerceDatabaseAsync()
        {
            try
            {
                string? commerceConnectionString = _connectionStringConfig.CurrentValue?.ConnectionString;
                string? umbracoConnectionString = _configuration.GetConnectionString(Umbraco.Cms.Core.Constants.System.UmbracoConnectionName);

                // Nothing to do when Commerce shares the main Umbraco database - the migration
                // above already put the table in the right place.
                if (string.IsNullOrWhiteSpace(commerceConnectionString)
                    || ConnectionStringsMatch(commerceConnectionString, umbracoConnectionString))
                {
                    return;
                }

                await _uowProvider.ExecuteAsync(async uow =>
                {
                    IDatabase commerceDb = await _dbProvider.GetDatabaseAsync().ConfigureAwait(false);
                    bool commerceIsSqlite = IsSqlite(commerceDb);

                    // 1. Ensure the table exists in the Commerce database.
                    if (!await TableExistsAsync(commerceDb, commerceIsSqlite, TableName).ConfigureAwait(false))
                    {
                        await commerceDb.ExecuteAsync(commerceIsSqlite ? CreateTableSqlite : CreateTableSqlServer).ConfigureAwait(false);
                    }

                    // 2. Copy across any settings still held in the main Umbraco database, but
                    //    only while the Commerce table is empty so restarts don't duplicate rows.
                    long existingCount = await commerceDb.ExecuteScalarAsync<long>($"SELECT COUNT(*) FROM {TableName}").ConfigureAwait(false);
                    if (existingCount == 0)
                    {
                        List<UmbracoCommerceProductFeedSetting> legacySettings = await ReadLegacySettingsAsync().ConfigureAwait(false);
                        foreach (UmbracoCommerceProductFeedSetting legacySetting in legacySettings)
                        {
                            _ = await commerceDb.InsertAsync(legacySetting).ConfigureAwait(false);
                        }

                        if (legacySettings.Count > 0 && _logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation(
                                "Migrated {Count} product feed setting(s) from the main Umbraco database to the Umbraco Commerce database.",
                                legacySettings.Count);
                        }
                    }

                    uow.Complete();
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Don't take the site down over this - the original data is left intact in the
                // main Umbraco database and the copy will be retried on the next startup.
                _logger.LogError(ex, "Failed to migrate product feed settings to the Umbraco Commerce database. The settings remain in the main Umbraco database and migration will be retried on the next application start.");
            }
        }

        private async Task<List<UmbracoCommerceProductFeedSetting>> ReadLegacySettingsAsync()
        {
            using IScope scope = _scopeProvider.CreateScope();
            List<UmbracoCommerceProductFeedSetting> rows = [];
            if (await TableExistsAsync(scope.Database, IsSqlite(scope.Database), TableName).ConfigureAwait(false))
            {
                rows = await scope.Database
                    .FetchAsync<UmbracoCommerceProductFeedSetting>($"SELECT * FROM {TableName}")
                    .ConfigureAwait(false);
            }

            scope.Complete();
            return rows;
        }

        private static async Task<bool> TableExistsAsync(IDatabase db, bool isSqlite, string tableName)
        {
            string sql = isSqlite
                ? "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND lower(name) = lower(@0)"
                : "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @0";
            return await db.ExecuteScalarAsync<long>(sql, tableName).ConfigureAwait(false) > 0;
        }

        private static bool IsSqlite(IDatabase db)
            => db.DatabaseType.GetType().Name.Contains("SQLite", StringComparison.OrdinalIgnoreCase);

        private static bool ConnectionStringsMatch(string? a, string? b)
        {
            if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b))
            {
                return false;
            }

            // Normalise so trivial differences in whitespace/casing don't read as different
            // databases. When unsure we treat them as different, which is safe: the copy step is
            // idempotent and never deletes the source rows.
            static string Normalise(string value) => value.Replace(" ", string.Empty).Trim().ToLowerInvariant();
            return Normalise(a) == Normalise(b);
        }

        private const string CreateTableSqlServer = @"
CREATE TABLE [umbracoCommerceProductFeedSetting] (
    [id] uniqueidentifier NOT NULL,
    [feedGeneratorId] uniqueidentifier NOT NULL,
    [feedRelativePath] nvarchar(255) NOT NULL,
    [feedName] nvarchar(255) NOT NULL,
    [feedDescription] nvarchar(max) NULL,
    [productDocumentTypeIds] nvarchar(max) NULL,
    [productChildVariantTypeIds] nvarchar(max) NULL,
    [productRootId] uniqueidentifier NOT NULL,
    [storeId] uniqueidentifier NOT NULL,
    [productPropertyNameMappings] nvarchar(max) NOT NULL,
    [includeTaxInPrice] bit NOT NULL CONSTRAINT [DF_umbracoCommerceProductFeedSetting_includeTaxInPrice] DEFAULT (1),
    CONSTRAINT [PK_umbracoCommerceProductFeedSetting] PRIMARY KEY ([id]),
    CONSTRAINT [uc_umbracoCommerceProductFeedSetting_feedRelativePath] UNIQUE ([feedRelativePath])
);";

        private const string CreateTableSqlite = @"
CREATE TABLE ""umbracoCommerceProductFeedSetting"" (
    ""id"" TEXT NOT NULL PRIMARY KEY,
    ""feedGeneratorId"" TEXT NOT NULL,
    ""feedRelativePath"" TEXT NOT NULL,
    ""feedName"" TEXT NOT NULL,
    ""feedDescription"" TEXT NULL,
    ""productDocumentTypeIds"" TEXT NULL,
    ""productChildVariantTypeIds"" TEXT NULL,
    ""productRootId"" TEXT NOT NULL,
    ""storeId"" TEXT NOT NULL,
    ""productPropertyNameMappings"" TEXT NOT NULL,
    ""includeTaxInPrice"" INTEGER NOT NULL DEFAULT 1
);";
    }
}
