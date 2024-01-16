using AutoMapper;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DbModels;

namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Implementations
{
    public class ProductFeedSettingsService : IProductFeedSettingsService
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMapper _mapper;

        public ProductFeedSettingsService(
            IScopeProvider scopeProvider,
            IMapper mapper)
        {
            _scopeProvider = scopeProvider;
            _mapper = mapper;
        }


        /// <inheritdoc/>
        public async Task<ProductFeedSettingReadModel?> FindSettingAsync(string requestRelativePath)
        {
            using (IScope scope = _scopeProvider.CreateScope())
            {
                UmbracoCommerceProductFeedSetting? feedSetting = await scope
                    .Database
                    .SingleOrDefaultAsync<UmbracoCommerceProductFeedSetting>(
                    @"
select *
from umbracoCommerceProductFeedSetting
where feedRelativePath = @0", requestRelativePath)
                    .ConfigureAwait(false);
                scope.Complete();

                if (feedSetting == null)
                {
                    return null;
                }

                if (!Enum.TryParse(feedSetting.FeedType, true, out ProductFeedType feedType))
                {
                    throw new InvalidCastException($"Unable to cast feed type: '{feedSetting.FeedType}'.");
                }

                return _mapper.Map<ProductFeedSettingReadModel>(feedSetting);
            }
        }

        /// <inheritdoc/>
        public async Task<int?> SaveSettingAsync(ProductFeedSettingAddModel input)
        {
            UmbracoCommerceProductFeedSetting dbModel = _mapper.Map<UmbracoCommerceProductFeedSetting>(input);

            try
            {
                using (IScope scope = _scopeProvider.CreateScope())
                {
                    await scope.Database.SaveAsync(dbModel).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return dbModel.Id;
        }
    }
}
