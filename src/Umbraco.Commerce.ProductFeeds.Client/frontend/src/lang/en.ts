import type { UmbLocalizationDictionary } from '@umbraco-cms/backoffice/localization-api';

export default {
    ucProductFeeds: {
        sectionMenuLabel: 'Product Feeds',
        collectionLabel: 'Product Feeds',

        'prop:feedRelativePathLabel': 'Feed URL Segment',
        'prop:feedRelativePathDescription': 'Enter a url-friendly string. Value must be unique among the feeds.',

        'prop:feedDescriptionLabel': 'Feed Description',
        'prop:feedDescriptionDescription': 'A brief description about this feed. It will be used in <description>.',

        'prop:feedTypeLabel': 'Feed Type',
        'prop:feedTypeDescription': 'Choose the type that suits the consumer of this feed. Each type has a different template.',

        'prop:productDocumentTypeIdsLabel': 'Product Document Types',
        'prop:productDocumentTypeIdsDescription': 'The published content of this document type will be included in the feed',

        'prop:productChildVariantTypeIdsLabel': 'Product Child Variant Types',
        'prop:productChildVariantTypeIdsDescription': 'Use this property when you use child variants implementation for Umbraco Commerce',

        'prop:productRootIdLabel': 'Product Root',
        'prop:productRootIdDescription': 'Select the root for products. Only products under this root will be included in the feed.',

        'prop:propNodeMappingLabel': 'Product Property And Feed Node Mapping',
        'prop:propNodeMappingDescription': 'Map between product property alias and the feed node under <item>',

        'propNodeMapper:nodeName': 'Feed Node Name',
        'propNodeMapper:propertyAlias': 'Property Alias',
        'propNodeMapper:valueExtractorName': 'Property Value Extractor',

        // BEGIN legacy props
        'prop:productDocumentTypeAliasesLabel': '[OBSOLETE] Product Document Type',
        'prop:productDocumentTypeAliasesDescription': 'This property will be removed in the future. Migrate to "Product Document Type" property as soon as you can.',

        'prop:productChildVariantTypeAliasLabel': '[OBSOLETE] Product Child Variant Type',
        'prop:productChildVariantTypeAliasDescription': 'This property will be removed in the future. Migrate to "Product Child Variant Type" property as soon as you can.',
        // END legacy props

        'message:saveFailed': 'Save failed',
        'message:deleteFailed': 'Delete failed',

    },
} as UmbLocalizationDictionary;