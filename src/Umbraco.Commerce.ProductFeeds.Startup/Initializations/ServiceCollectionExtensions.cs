using Microsoft.Extensions.DependencyInjection;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Application;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations;

namespace Umbraco.Commerce.ProductFeeds.Startup.Initializations
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPropertyValueExtractors(this IServiceCollection services)
        {
            services.AddScoped<ISingleValuePropertyExtractorFactory, SingleValuePropertyExtractorFactory>();
            services.AddScoped<IMultipleValuePropertyExtractorFactory, MultipleValuePropertyExtractorFactory>();
        }
    }
}
