    /**
     * @ngdoc service
     * @name merchello.resources.paymentResource
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

            getPaymentsByInvoice: function (invoiceKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'GetPaymentsByInvoice'),
                        method: "GET",
                        params: { id: invoiceKey }
                    }),
                    'Failed to get payments by invoice: ' + invoiceKey);
            },

            getAppliedPaymentsByInvoice: function (invoiceKey) {
                var deferred = $q.defer();
                var appliedPayments = [];

                var promise = paymentService.getPaymentsByInvoice(invoiceKey);
                promise.then(function (data) {
                    var payments = _.map(data, function (payment) {
                        return new merchello.Models.Payment(payment);
                    });

                    angular.forEach(payments, function (payment) {
                        angular.forEach(payment.appliedPayments, function (appliedPayment) {
                            var tempAppliedPayment = appliedPayment;
                            payment.appliedPayments = [];
                            tempAppliedPayment.payment = payment;
                            appliedPayments.push(tempAppliedPayment);
                        });
                    });

                    deferred.resolve(appliedPayments);
                }, function (reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
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
            }
        };
    }]);
