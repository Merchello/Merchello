    'use strict';
    /**
     * @ngdoc controller
     * @name Merchello.GatewayProviders.Dialogs.CashPaymentMethodRefundPaymentController
     * @function
     *
     * @description
     * The controller for the dialog used in refunding cash payments
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.CashPaymentMethodRefundPaymentController',
        ['$scope', 'invoiceHelper',
        function($scope, invoiceHelper) {

            $scope.wasFormSubmitted = false;
            $scope.save = save;

            function init() {
                $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.appliedAmount, 2);
            }

            function save() {
                $scope.wasFormSubmitted = true;
                if(invoiceHelper.valueIsInRage($scope.dialogData.amount, 0, $scope.dialogData.appliedAmount)) {
                    $scope.submit($scope.dialogData);
                } else {
                    $scope.refundForm.amount.$setValidity('amount', false);
                }
            }



            // initializes the controller
            init();
    }]);
