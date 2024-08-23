import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { getByStore, ProductFeedSettingReadModel } from '../generated/apis';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbPagedModel, UmbRepositoryResponse } from '@umbraco-cms/backoffice/repository';

export class UcProductFeedsDataSource {
    #host: UmbControllerHost;

    constructor(host: UmbControllerHost) {
        this.#host = host;
    }

    async getCollection(storeId: string): Promise<UmbRepositoryResponse<UmbPagedModel<ProductFeedSettingReadModel>>> {
        console.log('datasource getcollection');
        const { data, error } = await tryExecuteAndNotify(this.#host, getByStore({
            query: {
                storeId,
            },
        }));
        if (error) {
            return { error };
        }

        if (!data) {
            return {
                data: {
                    items: [],
                    total: 0,
                },
            };
        }

        return {
            data: {
                items: data.data as ProductFeedSettingReadModel[],
                total: 0,
            },
        };
    }
}

