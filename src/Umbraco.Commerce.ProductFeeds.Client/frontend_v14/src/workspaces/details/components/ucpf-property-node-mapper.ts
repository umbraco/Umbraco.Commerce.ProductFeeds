import { css, customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { FePropertyAndNodeMapDetails } from '../types';
import { UUIEvent } from '@umbraco-cms/backoffice/external/uui';
import { nanoid } from 'nanoid';

export interface UcpfPropNodeMapItemChangeEventData { itemId: string, value: string }
export interface UcpfPropNodeMapItemRemoveEventData { itemId: string }

const ELEMENT_NAME = 'ucpf-property-node-mapper';
@customElement(ELEMENT_NAME)
export class UcpfPropNodeMapper extends UmbLitElement {
    @property({ type: Array })
    mapItems: FePropertyAndNodeMapDetails[] = [];

    @state()
    private _propertyValueExtractors: Option[] = [];

    /**
     *
     */
    constructor() {
        super();
        console.log('UcpfPropNodeMapper ctor');
    }

    #onMapItemChange(evt: UUIEvent, itemId: string) {
        const element = evt.target as HTMLFormElement;
        if (element) {
            const mapItems = this.mapItems.map(item => {
                if (item.uiId !== itemId) {
                    return item;
                }

                return {
                    ...item,
                    [element.name]: element.value,
                };
            });

            this.dispatchEvent(new CustomEvent<FePropertyAndNodeMapDetails[]>('change', {
                detail: mapItems,
            }));

            // this.dispatchEvent(new CustomEvent('item-change', {
            //     detail: {
            //         itemId,
            //         value: element.value?.ToString() ?? '',
            //     },
            // }));
        }
    }

    #onAddMapItemClick() {
        const mapItems = [
            ...this.mapItems,
            {
                uiId: nanoid(),
                propertyAlias: '',
                nodeName: '',
                valueExtractorName: 'DefaultSingleValuePropertyExtractor',
            },
        ];

        // this.dispatchEvent(new CustomEvent('item-add'));

        this.dispatchEvent(new CustomEvent<FePropertyAndNodeMapDetails[]>('change', {
            detail: mapItems,
        }));
    }

    #onRemoveItemClick(itemId: string) {
        const mapItems = this.mapItems.filter(item => item.uiId !== itemId);
        this.dispatchEvent(new CustomEvent<FePropertyAndNodeMapDetails[]>('change', {
            detail: mapItems,
        }));
        // this.dispatchEvent(new CustomEvent<UcpfPropNodeMapItemRemoveEventData>('item-remove', { detail: { itemId } }));
    }

    render() {
        console.log('ucpf-prop-mapping');
        return html`
        <div class="ucpf-prop-mapping-row" style="font-weight: 600;">
            <div class="ucpf-prop-mapping-col"
                style="max-width: 200px;">
                <div class="umb-property-editor">Node name in feed</div>
            </div>
            <div class="ucpf-prop-mapping-col"
                style="max-width: 200px;">
                <div class="umb-property-editor">Property alias</div>
            </div>
            <div class="ucpf-prop-mapping-col">
                <div class="umb-property-editor">Value extractor</div>
            </div>
        </div>
        
        ${this.mapItems.map(mapItem => html`
            <div
                class="ucpf-prop-mapping-row"
                ng-repeat="item in vm.propertyAndNodeMappingVm track by item.uiId"
                ng-model="item.valueExtractorName">
                <div class="ucpf-prop-mapping-col"
                    style="max-width: 200px;">
                    <uui-input
                        class="ucpf-prop-mapping-control"
                        type="text"
                        label=${this.localize.term('ucProductFeed_propNodeMapper_nodeName')}
                        placeholder="Feed Node Name"
                        required
                        name='nodeName'
                        value=${mapItem.nodeName}
                        @change=${(e: CustomEvent) => this.#onMapItemChange(e, mapItem.uiId)}></uui-input>
                </div>
                
                <div class="ucpf-prop-mapping-col"
                    style="max-width: 200px;">
                    <uui-input
                        class="ucpf-prop-mapping-control"
                        type="text"
                        label=${this.localize.term('ucProductFeed_propNodeMapper_propertyAlias')}
                        placeholder="Property Alias"
                        value=${mapItem.propertyAlias}
                        name='propertyAlias'
                        @change=${(e: CustomEvent) => this.#onMapItemChange(e, mapItem.uiId)}
                        required></uui-input>
                </div>
                
                <div class="ucpf-prop-mapping-col">
                    <uui-select
                        placeholder=${`-- ${this.localize.term('ucPlaceholders_selectAnItem')} --`}
                        name='valueExtractorName'
                        label=${this.localize.term('ucProductFeed_propNodeMapper_valueExtractorName')}
                        placeholder=${this.localize.term('ucProductFeed_propNodeMapper_valueExtractorName')}
                        .options=${this._propertyValueExtractors.map(extractor => {
            return {
                ...extractor,
                selected: mapItem.valueExtractorName === extractor.value,
            };
        })}
                        @change=${(e: CustomEvent) => this.#onMapItemChange(e, mapItem.uiId)}>
                    </uui-select>
                    <uui-button
                        type="button"
                        @click=${() => this.#onRemoveItemClick(mapItem.uiId)}
                        label=${this.localize.term('general_remove')}
                        style="font-size: 1.2em; margin-left: 0.5em;">
                        <uui-icon name='trash'></uui-icon>
                        ${this.localize.term('general_remove')}
                    </uui-button>
                </div>
            </div>
        `)
            }
        
        <div class="ucpf-prop-mapping-row">
            <div class="ucpf-prop-mapping-col">
                <uui-button
                    type="button"
                    class="ucpf-add-map-item-btn"
                    @click=${this.#onAddMapItemClick}
                    look='placeholder'
                    label=${this.localize.term('general_add')}>
                    ${this.localize.term('general_add')}
                </uui-button>
            </div>
        </div>`;
    }

    static styles = css`
        .ucpf-prop-mapping-row {
            display: flex;
            flex-wrap: wrap;
            width: 100%;
            max-width: 800px;
            gap: 0.5em;
            box-sizing: border-box;
            padding: 0;
            margin-bottom: 0.5em;
        }
        
        .ucpf-prop-mapping-col {
            flex: 1 1;
            display: flex;
        }

        .ucpf-add-map -item-btn {
            width: 100%;
        }
    `;
}

declare global {
    interface HTMLElementTagNameMap {
        [ELEMENT_NAME]: UcpfPropNodeMapper;
    }
}
