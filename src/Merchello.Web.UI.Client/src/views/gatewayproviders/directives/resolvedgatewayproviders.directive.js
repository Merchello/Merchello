angular.module('merchello.directives').directive('resolvedGatewayProviders', [function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            providerList: '=',
            'activate': '&onActivate',
            'deactivate': '&onDeactivate',
            'configure': '&onConfigure'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/resolvedgatewayproviders.tpl.html'
    };
}]);
