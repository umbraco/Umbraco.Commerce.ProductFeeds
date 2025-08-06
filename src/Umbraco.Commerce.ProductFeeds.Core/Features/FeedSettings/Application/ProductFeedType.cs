using System.ComponentModel;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application
{
    [Obsolete("Will be removed in v17. Migrate to Feed Generator Id.")]
    public enum ProductFeedType
    {
        [Description("Google Merchant Center Feed")]
        GoogleMerchantCenter,
    }
}
