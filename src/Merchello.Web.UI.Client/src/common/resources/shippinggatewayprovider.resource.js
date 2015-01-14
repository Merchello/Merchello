/**
 * @ngdoc service
 * @name merchello.resources.shippingGatewayProviderResource
 * @description Loads in data for shipping providers and store shipping settings
 **/
angular.module('merchello.resources')
    .factory('shippingGatewayProviderResource',
    ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

            return {

                addShipMethod: function (shipMethod) {

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'AddShipMethod'),
                            shipMethod
                        ),
                        'Failed to create ship method');
                },

                deleteShipCountry: function (shipCountryKey) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'DeleteShipCountry'),
                            method: "GET",
                            params: { id: shipCountryKey }
                        }),
                        'Failed to delete ship country');
                },

                deleteShipMethod: function (shipMethod) {
                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'DeleteShipMethod'),
                            shipMethod
                        ),
                        'Failed to delete ship method');
                },

                getAllShipCountryProviders: function (shipCountry) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetAllShipCountryProviders'),
                            method: "GET",
                            params: { id: shipCountry.key }
                        }),
                        'Failed to retreive shipping gateway providers');
                },

                getAllShipGatewayProviders: function () {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetAllShipGatewayProviders'),
                            method: "GET"
                        }),
                        'Failed to retreive shipping gateway providers');
                },

                getShippingProviderShipMethods: function (shipProvider) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetShippingProviderShipMethods'),
                            method: "GET",
                            params: { id: shipProvider.key }
                        }),
                        'Failed to retreive shipping methods');
                },

                getShippingProviderShipMethodsByCountry: function (shipProvider, shipCountry) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetShippingProviderShipMethodsByCountry'),
                            method: "GET",
                            params: { id: shipProvider.key, shipCountryId: shipCountry.key }
                        }),
                        'Failed to retreive shipping methods');
                },

                getAllShipGatewayResourcesForProvider: function (shipProvider) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetAllShipGatewayResourcesForProvider'),
                            method: "GET",
                            params: { id: shipProvider.key }
                        }),
                        'Failed to retreive shipping gateway provider resources');
                },

                getShippingCountry: function (id) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetShipCountry'),
                            method: "GET",
                            params: { id: id }
                        }),
                        'Failed to retreive data for shipping country: ' + id);
                },

                getWarehouseCatalogShippingCountries: function (id) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetAllShipCountries'),
                            method: "GET",
                            params: { id: id }
                        }),
                        'Failed to retreive shipping country data for warehouse catalog');
                },

                newWarehouseCatalogShippingCountry: function (catalogKey, countryCode) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'NewShipCountry'),
                            method: "GET",
                            params: { catalogKey: catalogKey, countryCode: countryCode }
                        }),
                        'Failed to create ship country: ' + countryCode);
                },

                saveShipMethod: function (shipMethod) {

                    if (shipMethod.key === '') {
                        return this.addShipMethod(shipMethod);
                    }

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'PutShipMethod'),
                            shipMethod
                        ),
                        'Failed to save ship method');
                }

            };
        }]);
