    /**
     * @ngdoc directive
     * @name filter-by-date-range
     * @function
     *
     * @description
     * Directive to wrap all Merchello Mark up.
     */
    angular.module('merchello.directives').directive('filterByDateRange', function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                dateFormat: '=',
                filterWithDates: '&'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/filterbydaterange.tpl.html'
        };
    });
