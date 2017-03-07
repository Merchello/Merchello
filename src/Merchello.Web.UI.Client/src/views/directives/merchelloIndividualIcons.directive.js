// a save icon
angular.module('merchello.directives').directive('merchelloSaveIcon', function(localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            showSave: '=',
            doSave: '&'
        },
        template: '<span class="merchello-icons">' +
        '<a class="merchello-icon merchello-icon-provinces" data-ng-show="showSave" ng-click="doSave()" title="{{title}}" prevent-default>' +
        '<i class="icon icon-save"></i>' +
        '</a></span>',
        link: function(scope, elm, attr) {
            scope.title = '';
            localizationService.localize('buttons_save').then(function(value) {
                scope.title = value;
            });
        }
    }
});

angular.module('merchello.directives').directive('merchelloSortIcon', function(localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            doSort: '&'
        },
        template: '<span class="merchello-icons">' +
        '<a class="merchello-icon" ng-click="doSort()" title="{{title}}" prevent-default>' +
        '<i class="icon icon-navigation"></i>' +
        '</a></span>',
        link: function(scope, elm, attr) {

            scope.title = '';
            localizationService.localize('actions_sort').then(function(value) {
                scope.title = value;
            });
        }
    }
});

// the add icon
angular.module('merchello.directives').directive('merchelloAddIcon', function(localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            doAdd: '&',
        },
        template: '<span class="merchello-icons">' +
        '<a class="merchello-icon merchello-icon-add" ng-click="doAdd()" title="{{title}}" prevent-default>' +
        '<i class="icon icon-add"></i>' +
        '</a></span>',
        link: function(scope, elm, attr) {
            scope.title = '';
            localizationService.localize('general_add').then(function(value) {
                scope.title = value;
            });
        }
    }
});

// the edit icon
angular.module('merchello.directives').directive('merchelloEditIcon', function(localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            doEdit: '&',
        },
        template: '<span class="merchello-icons">' +
           '<a class="merchello-icon merchello-icon-edit" ng-click="doEdit()" title="{{title}}" prevent-default>' +
            '<i class="icon icon-edit"></i>' +
            '</a></span>',
        link: function(scope, elm, attr) {
            scope.title = '';
            localizationService.localize('general_edit').then(function(value) {
                scope.title = value;
            });
        }
    }
});

// the delete icon
angular.module('merchello.directives').directive('merchelloDeleteIcon', function(localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            doDelete: '&',
        },
        template: '<span class="merchello-icons">' +
        '<a class="merchello-icon merchello-icon-delete" ng-click="doDelete()" title="{{title}}" prevent-default>' +
        '<i class="icon icon-trash"></i>' +
        '</a></span>',
        link: function(scope, elm, attr) {
            scope.title = '';
            localizationService.localize('general_delete').then(function(value) {
                scope.title = value;
            });
        }
    }
});

// the provinces icon
angular.module('merchello.directives').directive('merchelloProvincesIcon', function(localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            showProvinces: '=',
            doProvinces: '&',
        },
        template: '<span class="merchello-icons">' +
        '<a class="merchello-icon merchello-icon-provinces" data-ng-show="showProvinces" ng-click="doProvinces()" title="{{title}}" prevent-default>' +
        '<i class="icon icon-globe-alt"></i>' +
        '</a></span>',
        link: function(scope, elm, attr) {
            scope.title = '';
            localizationService.localize('merchelloShippingMethod_adjustIndividualRegions').then(function(value) {
                scope.title = value;
            });
        }
    }
});

// the move icon
angular.module('merchello.directives').directive('merchelloMoveIcon', function(localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            doMove: '&'
        },
        template: '<span class="merchello-icons">' +
        '<a class="merchello-icon merchello-icon-edit" ng-click="doMove()" title="{{title}}" prevent-default>' +
        '<i class="icon icon-width"></i>' +
        '</a></span>',
        link: function(scope, elm, attr) {
            scope.title = '';
            localizationService.localize('general_move').then(function (value) {
                scope.title = value;
            });

        }
    }
});

// the Create button that emulates Umbraoc's directive
angular.module('merchello.directives').directive('merchelloCreateButton', function(localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            doCreate: '&',
            links: '='     // JSON format { text: [text], url: [url] }
        },
        template: '<div class="btn-group">' +
        '<a class="btn dropdown-toggle" data-toggle="dropdown" href="#">' +
        '<localize key="actions_create">Create</localize>' +
        '<span class="caret"></span>' +
        '</a>' +
        '<ul class="dropdown-menu">' +
        '<li>' +
        '<a href="#">' + '' + '</a>' +
        '</li>' +
        '</ul></div>',
        link: function(scope, elm, attr) {

        }
    }
});
