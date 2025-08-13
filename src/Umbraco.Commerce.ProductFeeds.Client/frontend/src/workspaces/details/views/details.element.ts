import type { UmbWorkspaceViewElement } from '@umbraco-cms/backoffice/workspace';
import type { UUIBooleanInputElement, UUIEvent, UUIInputElement, UUISelectElement } from '@umbraco-cms/backoffice/external/uui';
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

    @state()
    private _propertyValueExtractorOptions: Option[] = [];

    constructor() {
        super();
        this.consumeContext(DETAILS_WORKSPACE_CONTEXT, (ctx) => {
            this.#workspaceContext = ctx;
            this.#observeWorkspace();
        });

        this.consumeContext(UC_STORE_CONTEXT, (ctx) => {
            this.observe(ctx?.store, (store) => {
                this._store = store;
            });
        });
    }

    #observeWorkspace() {
        if (!this.#workspaceContext) return;

        this.observe(this.#workspaceContext.unique, (unique) => (this._id = unique));

        this.observe(this.#workspaceContext.model, (model) => {
            this._model = model;
            this._feedTypes = this.#markSelectedOption(this._feedTypes, model!.feedGeneratorId);
        });

        this.observe(this.#workspaceContext.feedTypeOptions, (feedTypes) => {
            this._feedTypes = this.#markSelectedOption(feedTypes, this._model?.feedGeneratorId);
        });

        this.observe(this.#workspaceContext.propertyValueExtractorOptions, (options) => {
            this._propertyValueExtractorOptions = options;
        });
    }

    #markSelectedOption(options: Option[], value?: string) {
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
        }
    }

    #onCheckboxChange(evt: UUIEvent) {
        const checkboxEl = evt.target as UUIBooleanInputElement;
        if (checkboxEl) {
            this.#workspaceContext?.setModel({
                ...this._model!,
                [checkboxEl.name]: checkboxEl.checked,
            });
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
            productRootId: selections.join(',') || undefined,
        });
    }

    #onProductDocumentTypeIdsChange(event: Event) {
        const element = event.target as UmbInputDocumentTypeElement;
        if (element) {
            this.#workspaceContext?.setModel({
                ...this._model!,
                productDocumentTypeIds: element.selection,
            });
        }
    }

    #onProductChildVariantTypeIdsChange(event: Event) {
        const element = event.target as UmbInputDocumentTypeElement;
        if (element) {
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
                [propName]: undefined,
            });
        };
    }

    #onPropNodeMapperChange(evt: CustomEvent<FePropertyAndNodeMapDetails[]>) {
        const mapItems = evt.detail;
        this.#workspaceContext?.setModel({
            ...this._model!,
            propertyNameMappings: mapItems,
        });
    }

    #renderLeftColumn() {
        return html`
            <uui-box headline=${this.localize.term('ucGeneral_general')}>
                <uc-stack look="loose" .divide=${true}>
                    <umb-property-layout label=${this.localize.term('ucProductFeeds_propFeedRelativePathLabel')}
                        description=${this.localize.term('ucProductFeeds_propFeedRelativePathDescription')}
                        mandatory>
                        <uui-input
                            slot="editor"
                            name="feedRelativePath"
                            label=${this.localize.term('ucProductFeeds_propFeedRelativePathLabel')}
                            value=${this._model?.feedRelativePath ?? ''}
                            @input=${this.#onInputChange}
                        ></uui-input>
                    </umb-property-layout>
                    
                    <umb-property-layout
                        label=${this.localize.term('ucProductFeeds_propFeedDescriptionLabel')}
                        description=${this.localize.term('ucProductFeeds_propFeedDescriptionDescription')}>
                        <uui-input
                            slot="editor"
                            name="feedDescription"
                            label=${this.localize.term('ucProductFeeds_propFeedDescriptionLabel')}
                            value=${this._model?.feedDescription ?? ''}
                            @input="${this.#onInputChange}"
                        ></uui-input>
                    </umb-property-layout>

                    <umb-property-layout label=${this.localize.term('ucProductFeeds_propFeedTypeLabel')}
                        description=${this.localize.term('ucProductFeeds_propFeedTypeDescription')}
                        mandatory>
                        <uui-select
                            label=${this.localize.term('ucProductFeeds_propFeedTypeLabel')}
                            placeholder=${`-- ${this.localize.term('ucPlaceholders_selectAnItem')} --`}
                            slot='editor'
                            name='feedGeneratorId'
                            .options=${this._feedTypes}
                            @change=${this.#onSelectElementChange}>
                        </uui-select>
                    </umb-property-layout>

                    <umb-property-layout
                            label=${this.localize.term('ucProductFeeds_propProductDocumentTypeIdsLabel')}
                            description=${this.localize.term('ucProductFeeds_propProductDocumentTypeIdsDescription')}
                            ?mandatory=${true}
                        >
                        <umb-input-document-type
                            name='productDocumentTypeIds'
                            slot='editor'
                            @change=${this.#onProductDocumentTypeIdsChange}
                            ?documentTypesOnly=${true}
                            .selection=${this._model?.productDocumentTypeIds ?? []}
                        ></umb-input-document-type>
                    </umb-property-layout>

                    <umb-property-layout
                        label=${this.localize.term('ucProductFeeds_propProductChildVariantTypeIdsLabel')}
                        description=${this.localize.term('ucProductFeeds_propProductChildVariantTypeIdsDescription')}
                    >
                        <umb-input-document-type
                            slot='editor'
                            @change=${this.#onProductChildVariantTypeIdsChange}
                            .selection=${this._model?.productChildVariantTypeIds ?? []}
                        ></umb-input-document-type>
                    </umb-property-layout>

                    <umb-property-layout
                        label=${this.localize.term('ucProductFeeds_propProductRootIdLabel')}
                        description=${this.localize.term('ucProductFeeds_propProductRootIdDescription')}
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
                        label=${this.localize.term('ucProductFeeds_propIncludeTaxInPriceLabel')}
                        description=${this.localize.term('ucProductFeeds_propIncludeTaxInPriceDescription')}
                    >
                        <uui-toggle
                            slot="editor"
                            label=""
                            name="includeTaxInPrice"
                            @change=${this.#onCheckboxChange}
                            ?checked=${!!this._model?.includeTaxInPrice}
                        ></uui-toggle>
                    </umb-property-layout>

                    <umb-property-layout
                        label=${this.localize.term('ucProductFeeds_propPropNodeMappingLabel')}
                        description=${this.localize.term('ucProductFeeds_propPropNodeMappingDescription')}
                        ?mandatory=${true}
                    >
                        <ucpf-property-node-mapper
                            slot='editor'
                            .mapItems=${this._model?.propertyNameMappings ?? []}
                            .propertyValueExtractorOptions=${this._propertyValueExtractorOptions}
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
