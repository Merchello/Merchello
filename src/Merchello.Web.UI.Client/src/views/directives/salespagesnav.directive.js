    /**
     * @ngdoc directive
     * @name MerchelloSalesPageNav
     * @function
     *
     * @description
     * directive to display display a buttons on subordinate sales pages.
     */

    angular.module('merchello.directives').directive('merchelloSalesPageNav', function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                key: '=',
                hideShipments: '=',
                hidePayments: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/salespagesnav.tpl.html'
        };
    });
