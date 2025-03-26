import type { UmbLocalizationDictionary } from '@umbraco-cms/backoffice/localization-api';

export default {
    ucProductFeeds: {
        sectionMenuLabel: 'Product Feeds',
        collectionLabel: 'Product Feeds',

        propFeedRelativePathLabel: 'Feed URL Segment',
        propFeedRelativePathDescription: 'Enter a url-friendly string. Value must be unique among the feeds.',

        propFeedDescriptionLabel: 'Feed Description',
        propFeedDescriptionDescription: 'A brief description about this feed. It will be used in \\<description\\>.',

        propFeedTypeLabel: 'Feed Type',
        propFeedTypeDescription: 'Choose the type that suits the consumer of this feed. Each type has a different template.',

        propProductDocumentTypeIdsLabel: 'Product Document Types',
        propProductDocumentTypeIdsDescription: 'The published content of this document type will be included in the feed.',

        propProductChildVariantTypeIdsLabel: 'Product Child Variant Types',
        propProductChildVariantTypeIdsDescription: 'Use this property when you use child variants implementation for Umbraco Commerce.',

        propProductRootIdLabel: 'Product Root',
        propProductRootIdDescription: 'Select the root for products. Only products under this root will be included in the feed.',

        propIncludeTaxInPriceLabel: 'Include Tax In Price',
        propIncludeTaxInPriceDescription: 'For countries like US and Canada, you need to show price without tax, for most other countries, you should show price with tax.',

        propPropNodeMappingLabel: 'Product Property And Feed Node Mapping',
        propPropNodeMappingDescription: 'Map between product property alias and the feed node under \\<item\\>.',

        propNodeMapperNodeName: 'Feed Node Name',
        propNodeMapperPropertyAlias: 'Property Alias',
        propNodeMapperValueExtractorId: 'Property Value Extractor',

        messageSaveFailed: 'Save failed',
        messageSaveSuccess: 'Saved',
        messageDeleteSuccess: 'Deleted action succeeded.',
        messageDeleteFailed: 'Delete action failed',

        buttonOpenFeed: 'Open Feed',
    },
} as UmbLocalizationDictionary;