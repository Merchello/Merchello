angular.module('merchello.directives').directive('entitySpecifiedFilters',
    function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                preValuesLoaded: '=',
                entity: '=',
                entityType: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/entity.specifiedfilters.tpl.html',
            link: function(scope, elm, attr) {

                scope.ready = false;

                scope.associateFilters = function() {

                }

                function init() {
                    scope.$watch('preValuesLoaded', function(nv, ov) {
                       if (nv === true) {
                           scope.ready = true;
                       }
                    });
                }

                init();
            }
        }
        });
