    /**
     * @ngdoc resource
     * @name paymentGatewayProviderResource
     * @description Loads in data for payment providers
     **/
    angular.module('merchello.resources').factory('paymentGatewayProviderResource',
        ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

            return {
                getGatewayResources: function (paymentGatewayProviderKey) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloPaymentGatewayApiBaseUrl'] + 'GetGatewayResources';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: {id: paymentGatewayProviderKey}
                        }),
                        'Failed to retreive gateway resource data for warehouse catalog');
                },

                getAllGatewayProviders: function () {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloPaymentGatewayApiBaseUrl'] + 'GetAllGatewayProviders';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET"
                        }),
                        'Failed to retreive data for all gateway providers');
                },

                getPaymentProviderPaymentMethods: function (paymentGatewayProviderKey) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloPaymentGatewayApiBaseUrl'] + 'GetPaymentProviderPaymentMethods';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: {id: paymentGatewayProviderKey}
                        }),
                        'Failed to payment provider methods for: ' + paymentGatewayProviderKey);
                },

                addPaymentMethod: function (paymentMethod) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloPaymentGatewayApiBaseUrl'] + 'AddPaymentMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            paymentMethod
                        ),
                        'Failed to create paymentMethod');
                },

                savePaymentMethod: function (paymentMethod) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloPaymentGatewayApiBaseUrl'] + 'PutPaymentMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            paymentMethod
                        ),
                        'Failed to save paymentMethod');
                },

                deletePaymentMethod: function (paymentMethodKey) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloPaymentGatewayApiBaseUrl'] + 'DeletePaymentMethod';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: {id: paymentMethodKey}
                        }),
                        'Failed to delete paymentMethod');
                }

            };

    }]);
