using Microsoft.Extensions.Logging;
using NPoco;
using NPoco.DatabaseTypes;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.Migrations
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "<Pending>")]
    internal class UpdateSchemaForV14 : MigrationBase
    {
        public UpdateSchemaForV14(IMigrationContext context)
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

                // Check if productRootId column already exists
                var productRootIdExists = Database.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM pragma_table_info('umbracoCommerceProductFeedSetting') WHERE name = 'productRootId'") > 0;

                if (productRootIdExists)
                {
                    Logger.LogDebug("Table [{DbTable}] already has productRootId column, skipping migration.", tableName);
                    return;
                }

                if (TableExists(tableName))
                {
                    Logger.LogDebug("Drop existing table [{DbTable}].", tableName);
                    Delete.Table(tableName).Do();
                }

                Logger.LogDebug("Re-creating table [{DbTable}].", tableName);
                Create.Table<UmbracoCommerceProductFeedSettingSchema_1_0_5>().Do();
            }
            else if (Context.SqlContext.DatabaseType is not SQLiteDatabaseType)
            {
                Logger.LogDebug("Running migration for SQLServer db");

                // Add new columns only if they don't exist
                var addProductChildVariantTypeIdsQuery = @"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                   WHERE TABLE_NAME = 'umbracoCommerceProductFeedSetting' 
                                   AND COLUMN_NAME = 'productChildVariantTypeIds')
                    BEGIN
                        ALTER TABLE umbracoCommerceProductFeedSetting ADD productChildVariantTypeIds varchar(max) NULL
                    END";

                Execute
                    .Sql(addProductChildVariantTypeIdsQuery)
                    .Do();

                var addProductDocumentTypeIdsQuery = @"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                   WHERE TABLE_NAME = 'umbracoCommerceProductFeedSetting' 
                                   AND COLUMN_NAME = 'productDocumentTypeIds')
                    BEGIN
                        ALTER TABLE umbracoCommerceProductFeedSetting ADD productDocumentTypeIds varchar(max) NULL
                    END";

                Execute
                    .Sql(addProductDocumentTypeIdsQuery)
                    .Do();

                // Alter column only if it exists and is not already nullable varchar(max)
                var alterProductDocumentTypeAliasesQuery = @"
                    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'umbracoCommerceProductFeedSetting' 
                               AND COLUMN_NAME = 'productDocumentTypeAliases')
                    BEGIN
                        ALTER TABLE umbracoCommerceProductFeedSetting ALTER COLUMN productDocumentTypeAliases varchar(max) NULL
                    END";

                Execute
                   .Sql(alterProductDocumentTypeAliasesQuery)
                   .Do();

                // Rename column only if old column exists and new column doesn't exist
                var renameProductRootKeyQuery = @"
                    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'umbracoCommerceProductFeedSetting' 
                               AND COLUMN_NAME = 'productRootKey')
                    AND NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                    WHERE TABLE_NAME = 'umbracoCommerceProductFeedSetting' 
                                    AND COLUMN_NAME = 'productRootId')
                    BEGIN
                        EXEC sp_rename 'umbracoCommerceProductFeedSetting.productRootKey', 'productRootId', 'COLUMN'
                    END";

                Execute
                    .Sql(renameProductRootKeyQuery)
                    .Do();
            }
        }

        [TableName("umbracoCommerceProductFeedSetting")]
        [PrimaryKey("id", AutoIncrement = false)]
        public class UmbracoCommerceProductFeedSettingSchema_1_0_5
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
        }
    }
}
