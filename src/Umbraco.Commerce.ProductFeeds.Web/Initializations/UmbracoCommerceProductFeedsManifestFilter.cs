using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Commerce.ProductFeeds.Constants;

namespace Umbraco.Commerce.ProductFeeds.Web.Initializations
{
    public class UmbracoCommerceProductFeedsManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            ArgumentNullException.ThrowIfNull(manifests);

            PackageManifest manifest = new()
            {
                PackageId = "Umbraco.Commerce.ProductFeeds",
                PackageName = "Umbraco Commerce ProductFeeds",
                Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion?.Split('+')[0] ?? throw new InvalidOperationException("Unable to identity the assembly version."),
                BundleOptions = BundleOptions.None,
                AllowPackageTelemetry = true,
                Scripts = [
                    $"/App_Plugins/{RouteParams.AreaName}/angularjs/umbraco-commerce-product-feeds.js",
                ],
                Stylesheets = [
                    $"/App_Plugins/{RouteParams.AreaName}/angularjs/umbraco-commerce-product-feeds.css"
                ],
            };

            manifests.Add(manifest);
        }
    }
}
