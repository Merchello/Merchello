(function(directives, undefined) {

    /**
     * @ngdoc directive
     * @name GatewayProviderListDirective
     * @function
     * 
     * @description
     * directive to display resolved gateway providers
     */

    directives.GatewayProviderResolverListDirective = function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                providerList: '=',
                'activate': '&onActivate',
                'deactivate': '&onDeactivate',
                'configure': '&onConfigure'
        },
            templateUrl: '/App_Plugins/Merchello/Modules/Settings/GatewayProviders/Directives/gatewayproviderresolver-list.html'
        };
    };

    angular.module("umbraco").directive('resolvedGatewayProviders', merchello.Directives.GatewayProviderResolverListDirective);

}(window.merchello.Directives = window.merchello.Directives || {}));