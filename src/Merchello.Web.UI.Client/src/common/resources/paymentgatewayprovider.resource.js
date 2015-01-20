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

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloPaymentGatewayApiBaseUrl', 'GetGatewayResources'),
                            method: "GET",
                            params: {id: paymentGatewayProviderKey}
                        }),
                        'Failed to retreive gateway resource data for warehouse catalog');
                },

                getAllGatewayProviders: function () {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloPaymentGatewayApiBaseUrl', 'GetAllGatewayProviders'),
                            method: "GET"
                        }),
                        'Failed to retreive data for all gateway providers');
                },

                getPaymentProviderPaymentMethods: function (paymentGatewayProviderKey) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloPaymentGatewayApiBaseUrl', 'GetPaymentProviderPaymentMethods'),
                            method: "GET",
                            params: {id: paymentGatewayProviderKey}
                        }),
                        'Failed to payment provider methods for: ' + paymentGatewayProviderKey);
                },

                addPaymentMethod: function (paymentMethod) {

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloPaymentGatewayApiBaseUrl', 'AddPaymentMethod'),
                            paymentMethod
                        ),
                        'Failed to create paymentMethod');
                },

                savePaymentMethod: function (paymentMethod) {

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloPaymentGatewayApiBaseUrl', 'PutPaymentMethod'),
                            paymentMethod
                        ),
                        'Failed to save paymentMethod');
                },

                deletePaymentMethod: function (paymentMethodKey) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloPaymentGatewayApiBaseUrl', 'DeletePaymentMethod'),
                            method: "GET",
                            params: {id: paymentMethodKey}
                        }),
                        'Failed to delete paymentMethod');
                }

            };

    }]);
