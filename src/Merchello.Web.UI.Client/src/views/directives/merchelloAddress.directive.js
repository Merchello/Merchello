/**
 * @ngdoc directive
 * @name address directive
 * @function
 *
 * @description
 * Directive to maintain a consistent format for displaying addresses
 */
angular.module('merchello.directives').directive('merchelloAddress', function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            address: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchelloAddress.tpl.html'
    };
});
