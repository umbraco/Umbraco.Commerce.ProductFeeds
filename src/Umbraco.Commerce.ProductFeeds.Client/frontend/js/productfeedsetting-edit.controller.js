import { nanoid } from 'nanoid';
import { MODULE } from './constants';
import ucUtils from './utils';
import { getDocumentTypesAsync, getFeedSettingAsync, getFeedTypesAsync, saveSettingAsync, deleteAsync, getPropertyValueExtractorsAsync } from './apis';
import { editRoute, listRoute } from './fe-routes';

angular
    .module(MODULE.NAME)
    .controller('umbraco.commerce.productfeeds.settingEditController', [
        '$scope',
        '$routeParams',
        '$location',
        'formHelper',
        'appState',
        'notificationsService',
        'navigationService',
        'editorService',
        'entityResource',

        function (
            $scope,
            $routeParams,
            $location,
            formHelper,
            appState,
            notificationsService,
            navigationService,
            editorService,
            entityResource,
        ) {
            const vm = this;
            let [storeId, id] = ucUtils.parseCompositeId($routeParams.id);
            vm.isCreateMode = !id;
            vm.page = {
                initializing: true,
                notfound: false,
                saveButtonState: 'init',
                deleteButtonState: 'init',
                menu: {
                    currentSection: appState.getSectionState('currentSection'),
                    currentNode: null,
                },
                breadcrumb: {
                    items: [],
                    itemClick: function (ancestor) {
                        $location.path(ancestor.routePath);
                    },
                },
            };

            vm.options = {
                feedTypes: [],
                documentTypes: [],
                propertyValueExtractors: [],
            };

            vm.content = {};
            vm.preview = {
                productDocumentTypeAliasesVm: [],
            };

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
                    documentTypes,
                    propertyValueExtractors,
                ] = await Promise.all([
                    getFeedTypesAsync(),
                    getDocumentTypesAsync(),
                    getPropertyValueExtractorsAsync(),
                ]);

                vm.options.feedTypes = feedTypes;
                vm.options.documentTypes = documentTypes;
                vm.options.propertyValueExtractors = [
                    {
                        label: 'Select a property value extractor',
                        value: '',
                        disabled: true,
                    },
                    ...propertyValueExtractors.map(x => ({
                        ...x,
                        disabled: false,
                    }))];

                if (vm.isCreateMode) {
                    vm.ready({
                        id: null,
                        storeId,
                        // generate sample mapping
                        propertyNameMappings: [
                            { uiId: nanoid(), 'nodeName': 'g:id', 'propertyAlias': 'sku', valueExtractorName: 'DefaultSingleValuePropertyExtractor' },
                            { uiId: nanoid(), 'nodeName': 'g:title', 'propertyAlias': 'Name', valueExtractorName: 'DefaultSingleValuePropertyExtractor' },
                            { uiId: nanoid(), 'nodeName': 'g:availability', 'propertyAlias': 'stock', 'valueExtractorName': 'DefaultGoogleAvailabilityValueExtractor' },
                            { uiId: nanoid(), 'nodeName': 'g:image_link', 'propertyAlias': 'image', 'valueExtractorName': 'DefaultMediaPickerPropertyValueExtractor' },
                            { uiId: nanoid(), 'nodeName': 'g:image_link', 'propertyAlias': 'images', 'valueExtractorName': 'DefaultMultipleMediaPickerPropertyValueExtractor' },
                        ],
                    });
                } else {
                    getFeedSettingAsync(id)
                        .then((feedSetting) => {
                            vm.ready({
                                ...feedSetting,
                                name: feedSetting.feedName,
                            });
                        }, () => {
                            vm.ready(null);
                        });
                }
                $scope.$apply();
            };

            vm.ready = function (model) {
                if (!model) {
                    vm.page.notfound = true;
                    vm.page.initializing = false;
                    return;
                }

                vm.content = model;
                vm.propertyAndNodeMappingVm = model.propertyNameMappings.map(x => ({
                    ...x,
                    uiId: nanoid(),
                    message: '',
                }));

                if (!vm.isCreateMode) {
                    vm.preview.productDocumentTypeAliasesVm = vm.options.documentTypes.filter(x => model.productDocumentTypeAliases.includes(x.alias));

                    // load previews
                    entityResource.getById(model.productRootKey, 'Document')
                        .then(function (entity) {
                            vm.preview['productRootKey'] = entity;
                        });

                    vm.preview.productChildVariantTypeAlias = vm.options.documentTypes.find(x => x.alias === model.productChildVariantTypeAlias);
                }

                vm.page.initializing = false;

                // sync state

                let pathToSync = ['-1', '1', storeId, '7428'];
                navigationService.syncTree({ tree: 'commercesettings', path: pathToSync, forceReload: true }).then(function (syncArgs) {
                    if (!vm.isCreateMode) {
                        vm.page.menu.currentNode = {
                            content: model,
                            storeId,
                            id,
                            menuUrl: '/umbraco/backoffice/umbracocommerceproductfeeds/productfeedstreenode/getmenu',
                        };
                    }
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
                        propertyNameMappings: vm.propertyAndNodeMappingVm,
                        productDocumentTypeAliases: vm.preview.productDocumentTypeAliasesVm.map(x => x.alias).join(';'),
                    })
                        .then((savedId) => {
                            $scope.$apply(() => {
                                formHelper.resetForm({
                                    scope: $scope,
                                });

                                vm.page.saveButtonState = 'success';
                                notificationsService.success('Feed setting saved', `Feed setting \'${vm.content.name}\' successfully saved.`);

                                if (vm.isCreateMode) {
                                    $location.path(editRoute(storeId, savedId));
                                }
                            });
                        }, handleApiError(`Failed to save setting '${vm.content.name}'`));
                } else {
                    vm.page.saveButtonState = 'error';
                }
            };

            vm.onAddPropertyMappingRowClick = () => {
                vm.propertyAndNodeMappingVm.push({
                    uiId: nanoid(),
                    propertyAlias: '',
                    nodeName: '',
                    valueExtractorName: 'DefaultSingleValuePropertyExtractor',
                    message: 'Please fill in all the textbox',
                });
            };

            vm.onDeletePropertyMappingRowClick = (uiId) => {
                vm.propertyAndNodeMappingVm = vm.propertyAndNodeMappingVm.filter(x => x.uiId != uiId);
            };

            vm.onOpenContentPicker = (targetField) => {
                editorService.contentPicker({
                    multiPicker: false,
                    submit: (currentService) => {
                        vm.content[targetField] = currentService.selection[0].key;
                        vm.preview[targetField] = currentService.selection[0];
                        editorService.close();
                    },
                    close: () => { editorService.close(); },
                });
            };

            vm.onOpenContentTypePicker = (targetField, multiPicker = false) => {
                editorService.contentTypePicker({
                    multiPicker,
                    submit: (currentService) => {
                        vm.content[targetField] = currentService.selection[0].key;
                        if (!multiPicker) {
                            const selected = vm.options.documentTypes.find(x => x.id === currentService.selection[0].id);
                            vm.preview[targetField] = selected;
                            vm.content[targetField] = selected.alias;
                        }
                        else {
                            const selectedIds = currentService.selection.map(x => x.id);
                            vm.preview[targetField] = [
                                ...vm.preview[targetField],
                                ...vm.options.documentTypes.filter(x => selectedIds.includes(x.id)),
                            ];
                        }

                        editorService.close();
                    },
                    close: () => { editorService.close(); },
                });
            };

            vm.onClearFieldClick = (fieldName) => {
                vm.content[fieldName] = '';
                vm.preview[fieldName] = null;
            };

            vm.onRemoveProductDocumentType = (docTypeId) => {
                vm.preview.productDocumentTypeAliasesVm = vm.preview.productDocumentTypeAliasesVm.filter(x => x.id !== docTypeId);
            };

            vm.onOpenFeedClick = () => {
                window.open(`/umbraco/commerce/productfeed/${vm.content.feedRelativePath}`, '_blank');
            };

            // initialization call
            vm.initAsync();
        }]);
