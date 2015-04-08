    /**
     * @ngdoc resource
     * @name paymentResource
     * @description Loads in data and allows modification for payments
     **/
    angular.module('merchello.resources').factory('paymentResource',
        ['$q', '$http', 'umbRequestHelper',
        function($q, $http, umbRequestHelper) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloPaymentApiBaseUrl'];

        return {

            getPayment: function (key) {
                var url = baseUrl + 'GetPaymeent';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to get payment: ' + key);
            },

            getPaymentMethod : function(key) {
                var url = baseUrl + 'GetPaymentMethod';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to get payment method: ' + key);
            },

            getPaymentsByInvoice: function (invoiceKey) {
                var url = baseUrl + 'GetPaymentsByInvoice';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { id: invoiceKey }
                    }),
                    'Failed to get payments by invoice: ' + invoiceKey);
            },

            authorizePayment: function (paymentRequest) {
                var url = baseUrl + 'AuthorizePayment';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        paymentRequest
                    ),
                    'Failed to authorize payment');
            },

            capturePayment: function (paymentRequest) {
                var url = baseUrl + 'CapturePayment';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        paymentRequest
                    ),
                    'Failed to capture payment');
            },

            authorizeCapturePayment: function (paymentRequest) {
                var url = baseUrl + 'AuthorizeCapturePayment';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        paymentRequest
                    ),
                    'Failed to authorize capture payment');
            },

            refundPayment: function (paymentRequest) {
                var url = baseUrl + 'RefundPayment';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        paymentRequest
                    ),
                    'Failed to refund payment');
            },

            voidPayment: function (paymentRequest) {
                return umbRequestHelper.resourcePromise(
                    $http.post(baseUrl + 'VoidPayment',
                        paymentRequest
                    ),
                    'Failed to void payment');
            }
        };
    }]);
