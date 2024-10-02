using Microsoft.Extensions.Logging;
using NPoco;
using NPoco.DatabaseTypes;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.Migrations
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "<Pending>")]
    internal class AddIncludeTaxInPrice_14_1_0 : MigrationBase
    {
        public AddIncludeTaxInPrice_14_1_0(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
        {
            const string tableName = "umbracoCommerceProductFeedSetting";

            // Lots of methods available in the MigrationBase class - discover with this.
            if (base.DatabaseType == DatabaseType.SQLite)
            {
                Logger.LogDebug("Running migration for SQLite db");
                if (TableExists(tableName))
                {
                    Logger.LogDebug("Drop existing table [{DbTable}].", tableName);
                    Delete.Table(tableName).Do();
                }

                Logger.LogDebug("Re-creating table [{DbTable}].", tableName);
                Create.Table<UmbracoCommerceProductFeedSettingSchema_14_1_0>().Do();
            }
            else if (Context.SqlContext.DatabaseType is not SQLiteDatabaseType)
            {
                Logger.LogDebug("Running migration for SQLServer db");

                Execute
                    .Sql("ALTER TABLE umbracoCommerceProductFeedSetting ADD includeTaxInPrice bit NOT NULL CONSTRAINT DF_includeTaxInPrice DEFAULT (1)")
                    .Do();
            }
        }

        [TableName("umbracoCommerceProductFeedSetting")]
        [PrimaryKey("id", AutoIncrement = false)]
        public class UmbracoCommerceProductFeedSettingSchema_14_1_0
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

            [Column("productDocumentTypeAliases")]
            [NullSetting(NullSetting = NullSettings.Null)]
            [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
            public string? ProductDocumentTypeAliases { get; set; }

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

            [Column("productRootKey")]
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
