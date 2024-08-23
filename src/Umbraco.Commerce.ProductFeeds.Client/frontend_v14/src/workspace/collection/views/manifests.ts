import { UMB_COLLECTION_ALIAS_CONDITION } from '@umbraco-cms/backoffice/collection';
import type { ManifestCollectionView } from '@umbraco-cms/backoffice/extension-registry';

export const WORKFLOW_APPROVALGROUP_TABLE_COLLECTION_VIEW_ALIAS =
    'Workflow.CollectionView.ApprovalGroup.Table';

const tableCollectionView: ManifestCollectionView = {
    type: 'collectionView',
    alias: WORKFLOW_APPROVALGROUP_TABLE_COLLECTION_VIEW_ALIAS,
    name: 'Product Feeds Collection View',
    js: () => import('./table.element'),
    meta: {
        label: 'Table',
        icon: 'icon-list',
        pathName: 'table',
    },
    conditions: [
        {
            alias: UMB_COLLECTION_ALIAS_CONDITION,
            match: 'Workflow.Collection.ApprovalGroup',
        },
    ],
};

export const manifests = [tableCollectionView];