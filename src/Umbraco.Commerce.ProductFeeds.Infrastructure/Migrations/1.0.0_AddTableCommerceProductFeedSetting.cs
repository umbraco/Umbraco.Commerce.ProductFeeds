using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.Migrations
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "<Pending>")]
#pragma warning disable SA1649 // File name should match first type name
    internal class AddTableCommerceProductFeedSetting : MigrationBase
#pragma warning restore SA1649 // File name should match first type name
    {
        public AddTableCommerceProductFeedSetting(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            const string tableName = "umbracoCommerceProductFeedSetting";
            Logger.LogDebug("Running migration {MigrationStep}", tableName);

            // Lots of methods available in the MigrationBase class - discover with this.
            if (TableExists(tableName))
            {
                Logger.LogDebug("The database table [{DbTable}] already exists, skipping", tableName);
            }
            else
            {
                Create.Table<UmbracoCommerceProductFeedSettingSchema>().Do();
                Create.UniqueConstraint($"uc_{tableName}_feedRelativePath").OnTable(tableName).Column("feedRelativePath").Do();
            }
        }

        [TableName("umbracoCommerceProductFeedSetting")]
        [PrimaryKey("id", AutoIncrement = true)]
        public class UmbracoCommerceProductFeedSettingSchema
        {
            [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
            [Column("id")]
            public int Id { get; set; }

            [Column("feedType")]
            public required string FeedType { get; set; }

            [Column("feedRelativePath")]
            public required string FeedRelativePath { get; set; }

            public required string FeedName { get; set; }
            public required string FeedDescription { get; set; }
            public required string ProductDocumentTypeAlias { get; set; }
            public required string ImagesPropertyAlias { get; set; }
            public required int ProductRootId { get; set; }
            public required Guid StoreId { get; set; }

            [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
            public required string ProductPropertyNameMappings { get; set; }
        }
    }
}
