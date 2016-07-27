angular.module('merchello.directives').directive('productOptionsList', [
    '$q', 'localizationService', 'eventsService', 'dialogService', 'queryDisplayBuilder', 'queryResultDisplayBuilder', 'productOptionDisplayBuilder',
    function($q, localizationService, eventsService, dialogService, queryDisplayBuilder, queryResultDisplayBuilder, productOptionDisplayBuilder) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            load: '&',
            doAdd: '&',
            doEdit: '&',
            doDelete: '&',
            sharedOnly: '=?',
            showFilter: '=?',
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
            scope.isShared = false;
            scope.hasFilter = false;
            scope.queryResult = queryResultDisplayBuilder.createDefault();

            /// PRIVATE

            var yes = '';
            var no = '';
            var values = '';

            var onAdd = 'merchelloProductOptionOnAddOpen';

            scope.getColumnValue = function(propName, option) {
                switch(propName) {
                    case 'name':
                        if (option.shared && !scope.isShared) {
                            return option.useName + ' (' + option.name + ')';
                        } else {
                            return option.name;
                        }
                    case 'shared':
                        return option.shared ? yes + ' (' + option.shareCount + ')' : no;
                    case 'values':
                        return option.choices.length + ' ' + values;
                }
            }

            scope.delete = function(option) {

                var dialogData = {
                    name: '',
                    option: option
                };
                dialogData.name = option.name;

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteOption,
                    dialogData: dialogData
                });
            }

            scope.showDelete = function(option) {
                return option.shared ? option.shareCount === 0 || !scope.isShared : true;
            }

            scope.add = function() {
                var dialogData = {
                    option: productOptionDisplayBuilder.createDefault(),
                    showTabs: !scope.sharedOnly,
                    productKey: '',
                    exclude: scope.isShared ? [] : _.pluck(
                        _.filter(scope.queryResult.items,
                            function(o) {
                                if (o.shared) { return o; }
                            }), 'key')
                };

                dialogData.option.shared = scope.sharedOnly !== undefined;

                eventsService.emit(onAdd, dialogData);

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/productoption.add.html',
                    show: true,
                    callback: processAddOption,
                    dialogData: dialogData
                });
            }

            scope.edit = function(option) {
                var clone = productOptionDisplayBuilder.createDefault();
                clone = angular.extend(clone, option);

                clone.choices = _.sortBy(clone.choices, 'sortOrder');

                var dialogData = {
                    option: clone,
                    showTabs: false,
                    productKey: '',
                    sharedOptionEditor: !scope.isShared && option.shared
                }


                eventsService.emit(onAdd, dialogData);

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/productoption.edit.html',
                    show: true,
                    callback: processEditOption,
                    dialogData: dialogData
                });
            }

            scope.enterSearch = function($event) {
                $($event.target).next().focus();
            }

            scope.next = function() {
                if (scope.options.currentPage < scope.queryResult.totalPages) {
                    scope.options.currentPage++;
                    scope.search();
                }
            };

            scope.goToPage = function(pageNumber) {
                scope.options.currentPage = pageNumber + 1;
                scope.search();
            }

            scope.prev = function() {
                if (scope.options.currentPage - 1 > 0) {
                    scope.options.currentPage--;
                    scope.search();
                }
            }

            scope.sortableOptions = {
                start : function(e, ui) {
                    ui.item.data('start', ui.item.index());
                },
                stop: function (e, ui) {
                    var choice = ui.item.scope().choice;
                    var start = ui.item.data('start'),
                        end =  ui.item.index();
                    for(var i = 0; i < scope.queryResult.items.length; i++) {
                        scope.queryResult.items[i].sortOrder = i + 1;
                    }
                },
                disabled: true,
                cursor: "move"
            }

            function processDeleteOption(dialogData) {
                scope.doDelete()(dialogData.option);
            }

            function processAddOption(dialogData) {
                scope.doAdd()(dialogData.option);
            }

            function processEditOption(dialogData) {
                scope.doEdit()(dialogData.option);
            }

            function init() {

                scope.isShared = ('sharedOnly' in attr);
                scope.hasFilter = ('showFilter' in attr);

                scope.sortableOptions.disabled = scope.isShared;
                var noResultsKey = scope.isShared ? 'noSharedProductOptions' : 'noProductOptions';

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
                   } else {
                       scope.isReady = false;
                   }

                   if (scope.isReady) {
                       scope.search();
                   }
                });
            }

            scope.search = function() {

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
