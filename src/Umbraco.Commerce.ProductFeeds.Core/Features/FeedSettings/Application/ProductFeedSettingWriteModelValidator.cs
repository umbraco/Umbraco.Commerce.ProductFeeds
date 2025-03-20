using FluentValidation;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application
{
    internal class ProductFeedSettingWriteModelValidator : AbstractValidator<ProductFeedSettingWriteModel>
    {
        private const int MaximumStringLength = 255;

        public ProductFeedSettingWriteModelValidator()
        {
            RuleFor(x => x.FeedName).NotEmpty().WithName("Feed Name");
            RuleFor(x => x.FeedRelativePath).NotEmpty().WithName("Feed Relative Path");
            RuleFor(x => x.FeedDescription).MaximumLength(MaximumStringLength).WithName("Feed Description");
            RuleFor(x => x.FeedType).NotEmpty().WithName("Feed Type");
            RuleFor(x => x.StoreId).NotEmpty().WithName("Umbraco Commerce Store");
            RuleFor(x => x.ProductRootId).NotEmpty().WithName("Product Root");
            RuleFor(x => x.ProductDocumentTypeIds).NotEmpty().WithName("Product Document Types");
            RuleFor(x => x.PropertyNameMappings).NotEmpty().WithName("Product Property And Feed Node Mapping");
        }
    }
}
