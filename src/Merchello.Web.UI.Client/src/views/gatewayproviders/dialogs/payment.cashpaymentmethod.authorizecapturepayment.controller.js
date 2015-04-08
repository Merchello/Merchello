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
    .controller('Merchello.GatewayProviders.Dialogs.CashPaymentMethodAuthorizeCapturePaymentController',
    ['$scope', 'invoiceHelper',
        function($scope, invoiceHelper) {

            function init() {
                $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.invoiceBalance, 2);
            }

            init();

    }]);
