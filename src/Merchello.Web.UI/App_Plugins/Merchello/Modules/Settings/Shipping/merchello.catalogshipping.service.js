(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name umbraco.resources.MerchelloCatalogShippingService
        * @description Loads in data for shipping providers and store shipping settings
        **/
    merchelloServices.MerchelloCatalogShippingService = function ($http, umbRequestHelper) {

        return {

            addShipMethod: function (shipMethod) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'AddShipMethod'),
                        shipMethod
                    ),
                    'Failed to create ship method');
            },

            deleteShipCountry: function (shipCountryKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'DeleteShipCountry'),
                        method: "GET",
                        params: { id: shipCountryKey }
                    }),
                    'Failed to delete ship country');
            },

            deleteShipMethod: function (shipMethod) {
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'DeleteShipMethod'),
                        shipMethod
                    ),
                    'Failed to delete ship method');
            },

            getAllShipCountryProviders: function (shipCountry) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'GetAllShipCountryProviders'),
                        method: "GET",
                        params: { id: shipCountry.key }
                    }),
                    'Failed to retreive shipping gateway providers');
            },

            getAllShipGatewayProviders: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'GetAllShipGatewayProviders'),
                        method: "GET"
                    }),
                    'Failed to retreive shipping gateway providers');
            },

            getShippingProviderShipMethods: function (shipProvider) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'GetShippingProviderShipMethods'),
                        method: "GET",
                        params: { id: shipProvider.key }
                    }),
                    'Failed to retreive shipping methods');
            },

            getShippingProviderShipMethodsByCountry: function (shipProvider, shipCountry) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'GetShippingProviderShipMethodsByCountry'),
                        method: "GET",
                        params: { id: shipProvider.key, shipCountryId: shipCountry.key }
                    }),
                    'Failed to retreive shipping methods');
            },

            getAllShipGatewayResourcesForProvider: function (shipProvider) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'GetAllShipGatewayResourcesForProvider'),
                        method: "GET",
                        params: { id: shipProvider.key }
                    }),
                    'Failed to retreive shipping gateway provider resources');
            },

            getShippingCountry: function (id) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'GetShipCountry'),
                        method: "GET",
                        params: { id: id }
                    }),
                    'Failed to retreive data for shipping country: ' + id);
            },

            getWarehouseCatalogShippingCountries: function (id) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'GetAllShipCountries'),
                        method: "GET",
                        params: { id: id }
                    }),
                    'Failed to retreive shipping country data for warehouse catalog');
            },

            newWarehouseCatalogShippingCountry: function (catalogKey, countryCode) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'NewShipCountry'),
                        method: "GET",
                        params: { catalogKey: catalogKey, countryCode: countryCode }
                    }),
                    'Failed to create ship country: ' + countryCode);
            },

            saveShipMethod: function (shipMethod) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'PutShipMethod'),
                        shipMethod
                    ),
                    'Failed to save ship method');
            },

        };
    };

    angular.module('umbraco.resources').factory('merchelloCatalogShippingService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloCatalogShippingService]);

}(window.merchello.Services = window.merchello.Services || {}));
