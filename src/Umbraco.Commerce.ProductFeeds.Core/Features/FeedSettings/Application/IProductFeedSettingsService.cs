namespace Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application
{
    public interface IProductFeedSettingsService
    {
        Task<bool> DeleteSettingAsync(Guid id);
        Task<ProductFeedSettingReadModel?> FindSettingAsync(FindSettingParams findSettingParams);

        Task<List<ProductFeedSettingReadModel>> GetListAsync(Guid storeId);

        /// <summary>
        /// Save product feed settings.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>id of the record that this method has just saved. Returns null if no record is saved.</returns>
        Task<Guid?> SaveSettingAsync(ProductFeedSettingWriteModel input);
    }
}
