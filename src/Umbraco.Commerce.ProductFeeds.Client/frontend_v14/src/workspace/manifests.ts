import type { ManifestWorkspace, ManifestWorkspaceAction, ManifestWorkspaceView } from '@umbraco-cms/backoffice/extension-registry';
import { WORKSPACE_ALIAS, WORKSPACE_ENTITY_ALIAS } from '../constants.js';
import { manifests as collectionManifests } from './collection/manifests.js';
import { UcProductFeedsWorkspaceContext } from './context.js';

const workspace: ManifestWorkspace = {
	type: 'workspace',
	kind: 'routable',
	alias: WORKSPACE_ALIAS,
	name: 'UC Product Feeds Workspace',
	api: UcProductFeedsWorkspaceContext,
	meta: {
		entityType: WORKSPACE_ENTITY_ALIAS,
	},
};

// const store: ManifestStore = {
// 	type: 'store',
// 	alias: WORKSPACE_STORE_ALIAS,
// 	name: 'Product Feed Store',
// 	api: () => import('../workspace/store.js'),
// };


const workspaceViews: Array<ManifestWorkspaceView> = [];
const workspaceActions: Array<ManifestWorkspaceAction> = [];

export const manifests = [
	workspace,
	// store,
	...workspaceViews,
	...workspaceActions,
	...collectionManifests];
