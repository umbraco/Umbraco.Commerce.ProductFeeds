# Umbraco Commerce Product Feeds &middot; [![NuGet](https://img.shields.io/nuget/v/Umbraco.Commerce.ProductFeeds.svg?style=modern&label=nuget)](https://www.nuget.org/packages/Umbraco.Commerce.ProductFeeds/) 

Expand your product reach and show your products to a larger audience with the help of product feeds.

## Features
With the Umbraco.Commerce.ProductFeeds installed you will be able to configure product feeds, and expose them in an XML-format feed containing comprehensive product information.

`Umbraco.Commerce.ProductFeeds` supports the feed template for `Google Merchant Center`.

## Documentation
- [Getting started [↗]](https://docs.umbraco.com/umbraco-commerce-packages/product-feeds/installation).
- [Release Notes](https://github.com/umbraco/Umbraco.Commerce.ProductFeeds/releases)


## Migrate from v13 and v0.5.5 to v14
- Due to the change in schema of Product Document Type and Product Child Variant Types, you will need to manually edit your feed settings. Please go the the feed settings and find the [obsolete] properties and migrate them to the newer one
![image](https://github.com/user-attachments/assets/36d48973-11dc-49f2-b744-432152458419)


## Working locally
### Frontend
- Nodejs v20+.
- Go to `./src/Umbraco.Commerce.ProductFeeds.Client/frontend` folder and run `npm i` then `npm run build` to build frontend code.
