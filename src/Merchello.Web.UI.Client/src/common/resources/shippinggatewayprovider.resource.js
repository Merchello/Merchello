/**
 * @ngdoc resource
 * @name shippingGatewayProviderResource
 * @description Loads in data for shipping providers and store shipping settings
 **/
angular.module('merchello.resources')
    .factory('shippingGatewayProviderResource',
    ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

            return {

                addShipMethod: function (shipMethod) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'AddShipMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            shipMethod
                        ),
                        'Failed to create ship method');
                },

                deleteShipCountry: function (shipCountryKey) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'DeleteShipCountry';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: shipCountryKey }
                        }),
                        'Failed to delete ship country');
                },

                deleteShipMethod: function (shipMethod) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'DeleteShipMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            shipMethod
                        ),
                        'Failed to delete ship method');
                },

                getAllShipCountryProviders: function (shipCountry) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetAllShipCountryProviders';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: shipCountry.key }
                        }),
                        'Failed to retreive shipping gateway providers');
                },

                getAllShipGatewayProviders: function () {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetAllShipGatewayProviders';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET"
                        }),
                        'Failed to retreive shipping gateway providers');
                },

                getShippingProviderShipMethods: function (shipProvider) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetShippingProviderShipMethods';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: shipProvider.key }
                        }),
                        'Failed to retreive shipping methods');
                },

                getShippingGatewayMethodsByCountry: function (shipProvider, shipCountry) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetShippingGatewayMethodsByCountry';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: shipProvider.key, shipCountryId: shipCountry.key }
                        }),
                        'Failed to retreive shipping methods');
                },

                getAllShipGatewayResourcesForProvider: function (shipProvider) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetAllShipGatewayResourcesForProvider';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: shipProvider.key }
                        }),
                        'Failed to retreive shipping gateway provider resources');
                },

                getShippingCountry: function (id) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetShipCountry';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: id }
                        }),
                        'Failed to retreive data for shipping country: ' + id);
                },

                getWarehouseCatalogShippingCountries: function (id) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetAllShipCountries';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: id }
                        }),
                        'Failed to retreive shipping country data for warehouse catalog');
                },

                newWarehouseCatalogShippingCountry: function (catalogKey, countryCode) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'NewShipCountry';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { catalogKey: catalogKey, countryCode: countryCode }
                        }),
                        'Failed to create ship country: ' + countryCode);
                },

                saveShipMethod: function (shipMethod) {

                    if (shipMethod.key === '') {
                        return this.addShipMethod(shipMethod);
                    }
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'PutShipMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            shipMethod
                        ),
                        'Failed to save ship method');
                }

            };
        }]);
