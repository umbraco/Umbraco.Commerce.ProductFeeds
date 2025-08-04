using System.Text.Json;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DbModels;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.Implementations
{
    internal class ProductFeedSettingsService : IProductFeedSettingsService
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductFeedSettingsService> _logger;
        private readonly FeedGeneratorCollection _feedGenerators;

        public ProductFeedSettingsService(
            IScopeProvider scopeProvider,
            IMapper mapper,
            ILogger<ProductFeedSettingsService> logger,
            FeedGeneratorCollection feedGenerators)
        {
            _scopeProvider = scopeProvider;
            _mapper = mapper;
            _logger = logger;
            _feedGenerators = feedGenerators;
        }

        /// <inheritdoc/>
        public async Task<ProductFeedSettingReadModel?> FindSettingAsync(FindSettingParams findSettingParams)
        {
            ArgumentNullException.ThrowIfNull(findSettingParams);

            using IScope scope = _scopeProvider.CreateScope();
            UmbracoCommerceProductFeedSetting? feedSetting = await scope
                .Database
                .SingleOrDefaultAsync<UmbracoCommerceProductFeedSetting>(
                @"
select *
from umbracoCommerceProductFeedSetting
where( @0 IS NULL OR feedRelativePath = @0)
AND (@1 IS NULL OR id = @1)",
                findSettingParams.FeedRelativePath,
                findSettingParams.Id)
                .ConfigureAwait(false);
            scope.Complete();

            if (feedSetting == null)
            {
                return null;
            }

            if (!_feedGenerators.Any(p => p.Id == feedSetting.FeedType))
            {
                throw new InvalidOperationException($"Unknown feed type: '{feedSetting.FeedType}'.");
            }

            ProductFeedSettingReadModel readModel = _mapper.Map<ProductFeedSettingReadModel>(feedSetting);
            return readModel;
        }

        public async Task<List<ProductFeedSettingReadModel>> GetListAsync(Guid storeId)
        {
            using IScope scope = _scopeProvider.CreateScope();
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

        /// <inheritdoc/>
        public async Task<Guid?> SaveSettingAsync(ProductFeedSettingWriteModel input)
        {
            UmbracoCommerceProductFeedSetting dbModel = _mapper.Map<UmbracoCommerceProductFeedSetting>(input);

            try
            {
                using IScope scope = _scopeProvider.CreateScope();
                if (input.Id == null)
                {
                    // add mode
                    dbModel.Id = Guid.NewGuid();
                    _ = await scope.Database.InsertAsync(dbModel).ConfigureAwait(false);
                }
                else
                {
                    // edit mode
                    int affectedRowCount = await scope.Database.UpdateAsync(dbModel).ConfigureAwait(false);
                    if (affectedRowCount != 1)
                    {
                        return null;
                    }
                }

                scope.Complete();
                return dbModel.Id;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unable to add a new record. Data: {Data}", JsonSerializer.Serialize(input));
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteSettingAsync(Guid id)
        {
            try
            {
                using IScope scope = _scopeProvider.CreateScope();
                int affectedRowCount = await scope.Database.DeleteAsync(new UmbracoCommerceProductFeedSetting
                {
                    Id = id,
                }).ConfigureAwait(false);
                scope.Complete();

                return affectedRowCount == 1;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unable to delete the record with id = {Id}", id);
                return false;
            }
        }
    }
}
