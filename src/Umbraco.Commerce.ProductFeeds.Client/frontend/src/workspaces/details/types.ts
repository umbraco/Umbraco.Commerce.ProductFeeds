import { ProductFeedSettingWriteModel, PropertyAndNodeMapItem } from '../../generated/apis';

export type FeProductFeedSettingWriteModel = Omit<ProductFeedSettingWriteModel, 'id' | 'propertyNameMappings' | 'productRootId' | 'feedType'> & {
    id?: string
    propertyNameMappings: Array<FePropertyAndNodeMapDetails>
    productRootId?: string | null,
    feedGeneratorId: string,
    feedType?: string,
}

export type FePropertyAndNodeMapDetails = PropertyAndNodeMapItem & {
    uiId: string
}
