angular.module('merchello.directives').directive('productOptionsList', [
    '$q', 'localizationService', 'queryDisplayBuilder', 'queryResultDisplayBuilder', 'productOptionDisplayBuilder',
    function($q, localizationService, queryDisplayBuilder, queryResultDisplayBuilder, productOptionDisplayBuilder) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            load: '&',
            sharedOnly: '=?',
            preValuesLoaded: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/productoptions.list.tpl.html',
        link: function(scope, elm, attr) {

            scope.loaded = false;
            scope.totalItems = 0;

            scope.isReady = false;

            scope.noResults = '';

            scope.options = {
                pageSize: 10,
                currentPage: 1,
                filter: '',
                orderBy: 'name',
                orderDirection: 'asc'
            };

            scope.queryResult = queryResultDisplayBuilder.createDefault();

            /// PRIVATE

            var yes = '';
            var no = '';
            var values = '';

            function init() {

                var isShared = ('sharedOnly' in attr);

                var noResultsKey = isShared ? 'noSharedProductOptions' : 'noProductOptions';

                $q.all([
                    localizationService.localize('general_yes'),
                    localizationService.localize('general_no'),
                    localizationService.localize('merchelloTableCaptions_optionValues'),
                    localizationService.localize('merchelloProductOptions_' + noResultsKey)
                ]).then(function(data) {
                    yes = data[0];
                    no = data[1];
                    values = data[2];
                    scope.noResults = data[3];
                    scope.loaded = true;
                });

                scope.$watch('preValuesLoaded', function(nv, ov) {
                   if (nv === true) {
                       scope.isReady = true;
                   }

                   if (scope.isReady) {
                       search();
                   }
                });
            }

            function search() {
                var page = scope.options.currentPage - 1;
                var perPage = scope.options.pageSize;
                var sortBy = scope.options.orderBy;
                var sortDirection = scope.options.orderDirection === 'asc' ? 'Ascending' : 'Descending';
                var query = queryDisplayBuilder.createDefault();
                query.currentPage = page;
                query.itemsPerPage = perPage;
                query.sortBy = sortBy;
                query.sortDirection = sortDirection;
                query.addFilterTermParam(scope.options.filter);

                scope.load()(query).then(function(result) {

                    console.info(result);
                    scope.queryResult = result;

                    scope.pagination = [];

                    //list 10 pages as per normal
                    if (scope.queryResult.totalPages <= 10) {
                        for (var i = 0; i < scope.queryResult.totalPages; i++) {
                            scope.pagination.push({
                                val: (i + 1),
                                isActive: scope.options.currentPage == (i + 1)
                            });
                        }
                    }
                    else {
                        //if there is more than 10 pages, we need to do some fancy bits

                        //get the max index to start
                        var maxIndex = scope.queryResult.totalPages - 10;
                        //set the start, but it can't be below zero
                        var start = Math.max(scope.options.currentPage - 5, 0);
                        //ensure that it's not too far either
                        start = Math.min(maxIndex, start);

                        for (var i = start; i < (10 + start) ; i++) {
                            scope.pagination.push({
                                val: (i + 1),
                                isActive: scope.options.currentPage == (i + 1)
                            });
                        }

                        //now, if the start is greater than 0 then '1' will not be displayed, so do the elipses thing
                        if (start > 0) {
                            scope.pagination.unshift({ name: "First", val: 1, isActive: false }, {val: "...",isActive: false});
                        }

                        //same for the end
                        if (start < maxIndex) {
                            scope.pagination.push({ val: "...", isActive: false }, { name: "Last", val: scope.queryResult.totalPages, isActive: false });
                        }
                    }
                });
            }

            init();
        }
    }
}]);
