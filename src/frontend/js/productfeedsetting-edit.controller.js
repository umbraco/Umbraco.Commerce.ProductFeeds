import { MODULE } from './constants';
import ucUtils from './utils';
import { getFeedSettingAsync } from './apis';
import { listRoute } from './fe-routes';

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
        'treeService',
        'ucCountryResource',
        'ucCurrencyResource',
        'ucShippingMethodResource',
        'ucPaymentMethodResource',
        'ucActions',

        function (
            $scope,
            $routeParams,
            $location,
            formHelper,
            appState,
            editorState,
            notificationsService,
            navigationService,
            treeService,
            ucCountryResource,
            ucCurrencyResource,
            ucShippingMethodResource,
            ucPaymentMethodResource,
            ucActions) {
            let compositeId = ucUtils.parseCompositeId($routeParams.id);
            let storeId = compositeId[0];

            let id = compositeId[1];
            const create = id === '-1';

            let vm = this;

            vm.page = {};
            vm.page.loading = true;
            vm.page.saveButtonState = 'init';

            vm.page.menu = {};
            vm.page.menu.currentSection = appState.getSectionState('currentSection');
            vm.page.menu.currentNode = null;

            vm.page.breadcrumb = {};
            vm.page.breadcrumb.items = [];
            vm.page.breadcrumb.itemClick = function (ancestor) {
                $location.path(ancestor.routePath);
            };

            vm.options = {
                currencies: [],
                shippingMethods: [],
                paymentMethods: [],
                editorActions: [],
            };
            vm.content = {};

            vm.back = function () {
                $location.path(listRoute(storeId)).search({});
            };

            vm.initAsync = async function () {

                // ucActions.getEditorActions({ storeId: storeId, entityType: 'Country' }).then(result => {
                //     vm.options.editorActions = result;
                // });

                // ucCurrencyResource.getCurrencies(storeId).then(function (currencies) {
                //     vm.options.currencies = currencies;
                // });

                // ucShippingMethodResource.getShippingMethods(storeId).then(function (shippingMethods) {
                //     vm.options.shippingMethods = shippingMethods;
                // });

                // ucPaymentMethodResource.getPaymentMethods(storeId).then(function (paymentMethods) {
                //     vm.options.paymentMethods = paymentMethods;
                // });

                // if (create) {

                //     ucCountryResource.createCountry(storeId).then(function (country) {
                //         vm.ready(country);
                //     });

                // } else {

                const feedSetting = await getFeedSettingAsync(id);
                vm.ready(feedSetting);

                // }

                // $scope.$on('Umbraco.Commerce.Entity.Deleted', function (evt, args) {
                //     if (args.entityType === 'Country' && args.storeId === storeId && args.entityId === id) {
                //         vm.back();
                //     }
                // });
            };

            vm.ready = function (model) {
                vm.page.loading = false;
                vm.content = model;

                if (create && $routeParams['preset'] === 'true') {
                    vm.content.name = $routeParams['name'];
                    vm.content.code = $routeParams['code'];
                    vm.content.presetIsoCode = $routeParams['code'];
                }

                // sync state
                editorState.set(vm.content);

                let pathToSync = ['-1', '1', storeId, '7428'];
                navigationService.syncTree({ tree: 'commercesettings', path: pathToSync, forceReload: true }).then(function (syncArgs) {
                    if (!create) {
                        treeService.getChildren({ node: syncArgs.node }).then(function (children) {
                            let node = children.find(function (itm) {
                                return itm.id === id;
                            });
                            vm.page.menu.currentNode = node;
                            vm.page.breadcrumb.items = ucUtils.createSettingsBreadcrumbFromTreeNode(node);
                        });
                    } else {
                        vm.page.breadcrumb.items = ucUtils.createSettingsBreadcrumbFromTreeNode(syncArgs.node);
                        vm.page.breadcrumb.items.push({ name: 'Untitled' });
                    }
                });
            };

            vm.saveAsync = async function (suppressNotification) {

                if (formHelper.submitForm({ scope: $scope, statusMessage: 'Saving...' })) {
                    vm.page.saveButtonState = 'busy';
                    ucCountryResource.saveCountry(vm.content).then(function (saved) {

                        formHelper.resetForm({ scope: $scope, notifications: saved.notifications });

                        vm.page.saveButtonState = 'success';

                        if (create) {
                            $location.path('/settings/commercesettings/country-edit/' + ucUtils.createCompositeId([storeId, saved.id]));
                        }
                        else {
                            vm.ready(saved);
                        }

                    }, function (err) {

                        if (!suppressNotification) {
                            vm.page.saveButtonState = 'error';
                            notificationsService.error('Failed to save country ' + vm.content.name,
                                err.data.message || err.data.Message || err.errorMsg);
                        }

                        vm.page.saveButtonState = 'error';
                    });
                }

            };

            // initialization call
            vm.initAsync();
        }]);