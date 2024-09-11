import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { getByStore, getDetails, getFeedTypes, LookupReadModel, ProductFeedSettingReadModel } from '../generated/apis';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbPagedModel, UmbRepositoryResponse } from '@umbraco-cms/backoffice/repository';

export class UcpfListDataSource {
    #host: UmbControllerHost;

    constructor(host: UmbControllerHost) {
        this.#host = host;
    }

    async fetchListAsync(storeId: string): Promise<UmbRepositoryResponse<UmbPagedModel<ProductFeedSettingReadModel>>> {
        console.log('datasource getcollection');
        const { data, error } = await tryExecuteAndNotify(this.#host, getByStore({
            query: {
                storeId,
            },
        }));
        if (error) {
            return { error };
        }

        if (!data || !data.data) {
            return {
                data: {
                    items: [],
                    total: 0,
                },
            };
        }

        return {
            data: {
                items: data.data,
                total: 0,
            },
        };
    }

    async fetchFeedSettingDetailsAsync(id: string) {
        const { data, error } = await tryExecuteAndNotify(this.#host, getDetails({
            path: {
                id,
            },
        }));

        if (!(data?.data)) {
            return {
                error,
            };
        }

        return { data: data.data as ProductFeedSettingReadModel };
    }

    async fetchFeedTypesAsync() {
        const { data, error } = await tryExecuteAndNotify(this.#host, getFeedTypes());

        if (!(data?.data)) {
            return {
                error,
            };
        }

        return { data: data.data as Array<LookupReadModel> };
    }
}

