angular.module('merchello.providers').controller('Merchello.Providers.Dialogs.BraintreeRefundPaymentController',
    ['$scope', 'invoiceHelper',
        function($scope, invoiceHelper) {

            $scope.wasFormSubmitted = false;
            $scope.save = save;

            function init() {
                $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.appliedAmount, 2);
                $scope.dialogData.warning = 'Please note this operation will refund process a refund with Braintree.';
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
