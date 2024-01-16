using Microsoft.Extensions.DependencyInjection;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Implementations
{
    public class SingleValuePropertyExtractorFactory : ISingleValuePropertyExtractorFactory
    {
        private readonly PropertyExtractorNameTypeMapping _extractorNameTypeMapping;
        private readonly IServiceProvider _serviceProvider;

        public SingleValuePropertyExtractorFactory(
            PropertyExtractorNameTypeMapping extractorNameTypeMapping,
            IServiceProvider serviceProvider)
        {
            _extractorNameTypeMapping = extractorNameTypeMapping;
            _serviceProvider = serviceProvider;
        }


        /// <inheritdoc/>
        public ISingleValuePropertyExtractor GetExtractor(string? uniqueExtractorName = null)
        {
            if (string.IsNullOrWhiteSpace(uniqueExtractorName))
            {
                return _serviceProvider.GetRequiredService<ISingleValuePropertyExtractor>();
            }

            Type extractorType = _extractorNameTypeMapping.GetExtractorType(uniqueExtractorName);
            return (ISingleValuePropertyExtractor)_serviceProvider.GetRequiredService(extractorType);
        }
    }
}
