import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import { WORKSPACE_ALIAS, WORKSPACE_ENTITY_ALIAS } from '../constants.js';
import { UmbWorkspaceContext, UmbWorkspaceRouteManager } from '@umbraco-cms/backoffice/workspace';
import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import UcProductFeedsWorkspaceCollectionElement from './workspace.element.js';

export const UC_PRODUCT_FEEDS_WORKSPACE_CONTEXT = new UmbContextToken<
    UmbWorkspaceContext,
    UcProductFeedsWorkspaceContext
>(
    'UmbWorkspaceContext',
    undefined,
    (context): context is UcProductFeedsWorkspaceContext => context.getEntityType() === WORKSPACE_ENTITY_ALIAS,
);

export class UcProductFeedsWorkspaceContext extends UmbControllerBase
    implements UmbWorkspaceContext {
    readonly routes = new UmbWorkspaceRouteManager(this);

    constructor(host: UmbControllerHost) {
        super(host);
        this.workspaceAlias = WORKSPACE_ALIAS;
        this.provideContext(UC_PRODUCT_FEEDS_WORKSPACE_CONTEXT, this);
        this.routes.setRoutes([
            {
                path: '',
                component: UcProductFeedsWorkspaceCollectionElement,
            },
        ]);
    }

    workspaceAlias: string;

    getEntityType(): string {
        return WORKSPACE_ENTITY_ALIAS;
    }
}

