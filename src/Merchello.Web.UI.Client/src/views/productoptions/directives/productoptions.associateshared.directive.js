angular.module('merchello.directives').directive('productOptionsAssociateShared',
    ['$q', 'localizationService', 'eventsService', 'productOptionResource', 'queryDisplayBuilder', 'queryResultDisplayBuilder',
        'productOptionDisplayBuilder',
    function($q, localizationService, eventsService, productOptionResource, queryDisplayBuilder, queryResultDisplayBuilder,
        productOptionDisplayBuilder) {

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

                scope.defaultChoice = {
                    current: undefined,
                    previous: undefined,
                    isSet: false
                };

                scope.sharedOption = {};

                scope.queryResult = queryResultDisplayBuilder.createDefault();

                scope.options = {
                    currentPage: 1,
                    orderDirection: 'asc',
                    orderBy: 'name',
                    itemsPerPage: 5,
                    filter: ''
                };

                scope.ready = false;

                var values = '';

                eventsService.on('merchSharedProductOptionSave', function(name, args) {
                    validate(args);
                });


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
                        scope.selectedChoices.push(c.key);

                        if (c.isDefaultChoice) {

                            scope.defaultChoice.current = c;
                            scope.defaultChoice.previous = c;
                            scope.defaultChoice.isSet = true;
                        }
                    });

                    scope.optionSelected = true;
                }

                scope.toggleChoice = function(idx) {
                    scope.sharedOption.choices[idx].selected = !scope.sharedOption.choices[idx].selected;
                }

                scope.addRemoveChoice = function(choice) {

                    var fnd = _.find(scope.selectedChoices, function(sc) { return sc === choice.key;});
                    if (fnd) {
                        scope.selectedChoices = _.reject(scope.selectedChoices, function(c) { return c === choice.key; });
                        if (choice.isDefaultChoice) {
                            // reset default choice
                            scope.defaultChoice.choice = undefined;
                            scope.defaultChoice.previous = undefined;
                            scope.defaultChoice.isSet = false;
                            choice.isDefaultChoice = false;
                            ensureDefaultChoice();
                        }
                    } else {

                        scope.selectedChoices.push(choice.key);
                        // sets the default choice in the case where the selectedChoices array was originally empty
                        ensureDefaultChoice();
                    }

                }

                // validates that a choice can be selected
                scope.validateDefaultChoice = function() {
                    if (ensureDefaultChoice()) {
                        var fnd = _.find(scope.selectedChoices, function(sc) { return sc === scope.defaultChoice.current.key;});
                        if (fnd) {
                            scope.defaultChoice.previous.isDefaultChoice = false;
                            scope.defaultChoice.previous = scope.defaultChoice.current;
                        } else {
                            scope.defaultChoice.current = scope.defaultChoice.previous;
                        }
                        scope.defaultChoice.current.isDefaultChoice = true;
                    } else {
                        // should never get here as there should be no options rendered
                        var err = new Error('defaultChoice has has not been set and there was an attempt to change the default choice.  This should not be possible!');
                        throw err;
                    }
                }

                // ensures an option choice is available to be selected.
                function ensureDefaultChoice() {
                    // assert there is a default
                    if (scope.defaultChoice.isSet) {
                        return true;
                    }

                    if (!scope.defaultChoice.isSet && scope.selectedChoices.length > 0) {
                        // handles legacy no previous default setting

                        // find the first selected choice
                        var choice = _.find(scope.sharedOption.choices, function(c) {
                           var exists = _.find(scope.selectedChoices, function(key) {
                             return key === c.key;
                           });
                            if (exists) {
                                return c;
                            }
                        });

                        if (choice) {
                            //var choice = scope.sharedOption.choices[0];
                            scope.defaultChoice.current = choice;
                            scope.defaultChoice.previous = choice;
                            scope.defaultChoice.isSet = true;

                            _.each(scope.sharedOption.choices, function(c) {
                                c.isDefaultChoice = c.key === choice.key;
                            });

                            return true;
                        }
                    }

                    return false;
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

                function createAssociatedOption() {
                    var option = angular.extend(scope.option, scope.sharedOption);

                    option.choices = _.filter(option.choices, function(oc) {
                       var exists = _.find(scope.selectedChoices, function (sc) {
                          return sc === oc.key;
                       });
                        if (exists) {
                            return oc;
                        }
                    });

                }


                function validate(args) {
                    if (scope.productOptionForm.$valid) {
                        scope.option = createAssociatedOption();
                        args.valid = true;
                    } else {
                        scope.wasFormSubmitted = true;
                    }

                };

                init();
            }
        };

    }]);
