import { MODULE } from './constants';

angular.module(MODULE.NAME, [])
  .config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('commerceProductFeedsRouteInterceptor');
  }]);

// routing
angular.module('umbraco.commerce.productfeeds').factory('commerceProductFeedsRouteInterceptor', [
  function () {
    return {
      'request': function (config) {
        const pattern = /views\/umbracocommerceproductfeeds\//gi;
        if (pattern.test(config.url)) {
          // re-routing when matches
          config.url = config.url.replace(pattern, '/App_Plugins/umbracocommerceproductfeeds/angularjs/');
        }

        return config;
      },
    };
  }]);

angular.module('umbraco.commerce').requires.push(MODULE.NAME);