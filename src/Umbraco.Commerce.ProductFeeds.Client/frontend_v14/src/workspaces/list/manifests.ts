import { ManifestCollection, ManifestCollectionAction, ManifestCollectionView, ManifestRepository, ManifestWorkspace } from '@umbraco-cms/backoffice/extension-registry';
import { UcpfListListingWorkspaceContext } from './context.js';
import { UcpfListCollectionRepository } from './repository.js';
import { CollectionCreateAction } from './collections/actions/action.create.js';
import { UMB_COLLECTION_ALIAS_CONDITION } from '@umbraco-cms/backoffice/collection';
import UcpfListCollectionViewTableElement from './collections/views/table.element.js';

export const listingWorkspaceManifest: ManifestWorkspace = {
    type: 'workspace',
    kind: 'routable',
    alias: 'uc:workspace:product-feeds',
    name: 'UC Product Feeds Listing Workspace',
    api: UcpfListListingWorkspaceContext,
    meta: {
        entityType: 'uc:product-feeds',
    },
};

const collectionRepositoryManifest: ManifestRepository = {
    type: 'repository',
    alias: 'uc:product-feeds:repository',
    name: 'Product Feeds Listing Collection Repository',
    api: UcpfListCollectionRepository,
};

export const listingWorkspaceCollectionManifest: ManifestCollection = {
    type: 'collection',
    kind: 'default',
    alias: 'uc:product-feeds:collection',
    name: 'UC Product Feeds Listing Collection',
    meta: {
        repositoryAlias: collectionRepositoryManifest.alias,
    },
};

const createCollectionActionManifest: ManifestCollectionAction =
{
    type: 'collectionAction',
    alias: 'UcpfList.EntityAction.Create',
    name: 'Product Feeds Listing Collection Action - Create',
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
    alias: 'uc:product-feeds:collection:view:table',
    name: 'Product Feeds Collection View',
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
];