(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name ShippingHistoryDirective
     * @function
     * 
     * @description
     * directive to display shipping history
     */
    directives.ShippingHistoryDirective = function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
            	shipments: '='
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Order/Directives/shipping-history.html'
        };
    };

    angular.module("umbraco").directive('merchelloShippingHistory', merchello.Directives.ShippingHistoryDirective);

    
}(window.merchello.Directives = window.merchello.Directives || {}));

