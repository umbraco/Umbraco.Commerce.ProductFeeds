import type {
    ManifestWorkspace,
    ManifestWorkspaceAction,
    ManifestWorkspaceView,
} from '@umbraco-cms/backoffice/extension-registry';
import { UmbSubmitWorkspaceAction } from '@umbraco-cms/backoffice/workspace';
import { UcpfDetailsWorkspaceContext } from './context.js';
import UcpfDetailsWorkspaceViewElement from './views/details.element.js';

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
        alias: `${detailsWorkspaceManifest.alias}:action:save`,
        name: 'Save Product Feed Details Action',
        api: UmbSubmitWorkspaceAction,
        meta: {
            label: '#general_save',
            look: 'primary',
            color: 'positive',
        },
        conditions: [
            {
                alias: 'Umb.Condition.WorkspaceAlias',
                match: detailsWorkspaceManifest.alias,
            },
        ],
    },
];

export const manifests = [
    detailsWorkspaceManifest,
    ...workspaceViews,
    ...workspaceActions,
];
