import { css, customElement, html, property, repeat } from '@umbraco-cms/backoffice/external/lit';
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

    @property({ type: Array })
    propertyValueExtractorOptions: Option[] = [];

    /**
     *
     */
    constructor() {
        super();
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
        
        ${repeat(this.mapItems, item => item.uiId, mapItem => html`
            <div
                class="ucpf-prop-mapping-row"
                ng-repeat="item in vm.propertyAndNodeMappingVm track by item.uiId"
                ng-model="item.valueExtractorName">
                <div class="ucpf-prop-mapping-col"
                    style="max-width: 200px;">
                    <uui-input
                        class="ucpf-prop-mapping-control"
                        type="text"
                        label=${this.localize.term('ucProductFeed_propNodeMapperNodeName')}
                        placeholder="Feed Node Name"
                        required
                        name='nodeName'
                        value=${mapItem.nodeName}
                        @input=${(e: CustomEvent) => this.#onMapItemChange(e, mapItem.uiId)}></uui-input>
                </div>
                
                <div class="ucpf-prop-mapping-col"
                    style="max-width: 200px;">
                    <uui-input
                        class="ucpf-prop-mapping-control"
                        type="text"
                        label=${this.localize.term('ucProductFeed_propNodeMapperPropertyAlias')}
                        placeholder="Property Alias"
                        value=${mapItem.propertyAlias}
                        name='propertyAlias'
                        @input=${(e: CustomEvent) => this.#onMapItemChange(e, mapItem.uiId)}
                        required></uui-input>
                </div>
                
                <div class="ucpf-prop-mapping-col">
                    <uui-select
                        placeholder=${`-- ${this.localize.term('ucPlaceholders_selectAnItem')} --`}
                        name='valueExtractorName'
                        label=${this.localize.term('ucProductFeed_propNodeMapperValueExtractorName')}
                        placeholder=${this.localize.term('ucProductFeed_propNodeMapperValueExtractorName')}
                        .options=${this
                .propertyValueExtractorOptions
                .map(extractor => {
                    return {
                        ...extractor,
                        selected: mapItem.valueExtractorName === extractor.value,
                    };
                })}
                        .title=${mapItem.valueExtractorName ?? ''}
                        @change = ${(e: CustomEvent) => this.#onMapItemChange(e, mapItem.uiId)}>
                    </uui-select>
                    <uui-button
                        @click=${() => this.#onRemoveItemClick(mapItem.uiId)}
                        label=${this.localize.term('general_remove')}
                        title=${this.localize.term('general_remove')}>
                        <uui-icon name='icon-trash'></uui-icon>
                    </uui-button>
                </div>
            </div>
        `)
            }
        
        <div class="ucpf-prop-mapping-row" >
    <div class="ucpf-prop-mapping-col" >
        <uui-button
class='ucpf-add-map-item-btn'
@click=${this.#onAddMapItemClick}
look = 'placeholder'
label = ${this.localize.term('general_add')}>
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

        .ucpf-add-map-item-btn {
            width: 100%;
        }
    `;
}

declare global {
    interface HTMLElementTagNameMap {
        [ELEMENT_NAME]: UcpfPropNodeMapper;
    }
}
