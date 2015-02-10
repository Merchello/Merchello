angular.module('merchello.directives').directive('merchelloTabs', [function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            tabs: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/html/merchellotabs.tpl.html'
    };
}]);
