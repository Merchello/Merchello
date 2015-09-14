angular.module('merchello.directives').directive('detachedContentType', function() {

    return {
        restrict: 'E',
        replace: true,
        terminal: false,

        scope: {
            entityType: '@'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/detachedcontenttype.list.tpl.html',
        controller: 'Merchello.Directives.DetachedContentTypeListController'
    };

});
