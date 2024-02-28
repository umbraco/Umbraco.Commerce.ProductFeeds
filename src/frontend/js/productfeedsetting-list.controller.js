import { getFeedSetingsAsync } from './apis';
import { MODULE } from './constants';
import { createRoute } from './fe-routes';
import ucUtils from './utils';

angular
    .module(MODULE.NAME)
    .controller('umbraco.commerce.productfeeds.settingListController', [
        '$location',
        '$routeParams',
        'appState',
        'navigationService',

        function (
            $location,
            $routeParams,
            appState,
            navigationService,
        ) {
            const compositeId = ucUtils.parseCompositeId($routeParams.id);
            const storeId = compositeId[0];
            const vm = this;

            vm.page = {};
            vm.page.loading = true;

            vm.page.menu = {};
            vm.page.menu.currentSection = appState.getSectionState('currentSection');
            vm.page.menu.currentNode = null;

            vm.page.breadcrumb = {};
            vm.page.breadcrumb.items = [];
            vm.page.breadcrumb.itemClick = function (ancestor) {
                $location.path(ancestor.routePath);
            };

            vm.options = {
                createActions: [
                    {
                        name: 'Create Product Feed',
                        doAction: function () {
                            $location.path(createRoute(storeId));
                        },
                    }],
                bulkActions: [],
                items: [],
                itemProperties: [
                    { alias: 'feedTypeName', header: 'Type' },
                    { alias: 'feedRelativePath', header: 'Path' },
                ],
                itemClick: function (item) {
                    $location.path(item.routePath);
                },
            };

            vm.loadSettingsAsync = async function () {
                const entities = await getFeedSetingsAsync(storeId);
                console.log(entities);
                vm.options.items = entities;
            };

            /**
             * Actions to initialize this controller.
             */
            vm.init = async function () {
                console.log($routeParams);

                navigationService
                    .syncTree({ tree: 'commercesettings', path: ['-1', '1', storeId, '7428'], forceReload: true })
                    .then(function (syncArgs) {
                        vm.page.menu.currentNode = syncArgs.node;
                        vm.page.breadcrumb.items = ucUtils.createSettingsBreadcrumbFromTreeNode(syncArgs.node);
                        vm.page.loading = false;
                    });

                await vm.loadSettingsAsync();
                vm.page.loading = false;
            };

            // initialization call
            vm.init();
        }]);