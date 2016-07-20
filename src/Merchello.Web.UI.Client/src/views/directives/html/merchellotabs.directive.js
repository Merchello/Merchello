angular.module('merchello.directives').directive('merchelloTabs', [function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            tabs: '=',
            widthCss: '=?'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/html/merchellotabs.tpl.html',
        link: function(scope, elm, attr) {

            scope.span = 'span12';

            if('widthCss' in attr) {
                scope.span = scope.widthCss;
            }
        }
    };
}]);
