import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import type { UmbCollectionRepository } from '@umbraco-cms/backoffice/collection';
import { UmbRepositoryBase } from '@umbraco-cms/backoffice/repository';
import { UcProductFeedsDataSource } from '../datasource.js';
import { UC_STORE_CONTEXT, UcStoreModel } from '@umbraco-commerce/backoffice';

export class UcProductFeedCollectionRepository
    extends UmbRepositoryBase
    implements UmbCollectionRepository {

    #dataSource: UcProductFeedsDataSource;

    #store?: UcStoreModel;

    constructor(host: UmbControllerHost) {
        super(host);
        this.#dataSource = new UcProductFeedsDataSource(host);
        this.consumeContext(UC_STORE_CONTEXT, (ctx) => {
            this.observe(ctx.store, (store) => {
                console.log('repo store:', store);
                this.#store = store;
            });
        });
    }

    async requestCollection() {
        return this.#dataSource.getCollection(this.#store!.id);
    }
}

export { UcProductFeedCollectionRepository as api };
