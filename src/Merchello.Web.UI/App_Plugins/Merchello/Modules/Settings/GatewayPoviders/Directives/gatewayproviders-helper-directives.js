(function(directives, undefined) {

    /**
     * @ngdoc directive
     * @name GatewayProviderResolverListDirective
     * @function
     * 
     * @description
     * directive to display resolved gateway providers
     */

    directives.GatewayProviderResolverListDirective = function () {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Modules/Settings/GatewayProviders/Directives/gatewayproviderresolver-list.html'
        };
    };

    angular.module("umbraco").directive('shippingCountry', merchello.Directives.GatewayProviderResolverListDirective);

}(window.merchello.Directives = window.merchello.Directives || {}));