(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Order.Dialogs.CapturePaymentController
     * @function
     * 
     * @description
     * The controller for the capturing of authorized payments on the Order View page
     */
    controllers.CapturePaymentController = function ($scope) {

        $scope.payments = {};

        $scope.paymentRequest = new merchello.Models.PaymentRequest();
        $scope.paymentRequest.invoiceKey = $scope.dialogData.key;
        $scope.paymentRequest.amount = $scope.dialogData.total;
        if ($scope.dialogData.payments.length > 0) {
            $scope.payments = $scope.dialogData.payments[0];
            $scope.paymentRequest.paymentKey = $scope.payments.key;
            $scope.paymentRequest.paymentMethodKey = $scope.payments.paymentMethodKey;
        }

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Order.Dialogs.CapturePaymentController", ['$scope', merchello.Controllers.CapturePaymentController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
