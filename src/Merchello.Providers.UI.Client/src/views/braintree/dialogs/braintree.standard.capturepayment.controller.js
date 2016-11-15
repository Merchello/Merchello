angular.module('merchello.providers').controller('Merchello.Providers.Dialogs.BraintreeStandardCapturePaymentController',
    ['$scope', 'invoiceHelper',
        function($scope, invoiceHelper) {

            $scope.transaction = {};

            function init() {
                $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.invoiceBalance, 2);
                var transactionStr = $scope.dialogData.payment.extendedData.getValue('braintreeTransaction');
                console.info(transactionStr);
                if (transactionStr !== '') {
                    $scope.transaction = JSON.parse(transactionStr);
                } else {
                    $scope.close();
                }

                $scope.dialogData.warning = 'This action will submit a previously authorized transaction for settlement.';
            }


            init();
            // initialize the controller
        }]);

