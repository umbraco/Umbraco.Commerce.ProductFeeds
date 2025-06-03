import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbCollectionActionBase } from '@umbraco-cms/backoffice/collection';
import { UC_STORE_CONTEXT } from '@umbraco-commerce/backoffice';
import { createRoute } from '../../../routes';

export class CollectionCreateAction extends UmbCollectionActionBase {
    #storeId?: string = undefined;

    constructor(host: UmbControllerHost) {
        super(host);
        this.consumeContext(UC_STORE_CONTEXT, ctx => {
            this.observe(ctx?.store, (store) => {
                this.#storeId = store?.id;
            });
        });
    }

    async execute() {
        if (!this.#storeId) {
            throw 'storeId is undefined';
        }

        history.pushState(null, '', createRoute(this.#storeId!));
    }
}
