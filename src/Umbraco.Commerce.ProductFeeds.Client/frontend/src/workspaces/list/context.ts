import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import { UmbDefaultWorkspaceContext, UmbEntityWorkspaceContext, UmbRoutableWorkspaceContext, UmbWorkspaceContext, UmbWorkspaceRouteManager, UmbWorkspaceUniqueType } from '@umbraco-cms/backoffice/workspace';
import UcpfListWorkspaceElement from './index.element.js';
import { listingWorkspaceManifest } from './manifests.js';
import { UmbBasicState } from '@umbraco-cms/backoffice/observable-api';
import { UmbEntityContext } from '@umbraco-cms/backoffice/entity';

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
    implements UmbWorkspaceContext, UmbRoutableWorkspaceContext, UmbEntityWorkspaceContext {
    readonly routes = new UmbWorkspaceRouteManager(this);

    #entityContext: UmbEntityContext = new UmbEntityContext(this);

    #entityType: UmbBasicState<string | undefined> = new UmbBasicState<string | undefined>(undefined);
    readonly entityType = this.#entityType.asObservable();

    #unique: UmbBasicState<UmbWorkspaceUniqueType | undefined> = new UmbBasicState<UmbWorkspaceUniqueType | undefined>(undefined);
    readonly unique = this.#unique.asObservable();

    constructor(host: UmbControllerHost) {
        super(host);
        this.observe(this.entityType, (entityType) => this.#entityContext!.setEntityType(entityType));
        this.observe(this.unique, (unique) => this.#entityContext.setUnique(unique ?? null));
        this.routes.setRoutes([
            {
                path: '',
                component: UcpfListWorkspaceElement,
            },
        ]);
    }

    override getEntityType(): string {
        return listingWorkspaceManifest.meta.entityType;
    }
}

