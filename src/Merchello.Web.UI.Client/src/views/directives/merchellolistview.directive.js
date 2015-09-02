angular.module('merchello.directives').directive('merchelloListView',
    ['$routeParams', 'merchelloListViewHelper', 'queryDisplayBuilder', 'queryResultDisplayBuilder',
    function($routeParams, merchelloListViewHelper, queryDisplayBuilder, queryResultDisplayBuilder) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                builder: '=',
                entityType: '=',
                getColumnValue: '&',
                load: '&',
                disableCollections: '@?',
                includeDateFilter: '@?',
                noTitle: '@?'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchellolistview.tpl.html',
            link: function (scope, elm, attr) {

                scope.collectionKey = '';
                scope.sort = sort;
                scope.isSortDirection = isSortDirection;
                scope.next = next;
                scope.prev = prev;
                scope.goToPage = goToPage;
                scope.enterSearch = enterSearch;
                scope.search = search;
                scope.setPageSize = setPageSize;

                scope.hasCollections = true;
                scope.enableDateFilter = false;
                scope.showTitle = true;

                scope.config = merchelloListViewHelper.getConfig(scope.entityType);

                //scope.goToEditor = goToEditor;

                scope.listViewResultSet = {
                    totalItems: 0,
                    items: []
                };

                scope.options = {
                    pageSize: 25,
                    pageNumber: 1,
                    filter: '',
                    orderBy: (scope.config.orderBy ? scope.config.orderBy : 'name').trim(),
                    orderDirection: scope.config.orderDirection ? scope.config.orderDirection.trim() : "asc"
                };

                scope.pagination = [];

                function init() {
                    scope.hasCollections = !('disableCollections' in attr);
                    scope.enableDateFilter = 'includeDateFilter' in attr;
                    scope.showTitle = !('noTitle' in attr);
                    if(scope.hasCollections) {
                        scope.collectionKey = $routeParams.id !== 'manage' ? $routeParams.id : '';
                    }
                    console.info(scope.hasCollections);
                    search();
                }

                function search() {
                    var page = scope.options.pageNumber - 1;
                    var perPage = scope.options.pageSize;
                    var sortBy = scope.options.orderBy;
                    var sortDirection = scope.options.orderDirection === 'asc' ? 'Ascending' : 'Descending';

                    var query = queryDisplayBuilder.createDefault();
                    query.currentPage = page;
                    query.itemsPerPage = perPage;
                    query.sortBy = sortBy;
                    query.sortDirection = sortDirection;
                    query.addFilterTermParam(scope.options.filter);

                    if (scope.collectionKey !== '') {
                        query.addCollectionKeyParam(scope.collectionKey);
                        query.addEntityTypeParam(scope.entityType);
                    }
                    scope.load()(query).then(function (response) {
                        var queryResult = queryResultDisplayBuilder.transform(response, scope.builder);
                        scope.listViewResultSet.items = queryResult.items;
                        scope.listViewResultSet.totalItems = queryResult.totalItems;
                        scope.listViewResultSet.totalPages = queryResult.totalPages;


                        scope.pagination = [];

                        //list 10 pages as per normal
                        if (scope.listViewResultSet.totalPages <= 10) {
                            for (var i = 0; i < scope.listViewResultSet.totalPages; i++) {
                                scope.pagination.push({
                                    val: (i + 1),
                                    isActive: scope.options.pageNumber == (i + 1)
                                });
                            }
                        }
                        else {
                            //if there is more than 10 pages, we need to do some fancy bits

                            //get the max index to start
                            var maxIndex = scope.listViewResultSet.totalPages - 10;
                            //set the start, but it can't be below zero
                            var start = Math.max(scope.options.pageNumber - 5, 0);
                            //ensure that it's not too far either
                            start = Math.min(maxIndex, start);

                            for (var i = start; i < (10 + start) ; i++) {
                                scope.pagination.push({
                                    val: (i + 1),
                                    isActive: scope.options.pageNumber == (i + 1)
                                });
                            }

                            //now, if the start is greater than 0 then '1' will not be displayed, so do the elipses thing
                            if (start > 0) {
                                scope.pagination.unshift({ name: "First", val: 1, isActive: false }, {val: "...",isActive: false});
                            }

                            //same for the end
                            if (start < maxIndex) {
                                scope.pagination.push({ val: "...", isActive: false }, { name: "Last", val: scope.listViewResultSet.totalPages, isActive: false });
                            }
                        }

                        scope.preValuesLoaded = true;
                    }, function(reason) {
                        notificationsService.success("Entity Load Failed:", reason.message);
                    });
                }

                function sort(field, allow) {
                    if (allow) {
                        scope.options.orderBy = field;

                        if (scope.options.orderDirection === "desc") {
                            scope.options.orderDirection = "asc";
                        }
                        else {
                            scope.options.orderDirection = "desc";
                        }
                        search();
                    }
                };

                function next () {
                    if (scope.options.pageNumber < scope.listViewResultSet.totalPages) {
                        scope.options.pageNumber++;
                        search();
                    }
                };

                function goToPage(pageNumber) {
                    scope.options.pageNumber = pageNumber + 1;
                    search();
                }


                function prev() {
                    if (scope.options.pageNumber - 1 > 0) {
                        scope.options.pageNumber--;
                        search();
                    }
                }

                function enterSearch($event) {
                    $($event.target).next().focus();
                }

                function setPageSize() {
                    scope.options.pageNumber = 1;
                    search();
                }

                function isSortDirection(col, direction) {
                    return scope.options.orderBy.toUpperCase() == col.toUpperCase() && scope.options.orderDirection == direction;
                }

                init();
            }
        }
}]);
