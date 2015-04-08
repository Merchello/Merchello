angular.module('merchello.resources')
    .factory('shippingFixedRateProviderResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {

        return {

            getRateTable: function(shipMethod) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloFixedRateShippingApiBaseUrl'] + 'GetShipFixedRateTable';
                return umbRequestHelper.resourcePromise(
                    $http.post(url, shipMethod),
                    'Failed to acquire rate table');

            },

            saveRateTable: function(rateTable) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloFixedRateShippingApiBaseUrl'] + 'PutShipFixedRateTable';
                return umbRequestHelper.resourcePromise(
                    $http.post(url, rateTable),
                    'Failed to save rate table');
            }

        };

    }]);
