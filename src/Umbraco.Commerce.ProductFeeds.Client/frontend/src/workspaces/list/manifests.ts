import { ManifestEntityBulkAction, ManifestRepository, MetaEntityBulkAction } from '@umbraco-cms/backoffice/extension-registry';
import { ManifestWorkspace } from '@umbraco-cms/backoffice/workspace';
import { ManifestCollectionAction, ManifestCollectionView } from '@umbraco-cms/backoffice/collection';
import { UcpfListingWorkspaceContext } from './context.js';
import { UcpfListCollectionRepository } from './repository.js';
import { CollectionCreateAction } from './collections/actions/action.create.js';
import { UMB_COLLECTION_ALIAS_CONDITION } from '@umbraco-cms/backoffice/collection';
import UcpfListCollectionViewTableElement from './collections/views/table.element.js';
import { UcpfListBulkDeleteAction } from './collections/actions/bulk-action.delete.js';
import { ManifestCollection } from '@umbraco-cms/backoffice/collection';

export const listingWorkspaceManifest: ManifestWorkspace = {
    type: 'workspace',
    kind: 'routable',
    alias: 'ucpf:workspace:product-feeds',
    name: 'Ucpf Listing Workspace',
    api: UcpfListingWorkspaceContext,
    meta: {
        entityType: 'uc:product-feeds',
    },
};

const collectionRepositoryManifest: ManifestRepository = {
    type: 'repository',
    alias: 'ucpf:repository',
    name: 'Ucpf Listing Collection Repository',
    api: UcpfListCollectionRepository,
};

export const listingWorkspaceCollectionManifest: ManifestCollection = {
    type: 'collection',
    kind: 'default',
    alias: 'ucpf:collection',
    name: 'Ucpf Listing Collection',
    meta: {
        repositoryAlias: collectionRepositoryManifest.alias,
    },
};

const createCollectionActionManifest: ManifestCollectionAction =
{
    type: 'collectionAction',
    alias: 'ucpf:listing:collection:action:create',
    name: 'Ucpf Listing Collection Action - Create',
    kind: 'button',
    weight: 10,
    api: CollectionCreateAction,
    conditions: [
        {
            alias: UMB_COLLECTION_ALIAS_CONDITION,
            match: listingWorkspaceCollectionManifest.alias,
        },
    ],
    meta: {
        label: '#general_create',
    },
};

const tableCollectionViewManifest: ManifestCollectionView = {
    type: 'collectionView',
    alias: 'ucpf:listing:collection:view:table',
    name: 'Ucpf Listing Collection View - Table',
    element: UcpfListCollectionViewTableElement,
    meta: {
        label: 'Table',
        icon: 'icon-list',
        pathName: 'table',
    },
    conditions: [
        {
            alias: UMB_COLLECTION_ALIAS_CONDITION,
            match: listingWorkspaceCollectionManifest.alias,
        },
    ],
};

export const manifests = [
    listingWorkspaceManifest,
    collectionRepositoryManifest,
    listingWorkspaceCollectionManifest,
    createCollectionActionManifest,
    tableCollectionViewManifest,
    {
        type: 'entityBulkAction',
        name: 'Ucpf Listing Bulk Action Delete',
        alias: 'ucpf:listing:collection:bulk-action:delete',
        api: UcpfListBulkDeleteAction,
        forEntityTypes: [
            listingWorkspaceManifest.meta.entityType,
        ],
        meta: {
            label: '#actions_delete',
        } as MetaEntityBulkAction,
        conditions: [
            {
                alias: UMB_COLLECTION_ALIAS_CONDITION,
                match: listingWorkspaceCollectionManifest.alias,
            },
        ],
    } as ManifestEntityBulkAction,
];