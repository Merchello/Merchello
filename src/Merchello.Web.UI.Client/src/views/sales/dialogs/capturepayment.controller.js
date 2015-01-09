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
        .controller('Merchello.Sales.Dialogs.CapturePaymentController',
        ['$scope', function($scope) {

            function round(num, places) {
                return +(Math.round(num + "e+" + places) + "e-" + places);
            }

            console.info($scope.dialogData);

    }]);