(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name MerchelloPagerDirective
     * @function
     * 
     * @description
     * directive to display display a pager for orders, products, and others.  
     *
     * TODO: Currently, makes assumptions using the parent scope.  In future, make this work as an isolate scope.
     */
    directives.MerchelloPagerDirective = function () {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Directives/merchello-pager.html'
        };
    };

    angular.module("umbraco").directive('merchelloPager', merchello.Directives.MerchelloPagerDirective);


}(window.merchello.Directives = window.merchello.Directives || {}));

