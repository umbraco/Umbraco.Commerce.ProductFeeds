using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Commerce.Core;
using Umbraco.Commerce.Extensions;
using Umbraco.Commerce.ProductFeeds.Core.Common.Constants;
using Umbraco.Commerce.ProductFeeds.Core.Features.PropertyValueExtractors.Implementations;
using Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedGenerators.Implementations;
using Umbraco.Commerce.ProductFeeds.Core.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Application;
using Umbraco.Commerce.ProductFeeds.Core.ProductQueries.Implementations;
using Umbraco.Commerce.ProductFeeds.Extensions;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DtoMappings;
using Umbraco.Commerce.ProductFeeds.Infrastructure.Implementations;
using Umbraco.Commerce.ProductFeeds.Infrastructure.Migrations;
using Umbraco.Commerce.ProductFeeds.Startup.Initializations;
using Umbraco.Commerce.ProductFeeds.Startup.Swaggers;

namespace Umbraco.Commerce.ProductFeeds.Extensions
{
    public static class IUmbracoCommerceBuilderExtensions
    {
        /// <summary>
        /// Add new plugin to your umbraco commerce.
        /// </summary>
        /// <param name="ucBuilder"></param>
        /// <returns></returns>
        public static IUmbracoCommerceBuilder AddCommerceProductFeeds(this IUmbracoCommerceBuilder ucBuilder)
        {
            ArgumentNullException.ThrowIfNull(ucBuilder, "umbracoCommerceBuilder");
            ucBuilder.AddSwagger();
            ucBuilder.AddServices();
            ucBuilder.AddDbMigrations();
            ucBuilder.AddAutoMapper();
            return ucBuilder;
        }

        public static SingleValuePropertyExtractorCollectionBuilder SingleValuePropertyExtractors(this IUmbracoCommerceBuilder builder) => builder.WithUmbracoBuilder().WithCollectionBuilder<SingleValuePropertyExtractorCollectionBuilder>();

        public static MultipleValuePropertyExtractorCollectionBuilder MultipleValuePropertyExtractors(this IUmbracoCommerceBuilder builder) => builder.WithUmbracoBuilder().WithCollectionBuilder<MultipleValuePropertyExtractorCollectionBuilder>();

        private static IUmbracoCommerceBuilder AddDbMigrations(this IUmbracoCommerceBuilder builder)
        {
            builder.WithUmbracoBuilder().AddNotificationHandler<UmbracoApplicationStartingNotification, RunDbMigrations>();
            return builder;
        }

        private static IUmbracoCommerceBuilder AddAutoMapper(this IUmbracoCommerceBuilder builder)
        {
            builder.Services.AddAutoMapper(typeof(InfrastructureMappingProfile));
            return builder;
        }

        private static void AddServices(this IUmbracoCommerceBuilder builder)
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

            // Custom routing
            services.AddSingleton<CustomRouting>();
            services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter(General.PackageId)
                {
                    Endpoints = applicationBuilder =>
                    {
                        applicationBuilder.UseEndpoints(e =>
                        {
                            CustomRouting customRouting = applicationBuilder.ApplicationServices.GetRequiredService<CustomRouting>();
                            customRouting.CreateRoutes(e);
                        });
                    },
                });
            });
        }

        private static IUmbracoCommerceBuilder AddSwagger(this IUmbracoCommerceBuilder ucBuilder)
        {
            ucBuilder.WithUmbracoBuilder().Services.ConfigureOptions<ProductFeedsConfigureSwaggerGenOptions>();
            return ucBuilder;
        }
    }
}
