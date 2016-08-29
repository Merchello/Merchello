angular.module('merchello.directives').directive('entityFilterGroup',
    function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                preValuesLoaded: '=',
                collection: '=',
                entityType: '=',
                doSave: '&',
                autoSave: '=?'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/entity.filtergroup.tpl.html',
            link: function (scope, elm, attr) {

                var auto = ('autoSave' in attr && 'doSave' in attr) ? scope.autoSave : false;


                // this is used directly from the embedded directive not when the directive is used in a dialog
                scope.save = function(att) {
                    console.info(att);
                    if (!auto) return;
                    scope.doSave()(scope.collection, att);
                }

            }
        }
});
