angular.module('merchello.providers').controller('Merchello.Providers.Dialogs.BraintreeStandardVoidPaymentController',
    ['$scope',
        function($scope) {

            function init() {
                $scope.dialogData.warning = 'Please note this will only void the store payment record and this DOES NOT pass any information onto Braintree.'
            }

            // initialize the controller
            init();
        }]);
