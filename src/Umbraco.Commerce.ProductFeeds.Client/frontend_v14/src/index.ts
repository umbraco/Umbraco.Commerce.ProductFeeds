import type { UmbEntryPointOnInit } from '@umbraco-cms/backoffice/extension-api';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import { manifests as workspaceManifests } from './workspace/manifests.ts';
import { manifests as localizationManifests } from './lang/manifests.ts';
import { WORKSPACE_ENTITY_ALIAS } from './constants.js';
import { client as apiClient } from './generated/apis/services.gen';

const storeMenuManifests = {
    type: 'ucStoreMenuItem',
    alias: 'product-feeds',
    name: 'Product Feeds',
    meta: {
        label: '#ucProductFeeds_sectionMenuLabel',
        menus: ['Uc.Menu.StoreSettings'],
        entityType: WORKSPACE_ENTITY_ALIAS,
        icon: 'icon-rss',
    },
    weight: 100,
};

const manifests = [
    storeMenuManifests,
    ...localizationManifests,
    ...workspaceManifests,
];

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    console.log('UC Product Feeds v14 loaded.');

    extensionRegistry.registerMany(manifests);
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
