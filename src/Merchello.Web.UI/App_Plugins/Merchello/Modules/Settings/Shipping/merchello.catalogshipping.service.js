(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name umbraco.resources.MerchelloCatalogShippingService
        * @description Loads in data for shipping providers and store shipping settings
        **/
    merchelloServices.MerchelloCatalogShippingService = function ($http, umbRequestHelper) {

        return {

            getWarehouseCatalogShippingCountries: function (id) {

                return umbRequestHelper.resourcePromise(
                   $http({
                       url: umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'GetAllShipCountries'),
                       method: "GET",
                       params: { id: id }
                    }),
                   'Failed to retreive shipping country data for warehouse catalog');
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

            newWarehouseCatalogShippingCountry: function (catalogKey, countryCode) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'NewShipCountry'),
                        method: "GET",
                        params: { catalogKey: catalogKey, countryCode: countryCode }
                    }),
                    'Failed to create ship country: ' + countryCode);
            },

            getAllShipGatewayProviders: function () {

                return umbRequestHelper.resourcePromise(
                   $http({
                       url: umbRequestHelper.getApiUrl('merchelloCatalogShippingApiBaseUrl', 'GetAllShipGatewayProviders'),
                       method: "GET"
                   }),
                   'Failed to retreive shipping gateway providers');
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

        };
    }

    angular.module('umbraco.resources').factory('merchelloCatalogShippingService', merchello.Services.MerchelloCatalogShippingService);

}(window.merchello.Services = window.merchello.Services || {}));
