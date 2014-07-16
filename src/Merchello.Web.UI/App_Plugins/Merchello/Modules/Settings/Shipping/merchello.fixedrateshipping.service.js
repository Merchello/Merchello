(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name umbraco.resources.MerchelloFixedRateShippingService
        * @description Loads in data for getting and saving rate tables for fixed rate shipping.
        **/
    merchelloServices.MerchelloFixedRateShippingService = function ($http, umbRequestHelper) {

        return {

            getRateTable: function(shipMethod) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloRateTableApiBaseUrl', 'GetShipFixedRateTable'), shipMethod),
                    'Failed to acquire rate table');

            },

            saveRateTable: function(rateTable) {
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloRateTableApiBaseUrl', 'PutShipFixedRateTable'), rateTable),
                    'Failed to save rate table');
            }

        };
    };

    angular.module('umbraco.resources').factory('merchelloFixedRateShippingService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloFixedRateShippingService]);

}(window.merchello.Services = window.merchello.Services || {}));
