/**
 * @ngdoc directive
 * @name static-collection-tree-picker
 * @function
 *
 * @description
 * Directive to pick static entity collections.
 */
angular.module('merchello.directives').directive('entityStaticCollections',
    function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                preValuesLoaded: '=',
                entity: '=',
                entityType: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/entity.staticcollections.tpl.html',
            controller: 'Merchello.Directives.EntityStaticCollectionsDirectiveController'
        }
});
