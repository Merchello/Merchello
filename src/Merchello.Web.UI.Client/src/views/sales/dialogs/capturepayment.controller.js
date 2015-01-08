    'use strict';
    /**
     * @ngdoc controller
     * @name Merchello.Sales.Dialog.CapturePaymentController
     * @function
     *
     * @description
     * The controller for the dialog used in capturing payments on the sales overview page
     */
    angular.module('merchello')
        .controller('Merchello.Sales.Dialog.CapturePaymentController',
        ['$scope', function($scope) {

            function round(num, places) {
                return +(Math.round(num + "e+" + places) + "e-" + places);
            }

            $scope.payments = {};

            $scope.paymentRequest = {}; //new merchello.Models.PaymentRequest();
            $scope.paymentRequest.invoiceKey = $scope.dialogData.key;
            $scope.paymentRequest.amount = round($scope.dialogData.total, 2);

            var payments = _.map($scope.dialogData.appliedPayments, function(appliedPayment) {
                return appliedPayment.payment;
            });

            if (payments.length > 0) {
                $scope.payments = payments[0];
                $scope.paymentRequest.paymentKey = $scope.payments.key;
                $scope.paymentRequest.paymentMethodKey = $scope.payments.paymentMethodKey;
            }

    }]);