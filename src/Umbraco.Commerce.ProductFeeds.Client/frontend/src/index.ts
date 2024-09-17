import type { UmbEntryPointOnInit } from '@umbraco-cms/backoffice/extension-api';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import { manifests as workspacesManifests } from './workspaces/manifests.ts';
import { manifests as localizationManifests } from './lang/manifests.ts';
import { client as apiClient } from './generated/apis/services.gen';
import { listingWorkspaceManifest } from './workspaces/list/manifests.ts';
import { UcManifestStoreMenuItem } from '@umbraco-commerce/backoffice';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';

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
    weight: -1,
};

const allManifests = [
    storeMenuManifests,
    ...localizationManifests,
    ...workspacesManifests,
];

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    extensionRegistry.registerMany(allManifests);
    host.consumeContext(UMB_AUTH_CONTEXT, async (instance) => {
        if (!instance) return;

        apiClient.instance.interceptors.request.use(async (request) => {
            const token = await instance.getLatestToken();
            request.withCredentials = true;
            request.headers.set('Authorization', 'Bearer ' + token);
            return request;
        });
    });

    host.consumeContext(UMB_NOTIFICATION_CONTEXT, notificationContext => {
        apiClient.instance.interceptors.response.use(response => {
            switch (response.status) {
                case 500:
                    notificationContext?.peek('danger', {
                        data: {
                            headline: 'Server Error',
                            message: 'A fatal server error occurred. If this continues, please reach out to the package administrator.',
                        },
                    });
                    break;
            }

            return response;
        });
    });
};
