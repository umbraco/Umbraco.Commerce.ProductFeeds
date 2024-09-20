import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { delete_, getByStore, getDetails, getFeedTypes, getPropertyValueExtractors, LookupReadModel, ProductFeedSettingReadModel, ProductFeedSettingWriteModel, save } from '../generated/apis';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbPagedModel, UmbRepositoryResponse } from '@umbraco-cms/backoffice/repository';
import { FeProductFeedSettingWriteModel } from './details/types';

export class UcpfListDataSource {
    #host: UmbControllerHost;

    constructor(host: UmbControllerHost) {
        this.#host = host;
    }

    async fetchListAsync(storeId: string): Promise<UmbRepositoryResponse<UmbPagedModel<ProductFeedSettingReadModel>>> {
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
                total: data.data.length,
            },
        };
    }

    async fetchFeedSettingDetailsAsync(id: string) {
        const { data, error } = await getDetails({
            path: {
                id,
            },
        });

        if (data) {
            return { data: data as ProductFeedSettingReadModel };
        }

        return {
            error,
        };
    }

    async fetchFeedTypesAsync() {
        const { data, error } = await getFeedTypes();

        if (data) {
            return { data: data as Array<LookupReadModel> };
        }

        return {
            error,
        };
    }

    async fetchPropertyValueExtractorsAsync() {
        const { data, error } = await getPropertyValueExtractors();

        if (data) {
            return { data: data as Array<LookupReadModel> };
        }

        return {
            error,
        };
    }

    async saveAsync(model: FeProductFeedSettingWriteModel) {
        const { data, error } = await save({
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

        const { data, error } = await delete_({
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

