angular.module('merchello.directives').directive('productOptionsAssociateShared',
    ['$q', 'localizationService', 'productOptionResource', 'queryDisplayBuilder', 'queryResultDisplayBuilder',
    function($q, localizationService, productOptionResource, queryDisplayBuilder, queryResultDisplayBuilder) {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                option: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/productoptions.associateshared.tpl.html',
            link: function (scope, elm, attr) {

                scope.defaultChoice = {};
                scope.wasFormSubmitted = false;
                scope.noResults = '';

                scope.optionSelected = false;
                scope.selectedChoices = [];
                scope.defaultChoice = '';

                scope.sharedOption = {};

                scope.queryResult = queryResultDisplayBuilder.createDefault();

                scope.options = {
                    currentPage: 1,
                    orderDirection: 'asc',
                    orderBy: 'name',
                    itemsPerPage: 5,
                    filter: ''
                };

                var values = '';

                scope.ready = false;

                scope.prev = function() {
                    if (scope.options.currentPage - 1 > 0) {
                        scope.options.currentPage--;
                        scope.search();
                    }
                }

                scope.next = function() {
                    if (scope.options.currentPage < scope.queryResult.totalPages) {
                        scope.options.currentPage++;
                        scope.search();
                    }
                };

                scope.enterSearch = function($event) {
                    $($event.target).next().focus();
                }

                scope.search = function() {
                    var query = queryDisplayBuilder.createDefault();
                    query.currentPage = scope.options.currentPage - 1;
                    query.itemsPerPage = scope.options.itemsPerPage;
                    query.sortBy = scope.options.orderBy;
                    query.sortDirection = scope.options.orderDirection === 'asc' ? 'Ascending' : 'Descending';
                    query.addFilterTermParam(scope.options.filter);

                    productOptionResource.searchOptions(query).then(function(results) {
                       scope.queryResult = results;
                        scope.ready = true;
                    });

                }

                scope.getColumnValue = function(propName, option) {
                    switch(propName) {
                        case 'name':
                            if (option.shared && !scope.isShared) {
                                return option.useName + ' (' + option.name + ')';
                            } else {
                                return option.name;
                            }
                        case 'values':
                            return option.choices.length + ' ' + values;
                    }
                }

                scope.removeOption = function() {
                    scope.sharedOption = {};
                    scope.selectedChoices = [];
                    scope.optionSelected = false;
                }


                scope.selectOption = function(option) {
                    scope.sharedOption = option;
                    scope.selectedChoices = [];
                    _.each(scope.sharedOption.choices, function(c) {
                       c.selected = true;
                        if (c.isDefaultChoice) {
                            scope.defaultChoice = c.key;
                        }
                    });

                    scope.optionSelected = true;
                }

                scope.toggleChoice = function(idx) {
                    scope.sharedOption.choices[idx].selected = !scope.sharedOption.choices[idx].selected;
                }

                function init() {
                    $q.all([
                        localizationService.localize('merchelloTableCaptions_optionValues'),
                        localizationService.localize('merchelloProductOptions_noSharedProductOptions')
                    ]).then(function(data) {
                        values = data[0];
                        scope.noResults = data[1];
                        scope.loaded = true;
                    });

                    scope.search();
                }


                init();
            }
        };

    }]);
