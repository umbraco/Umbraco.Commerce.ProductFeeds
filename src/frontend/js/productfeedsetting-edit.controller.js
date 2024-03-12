import { nanoid } from 'nanoid';
import { MODULE } from './constants';
import ucUtils from './utils';
import { getDocumentTypesAsync, getFeedSettingAsync, getFeedTypesAsync, getCustomPropertyAliasesAsync, saveSettingAsync, deleteAsync, getPropertyValueExtractorsAsync } from './apis';
import { editRoute, listRoute } from './fe-routes';

angular
    .module(MODULE.NAME)
    .controller('umbraco.commerce.productfeeds.settingEditController', [
        '$scope',
        '$routeParams',
        '$location',
        'formHelper',
        'appState',
        'editorState',
        'notificationsService',
        'navigationService',
        'overlayService',

        function (
            $scope,
            $routeParams,
            $location,
            formHelper,
            appState,
            editorState,
            notificationsService,
            navigationService,
            overlayService,
        ) {
            const vm = this;
            let [storeId, id] = ucUtils.parseCompositeId($routeParams.id);
            vm.isCreateMode = !id;
            vm.page = {};
            vm.page.loading = true;
            vm.page.saveButtonState = 'init';
            vm.page.deleteButtonState = 'init';

            vm.page.menu = {};
            vm.page.menu.currentSection = appState.getSectionState('currentSection');
            vm.page.menu.currentNode = null;

            vm.page.breadcrumb = {};
            vm.page.breadcrumb.items = [];
            vm.page.breadcrumb.itemClick = function (ancestor) {
                $location.path(ancestor.routePath);
            };

            vm.overlay = {
                title: '',
                subtitle: '',
                editModel: '',
                show: false,
                submitButtonLabel: 'SUBMIT BUTTON TEXT',
                closeButtonLabel: 'CLOSE BUTTON TEXT',
                submit: function (model) {
                    vm.overlay.show = false;
                    vm.overlay = null;
                },
                close: function (oldModel) {
                    vm.overlay.show = false;
                    vm.overlay = null;
                },
            };

            vm.options = {
                feedTypes: [],
                documentTypeAliases: [],
                propertyAliases: [],
                propertyAliasesLoading: true,
                propertyValueExtractors: [],
            };

            vm.content = {};

            vm.propertyAndNodeMappingVm = [];

            vm.back = function () {
                $location.path(listRoute(storeId)).search({});
            };

            function handleApiError(message) {
                return (err) => {
                    vm.page.saveButtonState = 'error';
                    vm.page.deleteButtonState = 'error';
                    notificationsService.error(message, JSON.stringify(err.response.data));
                };
            }

            vm.initAsync = async function () {
                const [
                    feedTypes,
                    documentTypeAliases,
                    propertyAliases,
                    propertyValueExtractors,
                ] = await Promise.all([
                    getFeedTypesAsync(),
                    getDocumentTypesAsync(),
                    getCustomPropertyAliasesAsync(),
                    getPropertyValueExtractorsAsync(),
                ]);

                vm.options.feedTypes = feedTypes;
                vm.options.documentTypeAliases = documentTypeAliases;
                vm.options.propertyAliases = propertyAliases;
                vm.options.propertyValueExtractors = [
                    {
                        label: 'Select a property value extractor',
                        value: '',
                        disabled: true,
                    },
                    ...propertyValueExtractors.map(x => ({
                        label: x,
                        value: x,
                        disabled: false,
                    }))];

                console.log(vm.options.propertyValueExtractors);
                if (vm.isCreateMode) {
                    vm.ready({
                        id: null,
                        storeId,
                        // generate sample mapping
                        propertyNameMappings: [
                            { uiId: nanoid(), 'nodeName': 'g:id', 'propertyAlias': 'sku', valueExtractorName: 'DefaultSingleValuePropertyExtractor' },
                            { uiId: nanoid(), 'nodeName': 'g:title', 'propertyAlias': 'Name', valueExtractorName: 'DefaultSingleValuePropertyExtractor' },
                            { uiId: nanoid(), 'nodeName': 'g:description', 'propertyAlias': 'longDescription', valueExtractorName: 'DefaultSingleValuePropertyExtractor' },
                            { uiId: nanoid(), 'nodeName': 'g:availability', 'propertyAlias': 'stock', 'valueExtractorName': 'DefaultGoogleAvailabilityValueExtractor' },
                            { uiId: nanoid(), 'nodeName': 'g:image_link', 'propertyAlias': 'image', 'valueExtractorName': 'DefaultMediaPickerPropertyValueExtractor' },
                        ],
                    });
                } else {

                    const feedSetting = await getFeedSettingAsync(id);
                    vm.ready({
                        ...feedSetting,
                        name: feedSetting.feedName,
                        productDocumentTypeAliasVm: feedSetting.productDocumentTypeAlias.split(';'),
                        propertyNameMappingsString: JSON.stringify(feedSetting.propertyNameMappings),
                    });
                }
            };

            vm.ready = function (model) {
                console.log(model);
                vm.content = model;
                vm.propertyAndNodeMappingVm = model.propertyNameMappings.map(x => ({
                    ...x,
                    uiId: nanoid(),
                    message: '',
                }));
                vm.onProductDocumentTypeAliasChangeAsync();
                vm.page.loading = false;

                // sync state
                editorState.set(vm.content);

                let pathToSync = ['-1', '1', storeId, '7428'];
                navigationService.syncTree({ tree: 'commercesettings', path: pathToSync, forceReload: true }).then(function (syncArgs) {
                    vm.page.breadcrumb.items = ucUtils.createSettingsBreadcrumbFromTreeNode(syncArgs.node);
                    vm.page.breadcrumb.items.push({ name: vm.content?.name || 'Untitled' });
                });
            };

            vm.saveAsync = async function () {
                if (formHelper.submitForm({ scope: $scope, statusMessage: 'Saving...' })) {
                    vm.page.saveButtonState = 'busy';

                    saveSettingAsync({
                        ...vm.content,
                        feedName: vm.content.name,
                        // propertyNameMappings: JSON.parse(vm.content.propertyNameMappingsString),
                        propertyNameMappings: vm.propertyAndNodeMappingVm,
                    }).then((savedId) => {
                        vm.page.saveButtonState = 'success';
                        formHelper.resetForm({
                            scope: $scope,
                        });

                        notificationsService.success('Feed setting saved', `Feed setting \'${vm.content.name}\' successfully saved.`);
                        if (vm.isCreateMode) {
                            $location.path(editRoute(storeId, savedId));
                        }
                    }, handleApiError(`Failed to save setting '${vm.content.name}'`));
                };
            };

            vm.onProductDocumentTypeAliasChangeAsync = async () => {
                if (!vm.content.productDocumentTypeAliasVm || !vm.content.productDocumentTypeAliasVm.length) {
                    return;
                }

                vm.options.propertyAliasesLoading = true;
                vm.content.productDocumentTypeAlias = vm.content.productDocumentTypeAliasVm.join(';');
                const data = await getCustomPropertyAliasesAsync(vm.content.productDocumentTypeAlias);
                vm.options.propertyAliases = data;
                vm.options.propertyAliasesLoading = false;
            };

            vm.onDeleteButtonClickAsync = async () => {
                let overlay = {
                    title: 'Caution',
                    content: `Are you sure you want to delete '${vm.content.name}'?`,
                    disableBackdropClick: false,
                    disableEscKey: false,
                    submit: function () {
                        overlayService.close();
                        vm.page.deleteButtonState = 'busy';
                        vm.page.saveButtonState = 'busy';
                        deleteAsync(vm.content.id)
                            .then(success => {
                                if (success) {
                                    notificationsService.success('Deleted successfully.');
                                    vm.back();
                                }
                            }, handleApiError('Failed to delete record.'))
                            .finally(() => {
                                vm.page.deleteButtonState = 'init';
                                vm.page.saveButtonState = 'init';
                            });
                    },
                };

                overlayService.confirmDelete(overlay);
            };

            vm.onAddPropertyMappingRowClick = () => {
                vm.propertyAndNodeMappingVm.push({
                    uiId: nanoid(),
                    propertyAlias: '',
                    nodeName: '',
                    valueExtractorName: '',
                    message: 'Please fill in all the textbox',
                });
            };

            vm.onDeletePropertyMappingRowClick = (uiId) => {
                console.log('Delete ' + uiId);
                vm.propertyAndNodeMappingVm = vm.propertyAndNodeMappingVm.filter(x => x.uiId != uiId);
            };

            // initialization call
            vm.initAsync();
        }]);
