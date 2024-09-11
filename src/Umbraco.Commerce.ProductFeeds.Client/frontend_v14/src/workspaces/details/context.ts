import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import { UmbSubmittableWorkspaceContext, UmbSubmittableWorkspaceContextBase, UmbWorkspaceRouteManager } from '@umbraco-cms/backoffice/workspace';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbBasicState, UmbObjectState } from '@umbraco-cms/backoffice/observable-api';
import { UcpfWorkspaceEditorElement as UcpfWorkspaceElement } from './index.element';
import { detailsWorkspaceManifest } from './manifests';
import { ProductFeedSettingWriteModel } from '../../generated/apis';
import { UcpfReadWriteRepository } from './repository';
import { FeProductFeedSettingWriteModel } from './types';
import { nanoid } from 'nanoid';

export const DETAILS_WORKSPACE_CONTEXT = new UmbContextToken<UmbSubmittableWorkspaceContext, UcpfDetailsWorkspaceContext>(
    'UmbWorkspaceContext',
    undefined,
    (context): context is UcpfDetailsWorkspaceContext => context.getEntityType() === detailsWorkspaceManifest.meta.entityType,
);

export class UcpfDetailsWorkspaceContext
    extends UmbSubmittableWorkspaceContextBase<ProductFeedSettingWriteModel>
    implements UmbSubmittableWorkspaceContext {
    protected async submit(): Promise<void> {
        console.log('model', this.model);
    }

    #repository: UcpfReadWriteRepository = new UcpfReadWriteRepository(this);

    #unique = new UmbBasicState<string | undefined>(undefined);
    readonly unique = this.#unique.asObservable();

    #model = new UmbObjectState<FeProductFeedSettingWriteModel | undefined>(undefined);
    readonly model = this.#model.asObservable();
    readonly routes = new UmbWorkspaceRouteManager(this);

    #feedTypes = new UmbObjectState<Option[]>([]);
    setFeedTypes(data: Option[]) {
        this.#feedTypes.setValue(data);
    }

    readonly feedTypes = this.#feedTypes.asObservable();

    constructor(host: UmbControllerHost) {
        super(host, detailsWorkspaceManifest.alias);
        this.routes.setRoutes([
            // {
            // path: 'create',
            //component: UcEmailTemplateWorkspaceEditorElement,
            // setup: async (_component, info) => {
            //     this.removeUmbControllerByAlias(UcWorkspaceStoreEntityDeletedRedirectController.ALIAS);
            //     await this.scaffold();
            //     new UcWorkspaceIsNewStoreEntityRedirectController(
            //         this, this,
            //         this.getHostElement().shadowRoot!.querySelector('umb-router-slot')!,
            //     );
            // },
            // },
            {
                path: ':unique',
                component: UcpfWorkspaceElement,
                setup: async (_component, info) => {
                    // this.removeUmbControllerByAlias(UcWorkspaceIsNewStoreEntityRedirectController.ALIAS);
                    console.log(info.match);
                    await Promise.all([
                        this.loadFeedSettingsDetailsAsync(info.match.params.unique),
                        this.loadFeedTypeAsync(),
                    ]);
                    // new UcWorkspaceStoreEntityDeletedRedirectController(
                    //     this, this,
                    //     this.getHostElement().shadowRoot!.querySelector('umb-router-slot')!,
                    // );
                },
            },
        ]);
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
        this.#unique.setValue(undefined);
        this.#model.setValue(undefined);
    }

    async loadFeedSettingsDetailsAsync(id: string) {
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

    async loadFeedTypeAsync() {
        const { data } = await this.#repository.fetchFeedTypesAsync();
        if (data) {
            this.#feedTypes.setValue(data.map(x => ({
                value: x.value,
                name: x.label,
            })));
        }
    }

    setModel(data: FeProductFeedSettingWriteModel) {
        this.#model.setValue(data);
    }
}
