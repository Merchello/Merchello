angular.module('merchello.resources')
    .factory('shippingFixedRateProviderResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {

        return {

            getRateTable: function(shipMethod) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloFixedRateShippingApiBaseUrl', 'GetShipFixedRateTable'), shipMethod),
                    'Failed to acquire rate table');

            },

            saveRateTable: function(rateTable) {
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloFixedRateShippingApiBaseUrl', 'PutShipFixedRateTable'), rateTable),
                    'Failed to save rate table');
            }

        };

    }]);
