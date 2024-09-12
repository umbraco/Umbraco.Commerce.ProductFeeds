import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import { UmbDefaultWorkspaceContext, UmbRoutableWorkspaceContext, UmbWorkspaceContext, UmbWorkspaceRouteManager } from '@umbraco-cms/backoffice/workspace';
import UcpfListWorkspaceElement from './index.element.js';
import { listingWorkspaceManifest } from './manifests.js';

export const LISTING_WORKSPACE_CONTEXT = new UmbContextToken<
    UmbWorkspaceContext,
    UcpfListListingWorkspaceContext
>(
    'UmbWorkspaceContext',
    undefined,
    (context): context is UcpfListListingWorkspaceContext => context.getEntityType() === listingWorkspaceManifest.meta.entityType,
);

export class UcpfListListingWorkspaceContext
    extends UmbDefaultWorkspaceContext
    implements UmbWorkspaceContext, UmbRoutableWorkspaceContext {
    readonly routes = new UmbWorkspaceRouteManager(this);

    constructor(host: UmbControllerHost) {
        super(host);
        this.routes.setRoutes([
            {
                path: '',
                component: UcpfListWorkspaceElement,
            },
        ]);
    }
}

