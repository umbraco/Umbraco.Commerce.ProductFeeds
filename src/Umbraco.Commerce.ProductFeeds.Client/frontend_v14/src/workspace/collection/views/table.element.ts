import type { UmbDefaultCollectionContext } from '@umbraco-cms/backoffice/collection';
import { UMB_COLLECTION_CONTEXT } from '@umbraco-cms/backoffice/collection';
import type { UmbTableColumn, UmbTableConfig, UmbTableDeselectedEvent, UmbTableElement, UmbTableItem, UmbTableSelectedEvent } from '@umbraco-cms/backoffice/components';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { customElement, html, css, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { ProductFeedSettingReadModel } from '../../../generated/apis/types.gen';
import { editRoute } from '../../fe-routes';

const elementName = 'uc-product-feeds-table-collection-view';
@customElement(elementName)
export class UcProductFeedsTableCollectionViewElement extends UmbLitElement {

    @state()
    private _storeId?: string;

    @state()
    private _tableConfig: UmbTableConfig = {
        allowSelection: true,
    };

    @state()
    private _tableColumns: Array<UmbTableColumn> = [
        {
            alias: 'name',
            name: this.localize.term('ucGeneral_name'),
        },
        {
            alias: 'feedTypeName',
            name: this.localize.term('ucProductFeeds_typeLabel'),
        },
        {
            alias: 'feedRelativePath',
            name: this.localize.term('ucProductFeeds_pathLabel'),
        },
    ];

    @state()
    private _tableItems: Array<UmbTableItem> = [];

    @state()
    private _selection: Array<string> = [];

    #collectionContext?: UmbDefaultCollectionContext<UcEmailTemplateCollectionModel, UcStoreEntityCollectionFilterModel>;
    #collectionConfig?: UcStoreEntityCollectionConfiguration;

    constructor() {
        super();
        this.consumeContext(UMB_COLLECTION_CONTEXT, (instance) => {
            this.#collectionContext = instance;
            this.#collectionContext?.selection.setSelectable(true);
            this.#collectionConfig = this.#collectionContext?.getConfig() as UcStoreEntityCollectionConfiguration;
            this._storeId = this.#collectionConfig?.storeId;
            this.#observeCollectionItems();
        });
    }

    async #observeCollectionItems() {
        if (!this.#collectionContext) return;

        this.observe(
            this.#collectionContext.items,
            (collectionItems) => {
                this.#createTableItems(collectionItems);
            },
            'umbCollectionItemsObserver',
        );

        this.observe(
            this.#collectionContext.selection.selection,
            (selection) => {
                this._selection = selection as string[];
            },
            'umbCollectionSelectionObserver',
        );
    }

    #createTableItems(items: Array<ProductFeedSettingReadModel>) {
        this._tableItems = items.map((item) => {
            return {
                id: item.id,
                icon: 'icon-rss',
                data: [
                    {
                        columnAlias: 'name',
                        value: html`<uc-silent-link
                            href=${editRoute(this._storeId!, item.id)}>
							${item.feedName}</uc-silent-link> `,
                    },
                    {
                        columnAlias: 'feedTypeName',
                        value: html`<uc-enum-label .value=${item.feedTypeName}></uc-enum-label>`,
                    },
                    {
                        columnAlias: 'feedRelativePath',
                        value: html`<uc-enum-label .value=${item.feedRelativePath}></uc-enum-label>`,
                    },
                ],
            };
        });
    }

    #handleSelect(event: UmbTableSelectedEvent) {
        event.stopPropagation();
        const table = event.target as UmbTableElement;
        const selection = table.selection;
        this.#collectionContext?.selection.setSelection(selection);
    }


    #handleDeselect(event: UmbTableDeselectedEvent) {
        event.stopPropagation();
        const table = event.target as UmbTableElement;
        const selection = table.selection;
        this.#collectionContext?.selection.setSelection(selection);
    }

    render() {
        return html`
			<umb-table
                .config=${this._tableConfig}
                .columns=${this._tableColumns}
                .items=${this._tableItems}
                .selection=${this._selection}
				@selected="${this.#handleSelect}"
				@deselected="${this.#handleDeselect}"></umb-table>
		`;
    }

    static styles = [
        UmbTextStyles,
        css`
			:host {
				display: flex;
				flex-direction: column;
			}

            umb-table {
                padding-left: 0;
                padding-right: 0;
            }
		`,
    ];
}

export default UcProductFeedsTableCollectionViewElement;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: UcProductFeedsTableCollectionViewElement;
    }
}
