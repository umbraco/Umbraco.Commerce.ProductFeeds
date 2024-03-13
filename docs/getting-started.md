# Getting started
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