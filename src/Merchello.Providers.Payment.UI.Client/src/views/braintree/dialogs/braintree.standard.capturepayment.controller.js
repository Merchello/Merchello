angular.module('merchello.payments').controller('Merchello.Payments.Dialogs.BraintreeStandardCapturePaymentController',
    ['$scope', 'invoiceHelper',
        function($scope, invoiceHelper) {

            $scope.transaction = {};

            function init() {
                $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.invoiceBalance, 2);
                var transactionStr = $scope.dialogData.payment.extendedData.getValue('braintreeTransaction');
                $scope.transaction = JSON.parse(transactionStr);
                $scope.dialogData.warning = 'This action will submit a previously authorized transaction for settlement.';
            }


            init();
            // initialize the controller
        }]);

