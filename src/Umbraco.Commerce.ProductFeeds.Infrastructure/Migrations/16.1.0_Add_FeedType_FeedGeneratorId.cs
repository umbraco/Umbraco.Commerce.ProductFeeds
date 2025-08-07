using Microsoft.Extensions.Logging;
using NPoco;
using NPoco.DatabaseTypes;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.Migrations
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "<Pending>")]
    internal class Add_FeedType_FeedGeneratorId_16_1_0 : AsyncMigrationBase
    {
        public Add_FeedType_FeedGeneratorId_16_1_0(IMigrationContext context)
            : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            const string tableName = "umbracoCommerceProductFeedSetting";

            Logger.LogDebug("Altering table [{DbTable}].", tableName);
            if (base.DatabaseType == DatabaseType.SQLite)
            {
                Logger.LogDebug("Running migration for SQLite db");
                if (TableExists(tableName))
                {
                    Logger.LogDebug("Drop existing table [{DbTable}].", tableName);
                    Delete.Table(tableName).Do();
                }

                Logger.LogDebug("Re-creating table [{DbTable}].", tableName);
                Create.Table<UmbracoCommerceProductFeedSettingSchema_16_1_0>().Do();
            }
            else if (Context.SqlContext.DatabaseType is not SQLiteDatabaseType)
            {
                if (!ColumnExists(tableName, "feedGeneratorId"))
                {
                    Alter.Table(tableName)
                        .AddColumn("feedGeneratorId")
                        .AsGuid()
                        .Nullable()
                        .Do();

                    Update.Table(tableName)
                        .Set(new
                        {
                            feedGeneratorId = new Guid("101AE565-038F-443E-A29E-4FE0C7146C4A"), // Google Merchant Center Feed Service Id
                        })
                        .AllRows()
                        .Do();
                }

                if (ColumnExists(tableName, "feedType"))
                {
                    Alter.Table(tableName)
                        .AlterColumn("feedType") // TODO - v17: Drop column
                        .AsString()
                        .Nullable()
                        .Do();
                }
            }

            return Task.CompletedTask;
        }

        [TableName("umbracoCommerceProductFeedSetting")]
        [PrimaryKey("id", AutoIncrement = false)]
        public class UmbracoCommerceProductFeedSettingSchema_16_1_0
        {
            [Column("id")]
            public Guid Id { get; set; }

            [Column("feedType")]
            [NullSetting(NullSetting = NullSettings.Null)]
            public string? FeedType { get; set; } // TODO - v17: drop column

            [Column("feedGeneratorId")]
            public Guid FeedGeneratorId { get; set; }

            [Column("feedRelativePath")]
            public required string FeedRelativePath { get; set; }

            [Column("feedName")]
            public required string FeedName { get; set; }

            [Column("feedDescription")]
            [NullSetting(NullSetting = NullSettings.Null)]
            public string? FeedDescription { get; set; }

            [Column("productDocumentTypeIds")]
            [NullSetting(NullSetting = NullSettings.Null)]
            [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
            public string? ProductDocumentTypeIds { get; set; }

            [Column("productChildVariantTypeAlias")]
            [NullSetting(NullSetting = NullSettings.Null)]
            [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
            public string? ProductChildVariantTypeAlias { get; set; }

            [Column("productChildVariantTypeIds")]
            [NullSetting(NullSetting = NullSettings.Null)]
            [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
            public string? ProductChildVariantTypeIds { get; set; }

            [Column("productRootId")]
            public required Guid ProductRootId { get; set; }

            [Column("storeId")]
            public required Guid StoreId { get; set; }

            [Column("productPropertyNameMappings")]
            [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
            public required string ProductPropertyNameMappings { get; set; }

            public bool IncludeTaxInPrice { get; set; }
        }
    }
}
