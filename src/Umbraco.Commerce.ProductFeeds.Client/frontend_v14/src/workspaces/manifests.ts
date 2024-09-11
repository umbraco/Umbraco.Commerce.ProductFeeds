import { manifests as listingWorkspaceManifests } from './list/manifests';
import { manifests as detailsWorkspaceManifests } from './details/manifests';


// const store: ManifestStore = {
// 	type: 'store',
// 	alias: WORKSPACE_STORE_ALIAS,
// 	name: 'Product Feed Store',
// 	api: () => import('../workspace/store.js'),
// };

export const manifests = [
	...listingWorkspaceManifests,
	...detailsWorkspaceManifests,
];
