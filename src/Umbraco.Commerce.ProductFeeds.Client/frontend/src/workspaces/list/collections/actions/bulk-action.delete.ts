import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbEntityBulkActionArgs, UmbEntityBulkActionBase } from '@umbraco-cms/backoffice/entity-bulk-action';
import { MetaEntityBulkAction } from '@umbraco-cms/backoffice/extension-registry';
import { UMB_COLLECTION_CONTEXT, UmbDefaultCollectionContext } from '@umbraco-cms/backoffice/collection';
import { UcpfListCollectionRepository } from '../../repository';
import { UMB_ACTION_EVENT_CONTEXT } from '@umbraco-cms/backoffice/action';
import { UmbRequestReloadChildrenOfEntityEvent } from '@umbraco-cms/backoffice/entity-action';
import { UMB_ENTITY_CONTEXT } from '@umbraco-cms/backoffice/entity';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { UmbLocalizationController } from '@umbraco-cms/backoffice/localization-api';
import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';

export class UcpfListBulkDeleteAction extends UmbEntityBulkActionBase<MetaEntityBulkAction> {
    #collectionContext?: UmbDefaultCollectionContext;
    #repository: UcpfListCollectionRepository;

    constructor(host: UmbControllerHost, args: UmbEntityBulkActionArgs<MetaEntityBulkAction>) {
        super(host, args);
        this.consumeContext(UMB_COLLECTION_CONTEXT, ctx => {
            this.#collectionContext = ctx;
        });
        this.#repository = new UcpfListCollectionRepository(host);
    }
    async execute(): Promise<void> {
        const selectedIds = this.#collectionContext?.selection.getSelection() ?? [];
        if (selectedIds.length > 0) {

            try {
                await umbConfirmModal(this._host, {
                    headline: 'UCPF Delete Item',
                    content: `Are you sure you want to delete ${this.selection.length} items?`,
                    color: 'danger',
                    confirmLabel: 'Delete',
                });
            } catch {
                return;
            }

            const { isSuccess, error } = await this.#repository.deleteAsync(selectedIds.filter(x => x != null));
            const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
            const localize = new UmbLocalizationController(this._host);
            if (isSuccess) {

                const entityContext = await this.getContext(UMB_ENTITY_CONTEXT);
                if (!entityContext) throw new Error('Entity Context is not available');

                const entityType = entityContext.getEntityType();
                const unique = entityContext.getUnique();

                const eventContext = await this.getContext(UMB_ACTION_EVENT_CONTEXT);
                if (!eventContext) throw new Error('Event Context is not available');
                const event = new UmbRequestReloadChildrenOfEntityEvent({
                    // entityType: listingWorkspaceManifest.meta.entityType,
                    entityType: entityType!,
                    unique: unique!,
                    // unique: null,
                });

                eventContext.dispatchEvent(event);
                console.log('bulk delete finished');
                // notificationContext.peek('positive', {
                //     data: {
                //         message: localize.term('ucProductFeeds_messageDeleteSuccess') ?? 'Delete successfully',
                //     },
                // });
            } else {
                notificationContext.peek('danger', {
                    data: {
                        headline: localize.term('ucProductFeeds_messageDeleteFailed') ?? 'Delete failed',
                        message: JSON.stringify(error),
                    },
                });
            }
        }
    }
}