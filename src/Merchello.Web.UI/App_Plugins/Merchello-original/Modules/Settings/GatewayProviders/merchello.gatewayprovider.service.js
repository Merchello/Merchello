(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name merchello.Services.MerchelloGatewayProviderService
        * @description Loads in data for gateway providers and support activation/deactivation as well as saving provider settings
        **/
    merchelloServices.MerchelloGatewayProviderService = function ($http, umbRequestHelper) {

        return {
            getGatewayProvider: function (providerKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'GetGatewayProvider'),
                        method: "GET",
                        params: { id: providerKey }
                    }),
                    'Failed to retreive gateway provider data');
            },

            getResolvedNotificationGatewayProviders: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'GetResolvedNotificationGatewayProviders'),
                        method: "GET"
                        }),
                    'Failed to retrieve data for all resolved notification gateway providers');
            },

            getResolvedPaymentGatewayProviders: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'GetResolvedPaymentGatewayProviders'),
                        method: "GET"
                    }),
                    'Failed to retreive data for all resolved payment gateway providers');
            },

            getResolvedShippingGatewayProviders: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'GetResolvedShippingGatewayProviders'),
                        method: "GET"
                    }),
                    'Failed to retreive data for all resolved shipping gateway providers');
            },

            getResolvedTaxationGatewayProviders: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'GetResolvedTaxationGatewayProviders'),
                        method: "GET"
                    }),
                    'Failed to retreive data for all resolved taxation gateway providers');
            },

            activateGatewayProvider: function (gatewayProvider) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'ActivateGatewayProvider'),
                        gatewayProvider
                    ),
                    'Failed to activate gateway provider');
            },

            deactivateGatewayProvider: function (gatewayProvider) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'DeactivateGatewayProvider'),
                        gatewayProvider
                    ),
                    'Failed to deactivate gateway provider');
            },

            saveGatewayProvider: function(gatewayProvider) {
                
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'PutGatewayProvider'),
                        gatewayProvider
                    ),
                    'Failed to save gateway provider');
            }

        };
    };

    angular.module('umbraco.resources').factory('merchelloGatewayProviderService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloGatewayProviderService]);

}(window.merchello.Services = window.merchello.Services || {}));
