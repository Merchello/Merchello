angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.CashPaymentMethodAuthorizePaymentController',
    ['$scope', 'invoiceHelper',
    function($scope, invoiceHelper) {
        function init() {
            $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.invoiceBalance, 2);
        }

        init();
}]);
