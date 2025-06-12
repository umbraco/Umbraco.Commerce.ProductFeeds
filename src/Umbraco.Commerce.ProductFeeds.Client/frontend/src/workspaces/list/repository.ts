import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import type { UmbCollectionRepository } from '@umbraco-cms/backoffice/collection';
import { UmbRepositoryBase } from '@umbraco-cms/backoffice/repository';
import { UcpfListDataSource } from '../datasource.js';
import { UC_STORE_CONTEXT, UcStoreModel } from '@umbraco-commerce/backoffice';

export class UcpfListCollectionRepository
    extends UmbRepositoryBase
    implements UmbCollectionRepository {

    #dataSource: UcpfListDataSource;

    #store?: UcStoreModel;

    constructor(host: UmbControllerHost) {
        super(host);
        this.#dataSource = new UcpfListDataSource(host);
        this.consumeContext(UC_STORE_CONTEXT, (ctx) => {
            this.observe(ctx?.store, (store) => {
                this.#store = store;
            });
        });
    }

    async requestCollection() {
        if (!this.#store) return {
            data: {
                items: [],
                total: 0,
            },
        };

        return this.#dataSource.fetchListAsync(this.#store.id);
    }

    async deleteAsync(ids: string[]) {
        return this.#dataSource.deleteAsync(ids);
    }
}
