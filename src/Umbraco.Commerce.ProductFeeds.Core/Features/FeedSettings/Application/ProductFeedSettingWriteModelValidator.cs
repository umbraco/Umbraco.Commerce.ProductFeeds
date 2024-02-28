using FluentValidation;
using Umbraco.Commerce.ProductFeeds.Core.FeedSettings.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application
{
    internal class ProductFeedSettingWriteModelValidator : AbstractValidator<ProductFeedSettingWriteModel>
    {
        private const int MaximumStringLength = 255;

        public ProductFeedSettingWriteModelValidator()
        {
            RuleFor(x => x.FeedRelativePath).NotEmpty();
            RuleFor(x => x.FeedName).NotEmpty();
            RuleFor(x => x.FeedDescription).MaximumLength(MaximumStringLength);
            RuleFor(x => x.StoreId).NotEmpty();
            RuleFor(x => x.ProductRootId).NotEmpty();
            RuleFor(x => x.ProductDocumentTypeAlias).NotEmpty().MaximumLength(MaximumStringLength);
            RuleFor(x => x.ImagesPropertyAlias).NotEmpty().MaximumLength(MaximumStringLength);
            RuleFor(x => x.PropertyNameMappings).NotEmpty();

        }
    }
}
