using System.Text.Json;
using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Persistence;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedGenerators.Implementations;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DbModels;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DtoMappings;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.Implementations
{
    internal class ProductFeedSettingsService : IProductFeedSettingsService
    {
        // Product feed settings are stored alongside the rest of the Umbraco Commerce data,
        // i.e. against the Umbraco Commerce connection (umbracoCommerceDbDSN when configured,
        // otherwise the default Umbraco connection). Going through the Commerce unit of work
        // and NPoco database provider guarantees we always target that same database rather
        // than the main Umbraco database. See GitHub issue #812.
        private readonly IUnitOfWorkProvider _uowProvider;
        private readonly INPocoDatabaseProvider _dbProvider;
        private readonly ILogger<ProductFeedSettingsService> _logger;
        private readonly FeedGeneratorCollection _feedGenerators;

        public ProductFeedSettingsService(
            IUnitOfWorkProvider uowProvider,
            INPocoDatabaseProvider dbProvider,
            ILogger<ProductFeedSettingsService> logger,
            FeedGeneratorCollection feedGenerators)
        {
            _uowProvider = uowProvider;
            _dbProvider = dbProvider;
            _logger = logger;
            _feedGenerators = feedGenerators;
        }

        /// <inheritdoc/>
        public async Task<ProductFeedSettingReadModel?> FindSettingAsync(FindSettingParams findSettingParams)
        {
            ArgumentNullException.ThrowIfNull(findSettingParams);

            UmbracoCommerceProductFeedSetting? feedSetting = await _uowProvider.ExecuteAsync(async uow =>
            {
                IDatabase db = await _dbProvider.GetDatabaseAsync().ConfigureAwait(false);
                UmbracoCommerceProductFeedSetting? result = await db
                    .SingleOrDefaultAsync<UmbracoCommerceProductFeedSetting>(
                    @"
select *
from umbracoCommerceProductFeedSetting
where( @0 IS NULL OR feedRelativePath = @0)
AND (@1 IS NULL OR id = @1)",
                    findSettingParams.FeedRelativePath, findSettingParams.Id)
                    .ConfigureAwait(false);
                uow.Complete();
                return result;
            }).ConfigureAwait(false);

            if (feedSetting == null)
            {
                return null;
            }

            if (!_feedGenerators.Any(p => p.Id == feedSetting.FeedGeneratorId))
            {
                throw new InvalidOperationException($"Unknown feed generator detected. Id: '{feedSetting.FeedGeneratorId}'.");
            }

            ProductFeedSettingReadModel readModel = ProductFeedSettingMapper.MapToReadModel(feedSetting);
            return readModel;
        }

        public async Task<List<ProductFeedSettingReadModel>> GetListAsync(Guid storeId)
        {
            List<UmbracoCommerceProductFeedSetting> settings = await _uowProvider.ExecuteAsync(async uow =>
            {
                IDatabase db = await _dbProvider.GetDatabaseAsync().ConfigureAwait(false);
                List<UmbracoCommerceProductFeedSetting> result = await db
                    .FetchAsync<UmbracoCommerceProductFeedSetting>(
                    @"
select *
from umbracoCommerceProductFeedSetting
where storeId = @0", storeId)
                    .ConfigureAwait(false);
                uow.Complete();
                return result;
            }).ConfigureAwait(false);

            if (settings == null)
            {
                return [];
            }

            return settings.Select(setting => ProductFeedSettingMapper.MapToReadModel(setting)).ToList();
        }

        /// <inheritdoc/>
        public async Task<Guid?> SaveSettingAsync(ProductFeedSettingWriteModel input)
        {
            try
            {
                return await _uowProvider.ExecuteAsync(async uow =>
                {
                    IDatabase db = await _dbProvider.GetDatabaseAsync().ConfigureAwait(false);

                    // Create the database model directly from input data
                    UmbracoCommerceProductFeedSetting dbModel = ProductFeedSettingMapper.MapToDbModel(input);

                    if (input.Id == null)
                    {
                        // Add mode - generate new ID and insert
                        dbModel.Id = Guid.NewGuid();
                        _ = await db.InsertAsync(dbModel).ConfigureAwait(false);
                        uow.Complete();
                        return (Guid?)dbModel.Id;
                    }
                    else
                    {
                        // Edit mode - update directly without retrieving existing entity
                        int affectedRowCount = await db.UpdateAsync(dbModel).ConfigureAwait(false);
                        if (affectedRowCount != 1)
                        {
                            uow.Complete();
                            return (Guid?)null;
                        }

                        uow.Complete();
                        return (Guid?)dbModel.Id;
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(ex, "Unable to save record. Data: {Data}", JsonSerializer.Serialize(input));
                }

                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteSettingAsync(Guid id)
        {
            try
            {
                return await _uowProvider.ExecuteAsync(async uow =>
                {
                    IDatabase db = await _dbProvider.GetDatabaseAsync().ConfigureAwait(false);
                    int affectedRowCount = await db.DeleteAsync(new UmbracoCommerceProductFeedSetting
                    {
                        Id = id,
                    }).ConfigureAwait(false);
                    uow.Complete();

                    return affectedRowCount == 1;
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(ex, "Unable to delete the record with id = {Id}", id);
                }

                return false;
            }
        }
    }
}
