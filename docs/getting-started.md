# Getting started

## Basic usage
- Install the package from nuget: [![NuGet](https://img.shields.io/nuget/v/Umbraco.Commerce.ProductFeeds.svg?style=modern&label=nuget)](https://www.nuget.org/packages/Umbraco.Commerce.ProductFeeds/) 

- In your `Startup.cs` file, call `IServiceCollection.AddCommerceProductFeeds()` in `ConfigureServices` method

```cs
        public void ConfigureServices(IServiceCollection services)
        {
methods
            services.AddUmbraco(_env, _config)
                .AddBackOffice()
                .AddWebsite()
                .AddDemoStore()
                .AddComposers()
                .AddCommerceProductFeeds() // <====== this line
                .Build();
        }
```
- Go to your backoffice, open your store's setting page then click on `Product Feed` link.
![product feed list page](./assets/product-feed-list-page.png).

- From `Product Feeds` page, click on `Create Product Feed` button and fill in the feed settings. Mandatory fields are marked with a red asterisk (*).
![feed setting page](./assets/feed-setting-page.png)

- After saving the feed setting, you can find the feed link under `Feed Relative Path` field.
![open feed link](./assets/open-feed-link.png)

## Extending the plugin

### Add a custom property value extractor
When a simple node-name-to-property-alias mapping does not suit your need, you can create your own property value extractor to... *extract* the value from the property yourself.

Most of the time, you just need to create an implementation of `ISingleValuePropertyExtractor` then calls `PropertyExtractorNameToTypeMapping.TryAdd(string extractorName, Type type)` during application initialization to map your extractor name with the type of your implementation.

Afterwards, your extractor name should show up in the dropdown under `Property And Node Mapping` section.