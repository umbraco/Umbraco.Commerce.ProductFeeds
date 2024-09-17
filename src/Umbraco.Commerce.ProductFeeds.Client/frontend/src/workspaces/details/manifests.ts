import type {
    ManifestEntityAction,
    ManifestWorkspace,
    ManifestWorkspaceAction,
    ManifestWorkspaceView,
} from '@umbraco-cms/backoffice/extension-registry';
import { UmbSubmitWorkspaceAction } from '@umbraco-cms/backoffice/workspace';
import { UcpfDetailsWorkspaceContext } from './context.js';
import UcpfDetailsWorkspaceViewElement from './views/details.element.js';
import { ProductFeedEntityActionDelete } from './actions/entity-action.delete.js';
import { UcpfWorkspaceActionOpenFeed } from './actions/workspace-action.open-feed.js';

export const detailsWorkspaceManifest: ManifestWorkspace = {
    type: 'workspace',
    kind: 'routable',
    alias: 'uc:workspace:product-feed',
    name: 'Product Feed Details Workspace',
    api: UcpfDetailsWorkspaceContext,
    meta: {
        entityType: 'uc:product-feed',
    },
};

const workspaceViews: Array<ManifestWorkspaceView> = [
    {
        type: 'workspaceView',
        alias: `${detailsWorkspaceManifest.alias}:view:details`,
        name: 'Product Feed Details Workspace View',
        element: UcpfDetailsWorkspaceViewElement,
        weight: 90,
        meta: {
            label: 'Details',
            pathname: 'details',
            icon: 'edit',
        },
        conditions: [
            {
                alias: 'Umb.Condition.WorkspaceAlias',
                match: detailsWorkspaceManifest.alias,
            },
        ],
    },
];

const workspaceActions: Array<ManifestWorkspaceAction> = [
    {
        type: 'workspaceAction',
        kind: 'default',
        alias: `${detailsWorkspaceManifest.alias}:action:openFeed`,
        name: 'Open Product Feed',
        api: UcpfWorkspaceActionOpenFeed,
        meta: {
            label: '#ucProductFeeds_buttonOpenFeed',
            look: 'secondary',
        },
        conditions: [
            {
                alias: 'Umb.Condition.WorkspaceAlias',
                match: detailsWorkspaceManifest.alias,
            },
        ],
        weight: 901,
    },
    {
        type: 'workspaceAction',
        kind: 'default',
        alias: `${detailsWorkspaceManifest.alias}:action:save`,
        name: 'Save Product Feed Details Action',
        api: UmbSubmitWorkspaceAction,
        meta: {
            label: '#buttons_save',
            look: 'primary',
            color: 'positive',
        },
        conditions: [
            {
                alias: 'Umb.Condition.WorkspaceAlias',
                match: detailsWorkspaceManifest.alias,
            },
        ],
        weight: 900,
    },
];

export const manifests = [
    detailsWorkspaceManifest,
    ...workspaceViews,
    ...workspaceActions,
    {
        type: 'entityAction',
        kind: 'default',
        alias: 'Forms.EntityAction.Form.Delete',
        name: 'Delete Form Entity Action',
        weight: 50,
        api: ProductFeedEntityActionDelete,
        forEntityTypes: [detailsWorkspaceManifest.meta.entityType],
        meta: {
            icon: 'icon-delete',
            label: 'Delete...',
        },
    } as ManifestEntityAction,
];
