import { UcManifestStoreMenuItem } from '@umbraco-commerce/backoffice';
import { listingWorkspaceManifest } from './workspaces/list/manifests';

export const storeMenuManifests: UcManifestStoreMenuItem = {
    type: 'ucStoreMenuItem',
    alias: 'product-feeds',
    name: 'Product Feeds',
    meta: {
        label: '#ucProductFeeds_sectionMenuLabel',
        menus: ['Uc.Menu.StoreSettings'],
        entityType: listingWorkspaceManifest.meta.entityType,
        icon: 'icon-rss',
    },
    weight: 9999,
};