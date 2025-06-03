using Microsoft.Extensions.Logging;
using NPoco;
using NPoco.DatabaseTypes;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.Migrations
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "<Pending>")]
    internal class AddTableCommerceProductFeedSetting : AsyncMigrationBase
    {
        public AddTableCommerceProductFeedSetting(IMigrationContext context) : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            const string tableName = "umbracoCommerceProductFeedSetting";
            Logger.LogDebug("Running migration {MigrationStep}", tableName);

            // Lots of methods available in the MigrationBase class - discover with this.
            if (TableExists(tableName))
            {
                Logger.LogDebug("The database table [{DbTable}] already exists, skipping migration.", tableName);
            }
            else
            {
                Create.Table<UmbracoCommerceProductFeedSettingSchema_1_0_0>().Do();
                if (Context.SqlContext.DatabaseType is not SQLiteDatabaseType)
                {
                    Create
                        .UniqueConstraint($"uc_{tableName}_feedRelativePath")
                        .OnTable(tableName)
                        .Column("feedRelativePath")
                        .Do();
                }
            }

            return Task.CompletedTask;
        }

        [TableName("umbracoCommerceProductFeedSetting")]
        [PrimaryKey("id", AutoIncrement = false)]
        public class UmbracoCommerceProductFeedSettingSchema_1_0_0
        {
            [Column("id")]
            public Guid Id { get; set; }

            [Column("feedType")]
            public required string FeedType { get; set; }

            [Column("feedRelativePath")]
            public required string FeedRelativePath { get; set; }

            [Column("feedName")]
            public required string FeedName { get; set; }

            [Column("feedDescription")]
            [NullSetting(NullSetting = NullSettings.Null)]
            public string? FeedDescription { get; set; }

            [Column("productDocumentTypeAlias")]
            public required string ProductDocumentTypeAlias { get; set; }

            [Column("productChildVariantTypeKey")]
            [NullSetting(NullSetting = NullSettings.Null)]
            public Guid? ProductChildVariantTypeKey { get; set; }

            [Column("productRootKey")]
            public required Guid ProductRootKey { get; set; }

            [Column("storeId")]
            public required Guid StoreId { get; set; }

            [Column("productPropertyNameMappings")]
            [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
            public required string ProductPropertyNameMappings { get; set; }
        }
    }
}
