import { MODULE } from './constants';
import ucUtils from './utils';
import { deleteAsync } from './apis';
import { listRoute } from './fe-routes';

angular
    .module(MODULE.NAME)
    .controller('umbraco.commerce.productfeeds.deleteFeedSettingController', [
        '$scope',
        '$routeParams',
        '$location',
        'navigationService',
        'notificationsService',
        function (
            $scope,
            $routeParams,
            $location,
            navigationService,
            notificationsService,
        ) {
            const vm = this;
            let [storeId, id] = ucUtils.parseCompositeId($routeParams.id);

            vm.page = {};

            vm.performDelete = function () {
                // Prevent double clicking casuing additional delete requests
                vm.saveButtonState = 'busy';

                // Reset the error message
                vm.error = null;

                vm.page.deleteButtonState = 'busy';
                vm.page.saveButtonState = 'busy';
                deleteAsync(id)
                    .then(success => {
                        $scope.$apply(() => {
                            if (success) {
                                notificationsService.success('Deleted successfully.');
                                // Close the menu
                                navigationService.hideMenu();

                                $location.path(listRoute(storeId)).search({});
                            }
                        });
                    }, function (err) {
                        $scope.$apply(() => {
                            // Set the error object
                            vm.error = err;

                            // Set not busy
                            vm.saveButtonState = 'error';
                        });
                    });
            };

            vm.cancel = function () {
                navigationService.hideDialog();
            };
        }]);
