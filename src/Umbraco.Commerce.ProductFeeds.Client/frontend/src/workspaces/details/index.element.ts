import {
    css,
    html,
    customElement,
    state,
    nothing,
} from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type {
    UUIInputElement,
    UUIInputEvent,
} from '@umbraco-cms/backoffice/external/uui';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { DETAILS_WORKSPACE_CONTEXT } from './context.js';
import { listRoute, storeRoute } from '../routes.js';
import { UC_STORE_CONTEXT, UcStoreModel } from '@umbraco-commerce/backoffice';
import { detailsWorkspaceManifest } from './manifests.js';
import { FeProductFeedSettingWriteModel } from './types.js';

const ELEMENT_NAME = 'uc-product-feed-workspace-editor';
@customElement(ELEMENT_NAME)
export class UcpfWorkspaceEditorElement extends UmbLitElement {
    #workspaceContext?: typeof DETAILS_WORKSPACE_CONTEXT.TYPE;

    @state()
    _store?: UcStoreModel;

    @state()
    _model?: FeProductFeedSettingWriteModel;

    @state()
    _isNew?: boolean;

    @state()
    _aliasLocked = true;

    constructor() {
        super();
        this.consumeContext(DETAILS_WORKSPACE_CONTEXT, (context) => {
            this.#workspaceContext = context;
            this.#observeWorkspace();
        });

        this.consumeContext(UC_STORE_CONTEXT, (ctx) => {
            this.observe(ctx.store, (store) => {
                this._store = store;
            });
        });
    }

    #observeWorkspace() {
        if (!this.#workspaceContext) return;
        this.observe(this.#workspaceContext.model, (model) => (this._model = model));
        this.observe(this.#workspaceContext.isNew, (isNew) => {
            this._isNew = isNew;
            if (isNew) {                    // TODO: Would be good with a more general way to bring focus to the name input.
                (
                    this.shadowRoot?.querySelector('#name') as HTMLElement
                )?.focus();
            }
        }, '_observeIsNew');
    }

    #onNameChange(event: UUIInputEvent) {
        event.stopPropagation();
        const target = event.composedPath()[0] as UUIInputElement;
        this.#workspaceContext?.setModel({
            ...this._model,
            feedName: target.value,
        } as FeProductFeedSettingWriteModel);
    }

    render() {
        if (!this._model) return;
        return html`
            <umb-workspace-editor alias=${detailsWorkspaceManifest.alias}>
                <div id="header" slot="header">
                    <uui-button
                        href=${listRoute(this._store!.id)}
                        label=${this.localize.term('general_backToOverview')}
                        compact>
                        <uui-icon name="icon-arrow-left"></uui-icon>
                    </uui-button>
                    <uui-input
                        id="name"
                        .value=${this._model?.feedName ?? ''}
                        @input="${this.#onNameChange}"
                        label=${this.localize.term('ucGeneral_name')}
                        placeholder=${this.localize.term('ucPlaceholders_enterName')}
                        required>
                    </uui-input>
				</div>
	            ${!this._isNew && this._model
                ? html`<umb-workspace-entity-action-menu slot="action-menu"></umb-workspace-entity-action-menu>`
                : nothing}
                <div slot="footer-info" id="footer">
                    <a href=${storeRoute(this._store!.id)}>${this._store?.name}</a> /
	                <a href=${listRoute(this._store!.id)}>
                    ${this.localize.term('ucProductFeeds_collectionLabel')}</a> /
	                ${this._model?.feedName ?? this.localize.term('ucGeneral_untitled')}
	            </div>
			</umb-workspace-editor>
		`;
    }

    static styles = [
        UmbTextStyles,
        css`
            :host {
                display: block;
                width: 100%;
                height: 100%;
            }

            #header {
                display: flex;
                flex: 1 1 auto;
            }

            #footer {
                padding: 0 var(--uui-size-layout-1);
            }

            #name {
                width: 100%;
                flex: 1 1 auto;
                align-items: center;
            }
        `,
    ];
}

declare global {
    interface HTMLElementTagNameMap {
        [ELEMENT_NAME]: UcpfWorkspaceEditorElement;
    }
}
