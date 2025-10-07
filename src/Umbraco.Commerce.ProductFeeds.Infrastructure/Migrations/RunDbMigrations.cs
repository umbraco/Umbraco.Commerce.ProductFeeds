using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.Migrations
{
    public class RunDbMigrations : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public RunDbMigrations(
            IMigrationPlanExecutor migrationPlanExecutor,
            ICoreScopeProvider coreScopeProvider,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState)
        {
            _migrationPlanExecutor = migrationPlanExecutor;
            _coreScopeProvider = coreScopeProvider;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
        }

        public void Handle(UmbracoApplicationStartingNotification notification)
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

                // Run again because 1.0.5 was put at the wrong order;
                .To<UpdateSchemaForV14>("Migrate from aliases to ids");

            // Go and upgrade our site (Will check if it needs to do the work or not)
            // Based on the current/latest step
            Upgrader upgrader = new(migrationPlan);
            upgrader.Execute(
                _migrationPlanExecutor,
                _coreScopeProvider,
                _keyValueService);
        }
    }
}
