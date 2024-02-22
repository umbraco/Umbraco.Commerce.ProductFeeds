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
        public async Task<ProductFeedSettingReadModel?> FindSettingAsync(FindSettingParams findSettingParams)
        {
            ArgumentNullException.ThrowIfNull(findSettingParams);

            using (IScope scope = _scopeProvider.CreateScope())
            {
                UmbracoCommerceProductFeedSetting? feedSetting = await scope
                    .Database
                    .SingleOrDefaultAsync<UmbracoCommerceProductFeedSetting>(
                    @"
select *
from umbracoCommerceProductFeedSetting
where @0 IS NULL OR feedRelativePath = @0
AND @1 IS NULL OR id = @1",
                    findSettingParams.FeedRelativePath,
                    findSettingParams.Id)
                    .ConfigureAwait(false);
                scope.Complete();

                if (feedSetting == null)
                {
                    return null;
                }

                if (!Enum.TryParse(feedSetting.FeedType, true, out ProductFeedType feedType))
                {
                    throw new InvalidOperationException($"Unknown feed type: '{feedSetting.FeedType}'.");
                }

                return _mapper.Map<ProductFeedSettingReadModel>(feedSetting);
            }
        }

        public async Task<List<ProductFeedSettingReadModel>> GetListAsync(Guid storeId)
        {
            using (IScope scope = _scopeProvider.CreateScope())
            {
                List<UmbracoCommerceProductFeedSetting> settings = await scope
                    .Database
                    .FetchAsync<UmbracoCommerceProductFeedSetting>(
                    @"
select *
from umbracoCommerceProductFeedSetting
where storeId = @0", storeId)
                    .ConfigureAwait(false);
                scope.Complete();

                if (settings == null)
                {
                    return [];
                }

                return _mapper.Map<List<ProductFeedSettingReadModel>>(settings);
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
