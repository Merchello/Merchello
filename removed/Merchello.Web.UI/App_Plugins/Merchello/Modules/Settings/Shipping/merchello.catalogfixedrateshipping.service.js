(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name umbraco.resources.MerchelloCatalogFixedRateShippingService
        * @description Loads in data for shipping providers and store shipping settings
        **/
    merchelloServices.MerchelloCatalogFixedRateShippingService = function($http, umbRequestHelper) {

        return {
            getAllFixedRateTypes: function(id) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogFixedRateShippingApiBaseUrl', 'GetAllShipCountryFixedRateProviders'),
                        method: "GET",
                        params: { id: id }
                    }),
                    'Failed to retreive shipping rate table providers');
            },

            getAllFixedRateGatewayResources: function() {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogFixedRateShippingApiBaseUrl', 'GetAllFixedRateGatewayResources'),
                        method: "GET"
                    }),
                    'Failed to retreive shipping fixed rate gateway resources');
            },

            getAllShipCountryFixedRateProviders: function(id) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogFixedRateShippingApiBaseUrl', 'GetAllShipCountryFixedRateProviders'),
                        method: "GET",
                        params: { id: id }
                    }),
                    'Failed to retreive shipping rate table providers');
            },

            getAllFixedRateProviderMethods: function(id) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogFixedRateShippingApiBaseUrl', 'GetAllFixedRateProviderMethods'),
                        method: "GET",
                        params: { id: id }
                    }),
                    'Failed to retreive shipping rate table providers');
            },

            createRateTableShipMethod: function(rateTableShipMethod) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloCatalogFixedRateShippingApiBaseUrl', 'AddFixedRateShipMethod'),
                        rateTableShipMethod
                    ),
                    'Failed to create rateTableShipMethod');
            },

            saveRateTableShipMethod: function(rateTableShipMethod) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloCatalogFixedRateShippingApiBaseUrl', 'PutFixedRateShipMethod'),
                        rateTableShipMethod
                    ),
                    'Failed to save rateTableShipMethod');
            },

            deleteRateTableShipMethod: function(rateTableShipMethod) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloCatalogFixedRateShippingApiBaseUrl', 'DeleteRateTableShipMethod'),
                        rateTableShipMethod
                    ),
                    'Failed to delete rateTableShipMethod');
            }

        };
    };

    angular.module('umbraco.resources').factory('merchelloCatalogFixedRateShippingService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloCatalogFixedRateShippingService]);

}(window.merchello.Services = window.merchello.Services || {}));
