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

        function round(num, places) {
            return +(Math.round(num + "e+" + places) + "e-" + places);
        }

        $scope.payments = {};

        $scope.paymentRequest = new merchello.Models.PaymentRequest();
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

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Order.Dialogs.CapturePaymentController", ['$scope', merchello.Controllers.CapturePaymentController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
