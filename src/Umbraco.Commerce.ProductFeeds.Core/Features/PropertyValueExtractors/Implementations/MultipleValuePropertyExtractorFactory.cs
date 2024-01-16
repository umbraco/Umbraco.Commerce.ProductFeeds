using Microsoft.Extensions.DependencyInjection;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;

namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Implementations
{
    public class MultipleValuePropertyExtractorFactory : IMultipleValuePropertyExtractorFactory
    {
        private readonly PropertyExtractorNameTypeMapping _extractorNameTypeMapping;
        private readonly IServiceProvider _serviceProvider;

        public MultipleValuePropertyExtractorFactory(
            PropertyExtractorNameTypeMapping extractorNameTypeMapping,
            IServiceProvider serviceProvider)
        {
            _extractorNameTypeMapping = extractorNameTypeMapping;
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public IMultipleValuePropertyExtractor GetExtractor(string uniqueExtractorName)
        {
            if (string.IsNullOrWhiteSpace(uniqueExtractorName))
            {
                throw new ArgumentNullException(nameof(uniqueExtractorName));
            }

            Type extractorType = _extractorNameTypeMapping.GetExtractorType(uniqueExtractorName);
            return (IMultipleValuePropertyExtractor)_serviceProvider.GetRequiredService(extractorType);
        }
    }
}
