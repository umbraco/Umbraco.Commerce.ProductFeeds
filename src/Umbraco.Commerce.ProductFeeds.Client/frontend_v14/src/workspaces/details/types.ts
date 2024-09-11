import { ProductFeedSettingWriteModel, PropertyAndNodeMapDetails } from '../../generated/apis';

export type FeProductFeedSettingWriteModel = Omit<ProductFeedSettingWriteModel, 'id' | 'propertyNameMappings'> & {
    id: string | undefined,
    propertyNameMappings: Array<FePropertyAndNodeMapDetails>
}

export type FePropertyAndNodeMapDetails = PropertyAndNodeMapDetails & {
    uiId: string
}