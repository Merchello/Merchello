    /**
     * @ngdoc resource
     * @name paymentGatewayProviderResource
     * @description Loads in data for payment providers
     **/
    angular.module('merchello.resources').factory('paymentGatewayProviderResource',
        ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

            var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloPaymentGatewayApiBaseUrl'];

            return {
                getGatewayResources: function (paymentGatewayProviderKey) {
                    var url = baseUrl + 'GetGatewayResources';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: {id: paymentGatewayProviderKey}
                        }),
                        'Failed to retreive gateway resource data for warehouse catalog');
                },

                getAllGatewayProviders: function () {
                    var url = baseUrl + 'GetAllGatewayProviders';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET"
                        }),
                        'Failed to retreive data for all gateway providers');
                },

                getPaymentProviderPaymentMethods: function (paymentGatewayProviderKey) {
                    var url = baseUrl + 'GetPaymentProviderPaymentMethods';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: {id: paymentGatewayProviderKey}
                        }),
                        'Failed to payment provider methods for: ' + paymentGatewayProviderKey);
                },

                getAvailablePaymentMethods: function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetAvailablePaymentMethods',
                            method: "GET"
                        }),
                        'Failed to load payment methods');
                },

                getPaymentMethodByKey: function(paymentMethodKey) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetPaymentMethodByKey',
                            method: "GET",
                            params: {key: paymentMethodKey}
                        }),
                        'Failed to payment method: ' + paymentMethodKey);
                },

                addPaymentMethod: function (paymentMethod) {
                    var url = baseUrl + 'AddPaymentMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            paymentMethod
                        ),
                        'Failed to create paymentMethod');
                },

                savePaymentMethod: function (paymentMethod) {
                    var url = baseUrl + 'PutPaymentMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            paymentMethod
                        ),
                        'Failed to save paymentMethod');
                },

                deletePaymentMethod: function (paymentMethodKey) {
                    var url = baseUrl + 'DeletePaymentMethod';
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
