import { UmbControllerHostElement } from '@umbraco-cms/backoffice/controller-api';
import { UmbEntityActionArgs, UmbEntityActionBase } from '@umbraco-cms/backoffice/entity-action';
import { MetaEntityActionDefaultKind } from '@umbraco-cms/backoffice/entity-action';
import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { DETAILS_WORKSPACE_CONTEXT, UcpfDetailsWorkspaceContext } from '../context';

export class ProductFeedEntityActionDelete extends UmbEntityActionBase<MetaEntityActionDefaultKind> {
    #workspaceContext?: UcpfDetailsWorkspaceContext;

    constructor(host: UmbControllerHostElement, args: UmbEntityActionArgs<MetaEntityActionDefaultKind>) {
        super(host, args);

        this.consumeContext(DETAILS_WORKSPACE_CONTEXT, ctx => {
            this.#workspaceContext = ctx;
        });

    }
    async execute() {
        umbConfirmModal(this._host, {
            headline: 'Delete',
            content: 'Are you sure you want to delete this item?',
            color: 'danger',
            confirmLabel: 'Delete',
        }).then(async () => {
            await this.#workspaceContext?.deleteAsync();
        }, () => { });
    }
}