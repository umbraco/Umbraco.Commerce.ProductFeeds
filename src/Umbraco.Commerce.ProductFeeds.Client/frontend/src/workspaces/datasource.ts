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

        if (data?.data) {
            return { data: data.data as ProductFeedSettingReadModel };
        }

        return {
            error: data?.error ?? error,
        };
    }

    async fetchFeedTypesAsync() {
        const { data, error } = await tryExecuteAndNotify(this.#host, getFeedTypes());

        if (data?.data) {
            return { data: data.data as Array<LookupReadModel> };
        }

        return {
            error: data?.error ?? error,
        };
    }

    async fetchPropertyValueExtractorsAsync() {
        const { data, error } = await tryExecuteAndNotify(this.#host, getPropertyValueExtractors());

        if (data?.data) {
            return { data: data.data as Array<LookupReadModel> };
        }

        return {
            error: data?.error ?? error,
        };
    }

    async saveAsync(model: FeProductFeedSettingWriteModel) {
        const { data, error } = await save({
            body: model as ProductFeedSettingWriteModel,
        });

        console.log(data, error);

        if (data) {
            return { id: data };
        }

        return {
            error: error,
        };
    }

    async deleteAsync(id: string) {
        const { data, error } = await delete_({
            body: { id },
        });

        console.log(data, error);

        if (data) {
            return { isSuccess: data };
        }

        return {
            error: error,
        };
    }
}

