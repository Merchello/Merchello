/**
 * @ngdoc resource
 * @name gatewayProviderResource
 * @description Loads in data and allows modification of gateway providers
 **/
angular.module('merchello.resources')
    .factory('gatewayProviderResource',
    ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

        return {
            getGatewayProvider: function (providerKey) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'GetGatewayProvider';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { id: providerKey }
                    }),
                    'Failed to retreive gateway provider data');
            },

            getResolvedNotificationGatewayProviders: function () {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'GetResolvedNotificationGatewayProviders';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to retrieve data for all resolved notification gateway providers');
            },

            getResolvedPaymentGatewayProviders: function () {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'GetResolvedPaymentGatewayProviders';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to retreive data for all resolved payment gateway providers');
            },

            getResolvedShippingGatewayProviders: function () {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'GetResolvedShippingGatewayProviders';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to retreive data for all resolved shipping gateway providers');
            },

            getResolvedTaxationGatewayProviders: function () {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'GetResolvedTaxationGatewayProviders';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to retreive data for all resolved taxation gateway providers');
            },

            activateGatewayProvider: function (gatewayProvider) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'ActivateGatewayProvider';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        gatewayProvider
                    ),
                    'Failed to activate gateway provider');
            },

            deactivateGatewayProvider: function (gatewayProvider) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'DeactivateGatewayProvider';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        gatewayProvider
                    ),
                    'Failed to deactivate gateway provider');
            },

            saveGatewayProvider: function(gatewayProvider) {
                // we need to hack the extended data here
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'PutGatewayProvider';
                gatewayProvider.extendedData = gatewayProvider.extendedData.toArray();
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        gatewayProvider
                    ),
                    'Failed to save gateway provider');
            }
        };
    }]);
