
import { detailsWorkspaceManifest } from './details/manifests';
import { listingWorkspaceManifest } from './list/manifests';

const baseUrl = 'section/settings/workspace/uc:store-settings';

export const editRoute = (storeId: string, itemId: string) => `${baseUrl}/${storeId}/${detailsWorkspaceManifest.meta.entityType}/${itemId}`;

export const createRoute = (storeId: string) => editRoute(storeId, 'create');

export const listRoute = (storeId: string) => `${baseUrl}/${storeId}/${listingWorkspaceManifest.meta.entityType}`;

export const storeRoute = (storeId: string) => `${baseUrl}/${storeId}`;

export const viewFeedRoute = (relativePath: string) => `/umbraco/commerce/productfeed/${relativePath}`;