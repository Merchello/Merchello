(function() {

    angular.module('merchello.plugins.braintree').controller('Merchello.Plugins.Braintree.Dialogs.Standard.AuthorizePaymentController',
    ['$scope', 'invoiceHelper',
    function($scope, invoiceHelper) {

    }]);

    angular.module('merchello.plugins.braintree').controller('Merchello.Plugins.Braintree.Dialogs.Standard.VoidPaymentController',
        ['$scope',
        function($scope) {

            function init() {
                $scope.dialogData.warning = 'Please note this will only void the store payment record and this DOES NOT pass any information onto Braintree.'
            }

            // initialize the controller
            init();
        }]);

    angular.module('merchello').controller('Merchello.Plugins.Braintree.Dialogs.Dialogs.RefundPaymentController',
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

}());