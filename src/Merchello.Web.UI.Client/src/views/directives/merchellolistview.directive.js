angular.module('merchello.directives').directive('merchelloListView',
    ['$routeParams', '$log', '$filter', '$compile', 'dialogService', 'eventsService', 'localizationService', 'merchelloListViewHelper', 'queryDisplayBuilder', 'queryResultDisplayBuilder',
    function($routeParams, $log, $filter, $compile, dialogService, eventsService, localizationService, merchelloListViewHelper, queryDisplayBuilder, queryResultDisplayBuilder) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                builder: '=',
                entityType: '=',
                getColumnValue: '&',
                load: '&',
                ready: '=?',
                disableCollections: '@?',
                includeDateFilter: '@?',
                noTitle: '@?',
                noFilter: '@?',
                filterOptions: '=?',
                settingsComponent: '=?'
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
                scope.search = _.throttle(search, 750);
                scope.setPageSize = setPageSize;
                scope.openDateRangeDialog = openDateRangeDialog;

                scope.hasFilter = true;
                scope.hasCollections = true;
                scope.enableDateFilter = false;
                scope.showTitle = true;
                scope.isReady = false;

                // date filtering
                scope.clearDates = clearDates;
                scope.startDate = '';
                scope.endDate = '';
                scope.dateBtnText = '';
                var allDates = '';

                var handleChanged = "merchello.collection.changed";

                scope.config = merchelloListViewHelper.getConfig(scope.entityType);

                //scope.goToEditor = goToEditor;

                var cacheSettings = merchelloListViewHelper.cacheSettings(scope.entityType);
                var cacheEnabled = false;


                var cache = merchelloListViewHelper.cache(scope.entityType + '-listview');
                var OPTIONS_CACHE_KEY = 'options';

                scope.listViewResultSet = {
                    totalItems: 0,
                    items: []
                };

                scope.options = {
                    pageSize: scope.config.pageSize ? scope.config.pageSize : 10,
                    pageNumber: 1,
                    filter: '',
                    orderBy: (scope.config.orderBy ? scope.config.orderBy : 'name').trim(),
                    orderDirection: scope.config.orderDirection ? scope.config.orderDirection.trim() : "asc"
                };

                scope.pagination = [];

                function init() {
                    if (!('ready' in attr)) {
                        scope.isReady = true;
                    }

                    scope.hasCollections = !('disableCollections' in attr);
                    scope.enableDateFilter = 'includeDateFilter' in attr;
                    scope.hasFilter = !('noFilter' in attr);
                    scope.showTitle = !('noTitle' in attr);


                    if(scope.hasCollections) {
                        scope.collectionKey = $routeParams.id !== 'manage' ? $routeParams.id : '';
                        // none of the collections have the capability to filter by dates
                        if (scope.collectionKey !== '' && scope.enableDateFilter) {
                            scope.enableDateFilter = false;
                        }
                        OPTIONS_CACHE_KEY = OPTIONS_CACHE_KEY + scope.collectionKey;
                        cacheSettings.collectionKey = scope.collectionKey;
                    } else {
                        cacheSettings.collectionKey = '';
                    }

                    cacheEnabled = scope.collectionKey !== '' ? cacheSettings.stickyCollectionList : cacheSettings.stickyList;

                    if (cacheEnabled && cache.hasKey(OPTIONS_CACHE_KEY)) {
                        scope.options = cache.getValue(OPTIONS_CACHE_KEY);
                    }

                    if (scope.filterOptions !== undefined) {
                        // assert scope.options has filterOptions
                        if (!scope.options.hasOwnProperty('filterOptions')) {
                            scope.options.filterOptions = scope.filterOptions;
                        }
                    }

                    localizationService.localize('merchelloGeneral_allDates').then(function(value) {
                        allDates = value;
                        scope.dateBtnText = allDates;
                    });


                    scope.$watch('ready', function(newVal, oldVal) {
                        if (newVal === true) {
                            scope.isReady = newVal;
                        }
                          if(scope.isReady) {
                              search();
                          }
                    });

                    eventsService.on(handleChanged, search);

                }

                
                function search() {

                    if (cacheEnabled) {
                        cache.setValue(OPTIONS_CACHE_KEY, scope.options);
                    }


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

                    if (scope.enableDateFilter && scope.startDate !== '' && scope.endDate !== '') {
                        // just to be safe
                        var start = $filter('date')(scope.startDate, 'yyyy-MM-dd');
                        var end = $filter('date')(scope.endDate, 'yyyy-MM-dd');
                        query.addInvoiceDateParam(start, 'start');
                        query.addInvoiceDateParam(end, 'end');

                        scope.dateBtnText = scope.startDate + ' - ' + scope.endDate;
                    }

                    scope.load()(query, scope.options.filterOptions).then(function (response) {
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

                function clearDates() {
                    scope.startDate = '';
                    scope.endDate = '';
                    scope.dateBtnText = allDates;
                    search();
                }

                function openDateRangeDialog() {
                    var dialogData = {
                        startDate: scope.startDate,
                        endDate: scope.endDate,
                        showPreDeterminedDates: true
                    };

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/daterange.selection.html',
                        show: true,
                        callback: processDateRange,
                        dialogData: dialogData
                    });
                }

                function processDateRange(dialogData) {
                    scope.startDate = dialogData.startDate;
                    scope.endDate = dialogData.endDate;
                    search();
                }

                scope.openSettings = function() {

                    var component = buildFilterOptionComponent();

                    var dialogData = {
                        settings: cacheSettings,
                        entityType: scope.entityType,
                        settingsComponent: component
                    };

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/listview.settings.html',
                        show: true,
                        callback: processListViewSettings,
                        dialogData: dialogData
                    });
                }

                function processListViewSettings(dialogData) {
                    cacheSettings = merchelloListViewHelper.cacheSettings(scope.entityType, dialogData.settings);
                    cacheEnabled = scope.collectionKey !== '' ? cacheSettings.stickyCollectionList : cacheSettings.stickyList;
                    search();
                }

                function buildFilterOptionComponent() {
                    if (scope.settingsComponent !== undefined && scope.options.filterOptions !== undefined) {
                        var htm = "<" + scope.settingsComponent + " value='options.filterOptions'></" + scope.settingsComponent + ">";
                        return $compile(htm)(scope);
                    }
                    return undefined;
                }


                init();
            }
        }
}]);
