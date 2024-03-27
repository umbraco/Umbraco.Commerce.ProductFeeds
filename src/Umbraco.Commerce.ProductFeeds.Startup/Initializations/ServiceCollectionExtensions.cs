using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Core;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;
using Umbraco.Commerce.ProductFeeds.Startup.Initializations;

namespace Umbraco.Commerce.ProductFeeds.Extensions
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
