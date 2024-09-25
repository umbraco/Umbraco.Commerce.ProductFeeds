import { manifests as listingWorkspaceManifests } from './list/manifests';
import { manifests as detailsWorkspaceManifests } from './details/manifests';

export const manifests = [
	...listingWorkspaceManifests,
	...detailsWorkspaceManifests,
];
