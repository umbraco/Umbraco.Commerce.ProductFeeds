import { ProductFeedSettingReadModel } from '../../generated/apis';

export type ProductFeedsCollectionModel = (ProductFeedSettingReadModel) & {
    entityType: string;
    unique: string;
}
