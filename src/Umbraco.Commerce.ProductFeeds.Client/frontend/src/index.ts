import type { UmbEntryPointOnInit } from '@umbraco-cms/backoffice/extension-api';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import { manifests as workspacesManifests } from './workspaces/manifests.ts';
import { manifests as localizationManifests } from './lang/manifests.ts';
import { client as apiClient } from './generated/apis/client.gen.ts';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { storeMenuManifests } from './uc-menu-manifest.ts';

export * from './workspaces/details/components/ucpf-property-node-mapper.ts';

const allManifests = [
    storeMenuManifests,
    ...localizationManifests,
    ...workspacesManifests,
];

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
    extensionRegistry.registerMany(allManifests);
    host.consumeContext(UMB_AUTH_CONTEXT, (authContext) => {
        authContext?.configureClient(apiClient);
    });

    host.consumeContext(UMB_NOTIFICATION_CONTEXT, notificationContext => {
        apiClient.interceptors.response.use(response => {
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
