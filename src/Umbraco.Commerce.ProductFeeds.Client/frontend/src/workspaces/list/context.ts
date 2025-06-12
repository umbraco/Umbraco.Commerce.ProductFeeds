import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import { UmbEntityWorkspaceContext, UmbRoutableWorkspaceContext, UmbWorkspaceContext, UmbWorkspaceRouteManager } from '@umbraco-cms/backoffice/workspace';
import UcpfListWorkspaceElement from './index.element.js';
import { listingWorkspaceManifest } from './manifests.js';
import { UmbBasicState } from '@umbraco-cms/backoffice/observable-api';
import { UmbEntityContext, UmbEntityUnique } from '@umbraco-cms/backoffice/entity';
import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';

export const LISTING_WORKSPACE_CONTEXT = new UmbContextToken<
    UmbWorkspaceContext,
    UcpfListingWorkspaceContext
>(
    'UmbWorkspaceContext',
    'UcpfListingWorkspaceContext',
);

export class UcpfListingWorkspaceContext
    extends UmbContextBase
    implements UmbWorkspaceContext, UmbRoutableWorkspaceContext, UmbEntityWorkspaceContext {
    readonly routes = new UmbWorkspaceRouteManager(this);
    readonly workspaceAlias = listingWorkspaceManifest.alias;

    #entityContext: UmbEntityContext = new UmbEntityContext(this);

    #entityType: UmbBasicState<string | undefined> = new UmbBasicState<string | undefined>(undefined);
    readonly entityType = this.#entityType.asObservable();

    #unique: UmbBasicState<UmbEntityUnique | undefined> = new UmbBasicState<UmbEntityUnique | undefined>(undefined);
    readonly unique = this.#unique.asObservable();

    constructor(host: UmbControllerHost) {
        super(host, listingWorkspaceManifest.alias);
        this.observe(this.entityType, (entityType) => this.#entityContext!.setEntityType(entityType));
        this.#entityType.setValue(listingWorkspaceManifest.meta.entityType);

        this.observe(this.unique, (unique) => this.#entityContext.setUnique(unique ?? null));
        this.#unique.setValue(this.workspaceAlias);

        this.routes.setRoutes([
            {
                path: '',
                component: UcpfListWorkspaceElement,
            },
        ]);
    }

    getUnique(): UmbEntityUnique | undefined {
        return this.#entityContext.getUnique();
    }

    getEntityType(): string {
        return this.#entityContext.getEntityType()!;
    }
}

