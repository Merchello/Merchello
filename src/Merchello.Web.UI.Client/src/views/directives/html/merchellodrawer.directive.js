    /**
     * @ngdoc directive
     * @name merchello-drawer
     * @function
     *
     * @description
     * Directive to wrap the main function buttons in the footer of a page
     */
     angular.module('merchello.directives').directive('merchelloDrawer', function() {
         return {
             restrict: 'E',
             replace: true,
             transclude: 'true',
             templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/html/merchellodrawer.tpl.html'
         };
     });
