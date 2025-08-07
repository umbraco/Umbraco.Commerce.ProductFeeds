namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application
{
    public interface IProductFeedSettingsService
    {
        /// <summary>
        /// Delete product feed setting by the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteSettingAsync(Guid id);

        /// <summary>
        /// Find product feed setting by the given parameters.
        /// </summary>
        /// <param name="findSettingParams"></param>
        /// <returns></returns>
        Task<ProductFeedSettingReadModel?> FindSettingAsync(FindSettingParams findSettingParams);

        /// <summary>
        /// Get  list of product feed settings for the given store id.
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<List<ProductFeedSettingReadModel>> GetListAsync(Guid storeId);

        /// <summary>
        /// Save product feed settings.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>id of the record that this method has just saved. Returns null if no record is saved.</returns>
        Task<Guid?> SaveSettingAsync(ProductFeedSettingWriteModel input);
    }
}
