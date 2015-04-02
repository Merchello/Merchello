    /**
     * @ngdoc resource
     * @name paymentResource
     * @description Loads in data and allows modification for payments
     **/
    angular.module('merchello.resources').factory('paymentResource',
        ['$q', '$http', 'umbRequestHelper',
        function($q, $http, umbRequestHelper) {

        return {

            getPayment: function (key) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'GetPayment'),
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to get payment: ' + key);
            },

            getPaymentMethod : function(key) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'GetPaymentMethod'),
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to get payment method: ' + key);
            },

            getPaymentsByInvoice: function (invoiceKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'GetPaymentsByInvoice'),
                        method: "GET",
                        params: { id: invoiceKey }
                    }),
                    'Failed to get payments by invoice: ' + invoiceKey);
            },

            authorizePayment: function (paymentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'AuthorizePayment'),
                        paymentRequest
                    ),
                    'Failed to authorize payment');
            },

            capturePayment: function (paymentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'CapturePayment'),
                        paymentRequest
                    ),
                    'Failed to capture payment');
            },

            authorizeCapturePayment: function (paymentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'AuthorizeCapturePayment'),
                        paymentRequest
                    ),
                    'Failed to authorize capture payment');
            },

            refundPayment: function (paymentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'RefundPayment'),
                        paymentRequest
                    ),
                    'Failed to refund payment');
            },

            voidPayment: function (paymentRequest) {
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'VoidPayment'),
                        paymentRequest
                    ),
                    'Failed to void payment');
            }
        };
    }]);
