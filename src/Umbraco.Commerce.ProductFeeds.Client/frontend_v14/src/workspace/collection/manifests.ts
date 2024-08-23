import { ManifestRepository, ManifestTypes } from '@umbraco-cms/backoffice/extension-registry';
import { WORKSPACE_COLLECTION_ALIAS, WORKSPACE_REPOSITORY_ALIAS } from '../../constants';
import { CollectionCreateAction } from './action.create.js';
import { UMB_COLLECTION_ALIAS_CONDITION } from '@umbraco-cms/backoffice/collection';

const collectionManifest: ManifestTypes = {
    type: 'collection',
    kind: 'default',
    alias: WORKSPACE_COLLECTION_ALIAS,
    name: 'Product Feeds Collection',
    meta: {
        repositoryAlias: WORKSPACE_REPOSITORY_ALIAS,
    },
};

const collectionRepository: ManifestRepository = {
    type: 'repository',
    alias: WORKSPACE_REPOSITORY_ALIAS,
    name: 'Product Feed Collection Repository',
    api: () => import('./repository.js'),
};

const collectionActions: Array<ManifestTypes> = [
    {
        type: 'collectionAction',
        alias: 'UcProductFeeds.EntityAction.Create',
        name: 'Product Feeds Collection Action - Create',
        kind: 'button',
        weight: 10,
        api: CollectionCreateAction,
        conditions: [
            {
                alias: UMB_COLLECTION_ALIAS_CONDITION,
                match: WORKSPACE_COLLECTION_ALIAS,
            },
        ],
        meta: {
            label: '#general_create',
        },
    },
];

export const manifests = [
    collectionManifest,
    collectionRepository,
    ...collectionActions,
];