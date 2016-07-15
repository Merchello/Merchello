angular.module('merchello.directives').directive('productOptionsList', [
    '$q', 'localizationService',
    function($q, localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            load: '&',
            preValuesLoaded: '=',
            ready: '=?'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/productoptions.list.tpl.html',
        link: function(scope, elm, attr) {

            scope.loaded = false;
            scope.totalItems = 0;

            scope.options = {
                pageSize: 10,
                currentPage: 1,
                filter: '',
                orderBy: 'name',
                orderDirection: 'asc'
            };

            /// PRIVATE

            var yes = '';
            var no = '';
            var values = '';

            function init() {

                $q.all([
                    localizationService.localize('general_yes'),
                    localizationService.localize('general_no'),
                    localizationService.localize('merchelloTableCaptions_optionValues')
                ]).then(function(data) {
                    yes = data[0];
                    no = data[1];
                    values = data[2];
                    scope.loaded = true;
                });

                scope.$watch('preValuesLoaded', function(nv, ov) {
                   console.info('old value:' + ov);
                    console.info('new value: ' + nv);
                });
            }

            function search() {

            }

            init();
        }
    }
}]);
