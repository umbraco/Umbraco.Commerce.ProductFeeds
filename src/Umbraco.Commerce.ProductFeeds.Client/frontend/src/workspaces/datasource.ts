import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbPagedModel, UmbRepositoryResponse } from '@umbraco-cms/backoffice/repository';
import { FeProductFeedSettingWriteModel } from './details/types';
import { deleteV2, getByStoreV2, getDetailsV2, getFeedGeneratorsV2, getPropertyValueExtractorsV2, LookupReadModel, ProductFeedSettingReadModelReadable, ProductFeedSettingWriteModel, saveV2 } from '../generated/apis';

export class UcpfListDataSource {
    #host: UmbControllerHost;

    constructor(host: UmbControllerHost) {
        this.#host = host;
    }

    async fetchListAsync(storeId: string): Promise<UmbRepositoryResponse<UmbPagedModel<ProductFeedSettingReadModelReadable>>> {
        const { data, error } = await tryExecute(this.#host, getByStoreV2({
            query: {
                storeId,
            },
        }));
        if (error) {
            return { error };
        }

        return {
            data: {
                items: data ?? [],
                total: data?.length ?? 0,
            },
        };
    }

    async fetchFeedSettingDetailsAsync(id: string) {
        const { data, error } = await getDetailsV2({
            path: {
                id,
            },
        });

        if (data) {
            return { data: data as ProductFeedSettingReadModelReadable };
        }

        return {
            error,
        };
    }

    async fetchFeedTypesAsync() {
        const { data, error } = await getFeedGeneratorsV2();

        if (data) {
            return { data: data as Array<LookupReadModel> };
        }

        return {
            error,
        };
    }

    async fetchPropertyValueExtractorsAsync() {
        const { data, error } = await getPropertyValueExtractorsV2();

        if (data) {
            return { data: data as Array<LookupReadModel> };
        }

        return {
            error,
        };
    }

    async saveAsync(model: FeProductFeedSettingWriteModel) {
        const { data, error } = await saveV2({
            body: model as ProductFeedSettingWriteModel,
        });

        if (data) {
            return { id: data };
        }

        return {
            error,
        };
    }

    async deleteAsync(ids: string[]) {
        if (!ids || !ids.length) {
            throw 'no id to delete';
        }

        const { data, error } = await deleteV2({
            body: { ids },
        });

        if (data) {
            return { isSuccess: data };
        }

        return {
            error,
        };
    }
}

