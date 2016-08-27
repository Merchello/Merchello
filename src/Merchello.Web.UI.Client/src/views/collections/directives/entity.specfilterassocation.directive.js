angular.module('merchello.directives').directive('entitySpecFilterAssociation',
    function(entityCollectionResource) {
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
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/entity.specfilterassociation.tpl.html',
            link: function (scope, elm, attr) {

                var auto = ('autoSave' in attr && 'doSave' in attr) ? scope.autoSave : false;


                // this is used directly from the embedded directive not when the directive is used in a dialog
                scope.save = function(att) {
                    if (!auto) return;
                    console.info(scope.doSave);
                    console.info(scope.collection);
                    console.info(att);
                    scope.doSave()(scope.collection, att);
                }

            }
        }
});
