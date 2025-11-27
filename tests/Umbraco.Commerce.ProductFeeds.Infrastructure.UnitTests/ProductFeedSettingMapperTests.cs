using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DbModels;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DtoMappings;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.UnitTests
{
#pragma warning disable CA1515 // Consider making public types internal
    public class ProductFeedSettingMapperTests
#pragma warning restore CA1515 // Consider making public types internal
    {
        [Fact]
        public void ProductFeedSettingMapper_Should_Map_WriteModel_To_DbModel_Correctly()
        {
            // Arrange
            var writeModel = new ProductFeedSettingWriteModel
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                FeedRelativePath = "/test-feed.xml",
                FeedGeneratorId = new Guid("22222222-2222-2222-2222-222222222222").ToString(),
                FeedName = "Test Feed",
                FeedDescription = "Test Description",
                StoreId = new Guid("33333333-3333-3333-3333-333333333333"),
                ProductRootId = new Guid("44444444-4444-4444-4444-444444444444"),
                PropertyNameMappings =
                [
                    new () { PropertyAlias = "name", NodeName = "title", ValueExtractorId = null }
                ],
                ProductChildVariantTypeIds = [new Guid("55555555-5555-5555-5555-555555555555"), new Guid("66666666-6666-6666-6666-666666666666")],
                ProductDocumentTypeIds = [new Guid("77777777-7777-7777-7777-777777777777"), new Guid("71717171-7171-7171-7171-717171717171")],
                IncludeTaxInPrice = true
            };

            // Act
            UmbracoCommerceProductFeedSetting dbModel = ProductFeedSettingMapper.MapToDbModel(writeModel);

            // Assert
            Assert.Equal(writeModel.Id, dbModel.Id);
            Assert.Equal(writeModel.FeedRelativePath, dbModel.FeedRelativePath);
            Assert.Equal(writeModel.FeedName, dbModel.FeedName);
            Assert.Equal(writeModel.FeedDescription, dbModel.FeedDescription);
            Assert.Equal(writeModel.StoreId, dbModel.StoreId);
            Assert.Equal(writeModel.ProductRootId, dbModel.ProductRootId);
            Assert.Equal(writeModel.IncludeTaxInPrice, dbModel.IncludeTaxInPrice);
            Assert.Contains("name", dbModel.ProductPropertyNameMappings, StringComparison.Ordinal);
            Assert.Contains("title", dbModel.ProductPropertyNameMappings, StringComparison.Ordinal);
            Assert.Contains(";", dbModel.ProductChildVariantTypeIds, StringComparison.Ordinal);
            Assert.Contains(";", dbModel.ProductDocumentTypeIds, StringComparison.Ordinal);
        }

        [Fact]
        public void ProductFeedSettingMapper_Should_Map_DbModel_To_ReadModel_Correctly()
        {
            // Arrange
            var testId = new Guid("11111111-1111-1111-1111-111111111111");
            var testFeedGeneratorId = new Guid("22222222-2222-2222-2222-222222222222");
            var testStoreId = new Guid("33333333-3333-3333-3333-333333333333");
            var testProductRootId = new Guid("44444444-4444-4444-4444-444444444444");
            var testVariantTypeId1 = new Guid("55555555-5555-5555-5555-555555555555");
            var testVariantTypeId2 = new Guid("66666666-6666-6666-6666-666666666666");
            var testDocumentTypeId1 = new Guid("77777777-7777-7777-7777-777777777777");
            var testDocumentTypeId2 = new Guid("71717171-7171-7171-7171-717171717171");

            var dbModel = new UmbracoCommerceProductFeedSetting
            {
                Id = testId,
                FeedRelativePath = "/test-feed.xml",
                FeedGeneratorId = testFeedGeneratorId,
                FeedName = "Test Feed",
                FeedDescription = "Test Description",
                StoreId = testStoreId,
                ProductRootId = testProductRootId,
                ProductPropertyNameMappings = """[{"PropertyAlias":"name","NodeName":"title","ValueExtractorId":null}]""",
                ProductChildVariantTypeIds = $"{testVariantTypeId1};{testVariantTypeId2}",
                ProductDocumentTypeIds = $"{testDocumentTypeId1};{testDocumentTypeId2}",
                IncludeTaxInPrice = true,
            };

            // Act
            ProductFeedSettingReadModel readModel = ProductFeedSettingMapper.MapToReadModel(dbModel);

            // Assert
            Assert.Equal(dbModel.Id, readModel.Id);
            Assert.Equal(dbModel.FeedRelativePath, readModel.FeedRelativePath);
            Assert.Equal(dbModel.FeedName, readModel.FeedName);
            Assert.Equal(dbModel.FeedDescription, readModel.FeedDescription);
            Assert.Equal(dbModel.StoreId, readModel.StoreId);
            Assert.Equal(dbModel.ProductRootId, readModel.ProductRootId);
            Assert.Equal(dbModel.IncludeTaxInPrice, readModel.IncludeTaxInPrice);
            Assert.NotEmpty(readModel.PropertyNameMappings);
            Assert.Equal("name", readModel.PropertyNameMappings.First().PropertyAlias);
            Assert.Equal("title", readModel.PropertyNameMappings.First().NodeName);
            Assert.Equal(2, readModel.ProductChildVariantTypeIds.Count());
            Assert.Equal(2, readModel.ProductDocumentTypeIds.Count());
        }

        [Fact]
        public void ProductFeedSettingMapper_Should_Update_Existing_DbModel_Correctly()
        {
            // Arrange
            var updateModel = new ProductFeedSettingWriteModel
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                FeedRelativePath = "/updated-feed.xml",
                FeedGeneratorId = new Guid("99999999-9999-9999-9999-999999999999").ToString(),
                FeedName = "Updated Feed",
                FeedDescription = "Updated Description",
                StoreId = new Guid("33333333-3333-3333-3333-333333333333"),
                ProductRootId = new Guid("88888888-8888-8888-8888-888888888888"),
                PropertyNameMappings =
                [
                    new () { PropertyAlias = "updatedName", NodeName = "updatedTitle", ValueExtractorId = "extractor1" }
                ],
                ProductChildVariantTypeIds = [new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")],
                ProductDocumentTypeIds = [new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")],
                IncludeTaxInPrice = true
            };

            // Act - Use the simplified mapper that creates a complete entity from input
            UmbracoCommerceProductFeedSetting dbModel = ProductFeedSettingMapper.MapToDbModel(updateModel);

            // Assert
            Assert.Equal(updateModel.Id, dbModel.Id);
            Assert.Equal(updateModel.FeedRelativePath, dbModel.FeedRelativePath);
            Assert.Equal(updateModel.FeedName, dbModel.FeedName);
            Assert.Equal(updateModel.FeedDescription, dbModel.FeedDescription);
            Assert.Equal(updateModel.ProductRootId, dbModel.ProductRootId);
            Assert.Equal(updateModel.IncludeTaxInPrice, dbModel.IncludeTaxInPrice);
            Assert.Contains("updatedName", dbModel.ProductPropertyNameMappings, StringComparison.Ordinal);
            Assert.Contains("updatedTitle", dbModel.ProductPropertyNameMappings, StringComparison.Ordinal);
            Assert.Contains("extractor1", dbModel.ProductPropertyNameMappings, StringComparison.Ordinal);
        }
    }
}
