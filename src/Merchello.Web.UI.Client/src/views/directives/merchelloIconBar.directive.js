angular.module('merchello.directives').directive('merchelloIconBar', function(localizationService) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            showAdd: '=?',
            showEdit: '=?',
            showActivate: '=?',
            showDelete: '=?',
            doAdd: '&?',
            doEdit: '&?',
            doActivate: '&?',
            doDelete: '&?',
            args: '=?'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchelloiconbar.tpl.html',
        link: function(scope, elm, attr) {
            scope.editTitle = '';
            scope.deleteTitle = '';
            scope.activateTitle = '';
            scope.addTitle = '';

            localizationService.localize('general_add').then(function(value) {
              scope.addTitle = value;
            });
            localizationService.localize('general_edit').then(function(value) {
                scope.editTitle = value;
            });
            localizationService.localize('general_delete').then(function(value) {
                scope.deleteTitle = value;
            });
            localizationService.localize('merchelloGatewayProvider_activate').then(function(value) {
                scope.activateTitle = value;
            });
        }
    };

});
