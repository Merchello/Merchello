angular.module('merchello.directives').directive('productOptionsAssociateShared',
    ['$q', '$timeout', 'localizationService', 'eventsService', 'productOptionResource', 'queryDisplayBuilder', 'queryResultDisplayBuilder',
        'productOptionDisplayBuilder',
    function($q, $timeout, localizationService, eventsService, productOptionResource, queryDisplayBuilder, queryResultDisplayBuilder,
        productOptionDisplayBuilder) {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                option: '=',
                exclude: '=?',
                sharedOptionsEditor: '=?'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/productoptions.associateshared.tpl.html',
            link: function (scope, elm, attr) {

                scope.defaultChoice = {};
                scope.wasFormSubmitted = false;
                scope.noResults = '';
                scope.ready = false;

                scope.optionSelected = false;
                scope.selectedChoices = [];
                scope.pagination = [];

                scope.defaultChoice = {
                    current: undefined,
                    previous: undefined,
                    isSet: false
                };

                scope.productOption = {};
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

                eventsService.on('merchSharedProductOptionSave', function(name, args) {
                    validate(args);
                });


                // init shared option editor
                if(('sharedOptionsEditor' in attr) && scope.sharedOptionsEditor) {

                    scope.productOption = scope.option;
                    productOptionResource.getByKey(scope.option.key).then(function(po) {
                        po.sortOrder = scope.option.sortOrder;
                        po.useName = scope.option.useName;
                       scope.sharedOption = po;

                        scope.optionSelected = true;

                        _.each(scope.sharedOption.choices, function(soc) {

                            // find the previously selected choice
                            var fnd = _.find(scope.productOption.choices, function(poc) {
                                return poc.key === soc.key;
                            });


                            if (fnd) {
                                soc.selected = true;
                                scope.selectedChoices.push(soc.key);
                                if (fnd.isDefaultChoice) {
                                    scope.defaultChoice.current = soc;
                                    scope.defaultChoice.previous = soc;
                                    scope.defaultChoice.isSet = true;
                                }
                                ensureDefaultChoice();
                            } else {
                                soc.selected = false;
                            }
                        });

                        scope.ready = true;
                    });
                } else {
                    scope.ready = true;
                }


                scope.prev = function() {
                    if (scope.options.currentPage - 1 > 0) {
                        scope.options.currentPage--;
                        scope.search();
                    }
                }

                scope.goToPage = function(pageNumber) {
                    scope.options.currentPage = pageNumber + 1;
                    scope.search();
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

                scope.showAdd = function(option) {
                    if (scope.exclude) {
                        var fnd = _.find(scope.exclude, function(key) {
                           return key === option.key;
                        });
                        return !fnd;
                    }
                    return true;
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

                    // if a choice has been previously specified to be the default for the product,
                    // then set this as the default 
                    if (typeof scope.productOption !== "undefined") {
                        if (typeof scope.productOption.choices !== "undefined") {
                            if (scope.productOption.choices.length > 0) {

                                // find the first selected choice
                                var productDefaultChoice = _.find(scope.productOption.choices, function(c) {
                                    var exists = _.find(scope.selectedChoices, function() {
                                        return c.isDefaultChoice === true;
                                    });
                                    if (exists) {
                                        return c;
                                    }
                                });

                                if (productDefaultChoice) {
                                    //var choice = scope.sharedOption.choices[0];
                                    scope.defaultChoice.current = productDefaultChoice;
                                    scope.defaultChoice.previous = productDefaultChoice;
                                    scope.defaultChoice.isSet = true;

                                    _.each(scope.sharedOption.choices, function(c) {
                                        c.isDefaultChoice = c.key === productDefaultChoice.key;
                                    });

                                    return true;
                                }
                            }
                        }
                    }

                    // if a choice has not previously been specified to be the default,
                    // then use the default as specified in the option settings as the default 
                    if (scope.selectedChoices.length > 0) {
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

                    // todo - hashkeys sometimes don't map here
                    var option = angular.extend(scope.option, scope.sharedOption);


                    option.choices = _.filter(option.choices, function(oc) {
                       var exists = _.find(scope.selectedChoices, function (sc) {
                          return sc === oc.key;
                       });

                        if (exists) {
                            // wait for the hashkeys
                            return oc;
                        }
                    });


                    return option;
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
