import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import { UmbRoutableWorkspaceContext, UmbSubmittableWorkspaceContext, UmbSubmittableWorkspaceContextBase, UmbWorkspaceIsNewRedirectController, UmbWorkspaceRouteManager } from '@umbraco-cms/backoffice/workspace';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbBasicState, UmbObjectState } from '@umbraco-cms/backoffice/observable-api';
import { UcpfWorkspaceEditorElement as UcpfWorkspaceElement } from './index.element';
import { detailsWorkspaceManifest } from './manifests';
import { UcpfReadWriteRepository } from './repository';
import { FeProductFeedSettingWriteModel } from './types';
import { nanoid } from 'nanoid';
import { UMB_NOTIFICATION_CONTEXT, UmbNotificationContext } from '@umbraco-cms/backoffice/notification';
import { UmbLocalizationController } from '@umbraco-cms/backoffice/localization-api';
import { UC_STORE_CONTEXT, UcStoreModel } from '@umbraco-commerce/backoffice';
import { listRoute } from '../routes';

export const DETAILS_WORKSPACE_CONTEXT = new UmbContextToken<UmbSubmittableWorkspaceContext, UcpfDetailsWorkspaceContext>(
    'UmbWorkspaceContext',
    undefined,
    (context): context is UcpfDetailsWorkspaceContext => context.getEntityType() === detailsWorkspaceManifest.meta.entityType,
);

export class UcpfDetailsWorkspaceContext
    extends UmbSubmittableWorkspaceContextBase<FeProductFeedSettingWriteModel>
    implements UmbSubmittableWorkspaceContext, UmbRoutableWorkspaceContext {
    #repository: UcpfReadWriteRepository = new UcpfReadWriteRepository(this);

    #unique = new UmbBasicState<string | undefined>(undefined);
    readonly unique = this.#unique.asObservable();

    #model = new UmbObjectState<FeProductFeedSettingWriteModel | undefined>(undefined);
    readonly model = this.#model.asObservable();
    readonly routes = new UmbWorkspaceRouteManager(this);

    #feedTypes = new UmbObjectState<Option[]>([]);
    readonly feedTypeOptions = this.#feedTypes.asObservable();

    #propertyValueExtractorOptions = new UmbObjectState<Option[]>([]);
    readonly propertyValueExtractorOptions = this.#propertyValueExtractorOptions.asObservable();

    #notificationContext?: UmbNotificationContext;
    #localize?: UmbLocalizationController;

    #store?: UcStoreModel;

    constructor(host: UmbControllerHost) {
        super(host, detailsWorkspaceManifest.alias);
        this.routes.setRoutes([
            {
                path: 'create',
                component: UcpfWorkspaceElement,
                setup: async () => {
                    await Promise.all([
                        this.scaffold(),
                        this.fetchFeedTypesAsync(),
                        this.fetchPropertyValueExtractorsAsync(),
                    ]);
                    new UmbWorkspaceIsNewRedirectController(
                        this,
                        this,
                        this.getHostElement().shadowRoot!.querySelector('umb-router-slot')!,
                    );
                },
            },
            {
                path: 'edit/:unique',
                component: UcpfWorkspaceElement,
                setup: async (_component, info) => {
                    await Promise.all([
                        this.fetchFeedSettingsDetailsAsync(info.match.params.unique),
                        this.fetchFeedTypesAsync(),
                        this.fetchPropertyValueExtractorsAsync(),
                    ]);
                },
            },
        ]);

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, ctx => {
            this.#notificationContext = ctx;
        });

        this.#localize = new UmbLocalizationController(host);
        this.consumeContext(UC_STORE_CONTEXT, ctx => {
            this.observe(ctx?.store, storeDto => {
                this.#store = storeDto;
            });
        });
    }

    getUnique(): string | undefined {
        return this.#unique.getValue();
    }

    getEntityType(): string {
        return detailsWorkspaceManifest.meta.entityType;
    }

    getData(): FeProductFeedSettingWriteModel | undefined {
        return this.#model.getValue();
    }

    protected resetState(): void {
        super.resetState();
        this.#unique.setValue('');
        this.#model.setValue({
            feedDescription: '',
            storeId: this.#store!.id,
            feedName: '',
            feedRelativePath: '',
            productChildVariantTypeIds: [],
            productDocumentTypeIds: [],
            includeTaxInPrice: true,
            propertyNameMappings: [
                {
                    uiId: nanoid(),
                    nodeName: 'g:id',
                    propertyAlias: 'sku',
                    valueExtractorId: 'DefaultSingleValuePropertyExtractor',
                },
                {
                    uiId: nanoid(),
                    nodeName: 'g:title',
                    propertyAlias: 'Name',
                    valueExtractorId: 'DefaultSingleValuePropertyExtractor',
                },
                {
                    uiId: nanoid(),
                    nodeName: 'g:availability',
                    propertyAlias: 'stock',
                    valueExtractorId: 'DefaultGoogleAvailabilityValueExtractor',
                },
                {
                    uiId: nanoid(),
                    nodeName: 'g:image_link',
                    propertyAlias: 'image',
                    valueExtractorId: 'DefaultMediaPickerPropertyValueExtractor',
                },
                {
                    uiId: nanoid(),
                    nodeName: 'g:image_link',
                    propertyAlias: 'images',
                    valueExtractorId: 'DefaultMultipleMediaPickerPropertyValueExtractor',
                },
            ],
        } as FeProductFeedSettingWriteModel);
    }

    async fetchFeedSettingsDetailsAsync(id: string) {
        this.resetState();
        const { data } = await this.#repository.fetchSingleAsync(id);
        if (data) {
            const uiModel: FeProductFeedSettingWriteModel = {
                ...data,
                propertyNameMappings: data.propertyNameMappings.map(x => ({
                    ...x,
                    uiId: nanoid(),
                })),
            };

            this.#unique.setValue(uiModel.id);
            this.#model.setValue(uiModel);
            this.setIsNew(false);
        }
    }

    async fetchFeedTypesAsync() {
        const { data } = await this.#repository.fetchFeedTypesAsync();
        if (data) {
            this.#feedTypes.setValue(data.map(x => ({
                value: x.value,
                name: x.label,
            })));
        }
    }

    async fetchPropertyValueExtractorsAsync() {
        const { data } = await this.#repository.fetchPropertyValueExtractorsAsync();
        if (data) {
            this.#propertyValueExtractorOptions.setValue(data.map(x => ({
                value: x.value,
                name: x.label,
            })));
        }
    }

    setModel(data: FeProductFeedSettingWriteModel) {
        this.#model.setValue(data);
    }

    async scaffold() {
        this.resetState();
        this.setIsNew(true);
    }

    protected async submit(): Promise<void> {
        if (!this.#model.value) {
            return;
        }

        const { id, error } = await this.#repository.saveAsync(this.#model.value);
        const validationErrors = error as Array<{ errorMessage: string }>;
        if (validationErrors && validationErrors.length) {
            this.#notificationContext?.peek('danger', {
                data: {
                    message: this.#localize?.term('ucProductFeeds_messageSaveFailed') ?? 'Save failed',
                    structuredList: {
                        Errors: validationErrors.map(x => x.errorMessage),
                    },
                },
            });
        } else if (error) {
            this.#notificationContext?.peek('danger', {
                data: {
                    headline: this.#localize?.term('ucProductFeeds_messageSaveFailed') ?? 'Save failed',
                    message: JSON.stringify(error),
                },
            });
        }
        else {
            // succeeded
            this.#notificationContext?.peek('positive', {
                data: {
                    headline: this.#localize?.term('ucProductFeeds_messageSaveSuccess') ?? 'Saved',
                    message: '',
                },
            });
            this.#unique.setValue(id);
            this.#model.setValue({
                ...this.#model.value,
                id,
            });

            this.setIsNew(false);
        }
    }

    async deleteAsync() {
        const entityId = this.#unique.getValue()!;
        const { isSuccess, error } = await this.#repository.deleteAsync([entityId]);
        if (isSuccess) {
            this.#notificationContext?.peek('positive', {
                data: {
                    headline: this.#localize?.term('ucProductFeeds_messageDeleteSuccess') ?? 'Delete successfully',
                    message: JSON.stringify(error),
                },
            });
            window.history.pushState({}, '', listRoute(this.#store!.id));
        } else {
            this.#notificationContext?.peek('danger', {
                data: {
                    headline: this.#localize?.term('ucProductFeeds_messageDeleteFailed') ?? 'Delete failed',
                    message: JSON.stringify(error),
                },
            });
        }
    }
}
