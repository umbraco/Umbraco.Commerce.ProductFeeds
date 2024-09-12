import type { UmbEntryPointOnInit } from '@umbraco-cms/backoffice/extension-api';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import { manifests as workspacesManifests } from './workspaces/manifests.ts';
import { manifests as localizationManifests } from './lang/manifests.ts';
import { client as apiClient } from './generated/apis/services.gen';
import { listingWorkspaceManifest } from './workspaces/list/manifests.ts';
import { UcManifestStoreMenuItem } from '@umbraco-commerce/backoffice';

export * from './workspaces/details/components/ucpf-property-node-mapper.ts';

const storeMenuManifests: UcManifestStoreMenuItem = {
    type: 'ucStoreMenuItem',
    alias: 'product-feeds',
    name: 'Product Feeds',
    meta: {
        label: '#ucProductFeeds_sectionMenuLabel',
        menus: ['Uc.Menu.StoreSettings'],
        entityType: listingWorkspaceManifest.meta.entityType,
        icon: 'icon-rss',
    },
    weight: 100,
};

const allManifests = [
    storeMenuManifests,
    ...localizationManifests,
    ...workspacesManifests,
];

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    console.log('UC Product Feeds v14 loaded.');

    extensionRegistry.registerMany(allManifests);
    host.consumeContext(UMB_AUTH_CONTEXT, async (instance) => {
        if (!instance) return;
        const umbOpenApi = instance.getOpenApiConfiguration();

        apiClient.setConfig({
            credentials: 'same-origin',
            headers: {
                'Authorization': 'Bearer ' + await umbOpenApi.token(),
            },
        });
    });
};
