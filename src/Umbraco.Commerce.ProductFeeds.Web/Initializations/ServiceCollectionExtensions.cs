using System;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Implementations;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations;
using Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Implementations;
using Umbraco.Commerce.ProductFeeds.Core.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Application;
using Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Implementations;
using Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Application;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DtoMappings;
using Umbraco.Commerce.ProductFeeds.Infrastructure.Migrations;

namespace Umbraco.Commerce.ProductFeeds.Web.Initializations
{
    public static class ServiceCollectionExtensions
    {
        public static IUmbracoBuilder AddCommerceProductFeeds(this IUmbracoBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));

            builder.AddNotificationHandler<TreeNodesRenderingNotification, TreeNodesRenderingNotificationHandler>();
            builder.ManifestFilters().Append<UmbracoCommerceProductFeedsManifestFilter>();

            builder.AddServices();
            builder.AddDbMigrations();
            builder.AddAutoMapper();
            return builder;
        }

#pragma warning disable CA1062 // Validate arguments of public methods
        public static SingleValuePropertyExtractorCollectionBuilder SingleValuePropertyExtractors(this IUmbracoBuilder builder) => builder.WithCollectionBuilder<SingleValuePropertyExtractorCollectionBuilder>();
        public static MultipleValuePropertyExtractorCollectionBuilder MultipleValuePropertyExtractors(this IUmbracoBuilder builder) => builder.WithCollectionBuilder<MultipleValuePropertyExtractorCollectionBuilder>();
#pragma warning restore CA1062 // Validate arguments of public methods

        private static void AddServices(this IUmbracoBuilder builder)
        {
            IServiceCollection services = builder.Services;
            services.AddScoped<IProductFeedGeneratorFactory, ProductFeedGeneratorFactory>();
            services.AddScoped<GoogleMerchantCenterFeedService>();

            services.AddScoped<IProductFeedSettingsService, ProductFeedSettingsService>();
            services.AddScoped<IProductQueryService, ProductQueryService>();

            builder.SingleValuePropertyExtractors()
                .Append<DefaultSingleValuePropertyExtractor>()
                .Append<DefaultGoogleAvailabilityValueExtractor>()
                .Append<DefaultMediaPickerPropertyValueExtractor>();

            builder.MultipleValuePropertyExtractors()
                .Append<DefaultMultipleMediaPickerPropertyValueExtractor>();

            services.AddPropertyValueExtractors();
        }

        private static void AddPropertyValueExtractors(this IServiceCollection services)
        {
            services.AddScoped<ISingleValuePropertyExtractorFactory, SingleValuePropertyExtractorFactory>();
            services.AddScoped<IMultipleValuePropertyExtractorFactory, MultipleValuePropertyExtractorFactory>();
        }

        private static IUmbracoBuilder AddDbMigrations(this IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, RunDbMigrations>();
            return builder;
        }

        private static IUmbracoBuilder AddAutoMapper(this IUmbracoBuilder builder)
        {
            builder.Services.AddAutoMapper(typeof(InfrastructureMappingProfile));
            return builder;
        }
    }
}
