import type { UmbWorkspaceViewElement } from '@umbraco-cms/backoffice/extension-registry';
import type { UUIEvent, UUIInputElement, UUISelectElement } from '@umbraco-cms/backoffice/external/uui';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import {
    customElement,
    html,
    css,
    state,
} from '@umbraco-cms/backoffice/external/lit';

import { DETAILS_WORKSPACE_CONTEXT } from '../context.js';
import { UC_STORE_CONTEXT, UcStoreModel } from '@umbraco-commerce/backoffice';
import { FeProductFeedSettingWriteModel, FePropertyAndNodeMapDetails } from '../types.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbInputDocumentTypeElement } from '@umbraco-cms/backoffice/document-type';
import { UcpfPropNodeMapItemChangeEventData, UcpfPropNodeMapItemRemoveEventData } from '../components/ucpf-property-node-mapper.js';

const ELEMENT_NAME = 'uc-product-feed-details-workspace-view';
@customElement(ELEMENT_NAME)
export class UcpfDetailsWorkspaceViewElement
    extends UmbLitElement
    implements UmbWorkspaceViewElement {
    #workspaceContext?: typeof DETAILS_WORKSPACE_CONTEXT.TYPE;

    @state()
    private _store?: UcStoreModel;

    @state()
    private _id?: string | null;

    @state()
    private _model?: FeProductFeedSettingWriteModel;

    @state()
    private _feedTypes: Option[] = [];

    constructor() {
        super();
        console.log('details element');
        this.consumeContext(DETAILS_WORKSPACE_CONTEXT, (ctx) => {
            this.#workspaceContext = ctx;
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

        this.observe(this.#workspaceContext.unique, (unique) => (this._id = unique));

        this.observe(this.#workspaceContext.model, (model) => {
            this._model = model;
            this._feedTypes = this.#markSelectedOption(this._feedTypes, model!.feedType);
        });

        this.observe(this.#workspaceContext.feedTypes, (feedTypes) => {
            this._feedTypes = this.#markSelectedOption(feedTypes, this._model?.feedType ?? '');
        });
    }

    #markSelectedOption(options: Option[], value: string) {
        return options.map(x => {
            return {
                ...x,
                selected: x.value === value,
            };
        });
    }

    #onInputChange(evt: UUIEvent) {
        const inputEl = evt.target as UUIInputElement;
        if (inputEl) {
            this.#workspaceContext?.setModel({
                ...this._model!,
                [inputEl.name]: inputEl.value,
            });
            console.log(this._model);
        }
    }

    #onSelectElementChange(evt: UUIEvent) {
        const selectEl = evt.target as UUISelectElement;
        if (selectEl) {
            this.#workspaceContext?.setModel({
                ...this._model!,
                [selectEl.name]: selectEl.value,
            });
        }
    }

    #onProductRootChange(event: CustomEvent & { target: { selection: string[] | undefined } }) {
        const selections = event.target.selection ?? [];
        this.#workspaceContext?.setModel({
            ...this._model!,
            productRootId: selections.join(','),
        });

        // this.#validateForm(); // TODO Dinh
    }

    #onProductDocumentTypeIdsChange(event: Event) {
        const element = event.target as UmbInputDocumentTypeElement;
        if (element.value) {
            this.#workspaceContext?.setModel({
                ...this._model!,
                productDocumentTypeIds: element.selection,
            });
        }
    }

    #onProductChildVariantTypeIdsChange(event: Event) {
        const element = event.target as UmbInputDocumentTypeElement;
        if (element.value) {
            this.#workspaceContext?.setModel({
                ...this._model!,
                productChildVariantTypeIds: element.selection,
            });
        }
    }


    #onClearInputClick(propName: string) {
        return () => {
            this.#workspaceContext?.setModel({
                ...this._model!,
                [propName]: '',
            });
        };
    }

    #onPropNodeMapperChange(evt: CustomEvent<FePropertyAndNodeMapDetails[]>) {
        const mapItems = evt.detail;
        console.log('changing', mapItems);
    }

    #renderLeftColumn() {
        console.log(this._model?.productRootId ? [this._model.productRootId] : []);
        return html`
            <uui-box headline=${this.localize.term('ucGeneral_general')}>
                <uc-stack look="loose" .divide=${true}>
                    <umb-property-layout label=${this.localize.term('ucProductFeed_prop:feedRelativePathLabel')}
                        description=${this.localize.term('ucProductFeed_prop:feedRelativePathDescription')}>
                        <uui-input
                            slot="editor"
                            name="feedRelativePath"
                            label=${this.localize.term('ucProductFeed_prop:feedRelativePathLabel')}
                            value=${this._model?.feedRelativePath ?? ''}
                            @input=${this.#onInputChange}
                        ></uui-input>
                    </umb-property-layout>
                    
                    <umb-property-layout
                        label=${this.localize.term('ucProductFeed_prop:feedDescriptionLabel')}
                        description=${this.localize.term('ucProductFeed_prop:feedDescriptionDescription')}>
                        <uui-input
                            slot="editor"
                            name="feedDescription"
                            label=${this.localize.term('ucProductFeed_prop:feedDescriptionLabel')}
                            value=${this._model?.feedDescription ?? ''}
                            @input="${this.#onInputChange}"
                        ></uui-input>
                    </umb-property-layout>

                    <umb-property-layout label=${this.localize.term('ucGeneral_prop:feedTypeLabel')}
                        description=${this.localize.term('ucProductFeed_feedTypeDescription')}>
                        <uui-select
                            label=${this.localize.term('ucGeneral_prop:feedTypeLabel')}
                            placeholder=${`-- ${this.localize.term('ucPlaceholders_selectAnItem')} --`}
                            slot='editor'
                            name='feedType'
                            .options=${this._feedTypes}
                            @change=${this.#onSelectElementChange}>
                        </uui-select>
                    </umb-property-layout>

                    ${this._model?.productDocumentTypeAliases && html`
                        <umb-property-layout
                            label=${this.localize.term('ucProductFeed_prop:productDocumentTypeAliasesLabel')}
                            description=${this.localize.term('ucProductFeed_prop:productDocumentTypeAliasesDescription')}
                        >
                            <uui-input
                                slot="editor"
                                name="productDocumentTypeAliases"
                                label=${this.localize.term('ucProductFeed_prop:productDocumentTypeAliasesLabel')}
                                value=${this._model?.productDocumentTypeAliases?.join(';')}
                                @input="${this.#onInputChange}"
                                disabled
                            ></uui-input>
                        </umb-property-layout>
                    `}

                    <umb-property-layout
                            label=${this.localize.term('ucProductFeed_prop:productDocumentTypeIdsLabel')}
                            description=${this.localize.term('ucProductFeed_prop:productDocumentTypeIdsDescription')}
                            ?mandatory=${true}
                        >
                        <umb-input-document-type
                            name='productDocumentTypeIds'
                            slot='editor'
                            @change=${this.#onProductDocumentTypeIdsChange}
                        ></umb-input-document-type>
                    </umb-property-layout>

                    ${this._model?.productChildVariantTypeAlias && html`
                        <umb-property-layout
                            label=${this.localize.term('ucProductFeed_prop:productChildVariantTypeAliasLabel')}
                            description=${this.localize.term('ucProductFeed_prop:productChildVariantTypeAliasDescription')}
                        >
                            <uui-input
                                slot="editor"
                                name="productChildVariantTypeAlias"
                                label=${this.localize.term('ucProductFeed_prop:productChildVariantTypeAlias')}
                                value=${this._model?.productChildVariantTypeAlias ?? ''}
                                disabled
                            ></uui-input>
                            ${this._model?.productChildVariantTypeAlias && html`<uui-button slot='editor' class='ucpf-clearPropButton' look='secondary' @click=${this.#onClearInputClick('productChildVariantTypeAlias')} label=${this.localize.term('general_remove')}></uui-button>`}
                        </umb-property-layout>
                    `}

                    <umb-property-layout
                        label=${this.localize.term('ucProductFeed_prop:productChildVariantTypeIdsLabel')}
                        description=${this.localize.term('ucProductFeed_prop:productChildVariantTypeIdsDescription')}
                        ?mandatory=${true}
                    >
                        <umb-input-document-type
                            slot='editor'
                            @change=${this.#onProductChildVariantTypeIdsChange}
                        ></umb-input-document-type>
                    </umb-property-layout>

                    <umb-property-layout
                        label=${this.localize.term('ucProductFeed_prop:productRootIdLabel')}
                        description=${this.localize.term('ucProductFeed_prop:productRootIdDescription')}
                        ?mandatory=${true}
                    >
                        <umb-input-document
                            slot='editor'
                            .type=${'content'}
                            .max=${1}
                            ?showOpenButton=${true}
                            @change=${this.#onProductRootChange}
                            .value=${this._model?.productRootId ?? ''}>
                        </umb-input-document>
                        ${this._model?.productRootId && html`<uui-button class='ucpf-clearPropButton' slot='editor' look='secondary' @click=${this.#onClearInputClick('productRootId')} label=${this.localize.term('general_remove')}></uui-button>`}
                    </umb-property-layout>

                    <umb-property-layout
                        label=${this.localize.term('ucProductFeed_prop:propNodeMappingLabel')}
                        description=${this.localize.term('ucProductFeed_prop:propNodeMappingDescription')}
                        ?mandatory=${true}
                    >

                        <ucpf-property-node-mapper
                            slot='editor'
                            .mapItems=${this._model?.propertyNameMappings ?? []}
                            @change=${this.#onPropNodeMapperChange}
                        ></ucpf-property-node-mapper>

                    </umb-property-layout>
                </uc-stack>
            </uui-box>
        `;
    }

    #renderRightColumn() {
        return html`
            <uui-box headline=${this.localize.term('ucGeneral_info')}>
                <uc-stack look="compact">
                    <uc-info-item label-key="general_id">
                        ${!this._id
                ? html`<uui-tag color="default" look="placeholder">${this.localize.term('ucGeneral_unsaved')}</uui-tag>`
                : this._id}
                    </uc-info-item>
                    <uc-info-item label-key="ucGeneral_storeId">${this._store?.id}</uc-info-item>
                </uc-stack>
            </uui-box>
        `;
    }

    render() {
        return html`
            <uc-workspace-editor-layout>
                <div>${this.#renderLeftColumn()}</div>
                <div slot="aside">${this.#renderRightColumn()}</div>
            </uc-workspace-editor-layout>
        `;
    }

    static styles = [
        UmbTextStyles,
        css`
            :host {
                display: block;
            }

            umb-property-layout {
                padding: 0;
                border: 0;
            }

            uui-input,
            uui-select,
            uui-input{
                width: 100%;
            }
            
            .ucpf-clearPropButton{
                width: 100%;
                margin-top: 0.5em;
            }
        `,
    ];
}

export default UcpfDetailsWorkspaceViewElement;

declare global {
    interface HTMLElementTagNameMap {
        [ELEMENT_NAME]: UcpfDetailsWorkspaceViewElement;
    }
}
