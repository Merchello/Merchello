/**
 * @ngdoc directive
 * @name merchello-panel
 * @function
 *
 * @description
 * Directive to wrap all Merchello Mark up and provide common classes.
 */
angular.module('merchello.directives').directive('merchelloSpinner', function() {
    return {
        restrict: 'E',
        replace: true,
        transclude: 'true',
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/html/merchellospinner.tpl.html'
    };
});
