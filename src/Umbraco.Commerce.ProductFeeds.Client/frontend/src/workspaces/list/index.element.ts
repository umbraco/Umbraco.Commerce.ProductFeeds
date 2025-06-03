import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import {
    customElement,
    html,
    css,
    state,
    LitElement,
} from '@umbraco-cms/backoffice/external/lit';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { UC_STORE_CONTEXT, UcStoreModel } from '@umbraco-commerce/backoffice';
import { UcpfListCollectionConfiguration } from '../../types.js';
import { storeRoute } from '../routes.js';
import { listingWorkspaceCollectionManifest } from './manifests.js';

const elementName = 'uc-product-feeds-workspace-collection';
@customElement(elementName)
export class UcpfListWorkspaceElement extends UmbElementMixin(LitElement) {
    @state()
    private _config?: UcpfListCollectionConfiguration;

    @state()
    _store?: UcStoreModel;

    constructor() {
        super();

        this.consumeContext(UC_STORE_CONTEXT, (ctx) => {
            this.observe(ctx?.store, (store) => {
                this._store = store;
                this.#constructConfig();
            });
        });
    }

    #constructConfig() {
        if (!this._store) return;

        this._config = {
            storeId: this._store.id,
            pageSize: 400, // Currently have to set a page size otherwise it doesn't load initial data
        };
    }

    render() {
        if (!this._store) return;

        return this._config
            ? html`<umb-body-layout main-no-padding headline=${this.localize.term('ucProductFeeds_collectionLabel')}>
                  <umb-workspace-entity-action-menu
                      slot="action-menu"
                  ></umb-workspace-entity-action-menu>
                  <umb-collection
                      alias=${listingWorkspaceCollectionManifest.alias}
                      .config=${this._config}
                  ></umb-collection>
                  <div slot="footer-info" id="footer">
                      <a href=${storeRoute(this._store.id)} >${this._store?.name}</a>
                      / ${this.localize.term('ucProductFeeds_collectionLabel')}
                  </div>
              </umb-body-layout>`
            : '';
    }

    static styles = [
        UmbTextStyles,
        css`
            :host {
                display: block;
                width: 100%;
                height: 100%;
            }

            #footer {
                padding: 0 var(--uui-size-layout-1);
            }
        `,
    ];
}

export default UcpfListWorkspaceElement;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: UcpfListWorkspaceElement;
    }
}
