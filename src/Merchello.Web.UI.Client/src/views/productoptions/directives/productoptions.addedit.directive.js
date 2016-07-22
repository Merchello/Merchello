angular.module('merchello.directives').directive("productOptionsAddEdit",
    ['$timeout', 'eventsService', 'productAttributeDisplayBuilder',
    function($timeout, eventsService, productAttributeDisplayBuilder) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            option: '=',
            doSave: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/productoptions.addedit.tpl.html',
        link: function (scope, elm, attr) {

            scope.contentType = {};
            scope.choiceName = '';
            scope.defaultChoice = {};
            scope.selectedAttribute = {};
            scope.wasFormSubmitted = false;

            eventsService.on('merchNewProductOptionSave', function(name, args) {
                validate(args);
            });

            // adds a choice to the dialog data collection of choicds
            scope.addChoice = function() {

                if (scope.choiceName !== '') {
                    var choice = productAttributeDisplayBuilder.createDefault();
                    choice.name = scope.choiceName;
                    choice.sku = scope.choiceName.toLocaleLowerCase();
                    choice.sortOrder = scope.option.choices.length + 1;

                    if (scope.option.choices.length === 0) {
                        choice.isDefaultChoice = true;
                        scope.defaultChoice = choice;
                        scope.selectedAttribute = choice;
                    }

                    var exists = _.find(scope.option.choices, function(c) { return c.sku === choice.sku; });
                    if (exists === undefined) {
                        scope.option.choices.push(choice);
                    }
                }

                scope.choiceName = '';
            };

            // removes a choice from the dialog data collection of choices
            scope.remove = function(idx) {
                if (scope.option.choices.length > idx) {
                    var remover = scope.option.choices[idx];
                    var sort = remover.sortOrder;
                    var wasSelected = remover.isDefaultChoice;
                    scope.option.choices.splice(idx, 1);
                    _.each(scope.option.choices, function(c) {
                        if (c.sortOrder > sort) {
                            c.sortOrder -= 1;
                        }
                    });
                    if (wasSelected) {
                        if(scope.option.choices.length > 0) {
                            scope.option.choices[0].isDefaultChoice = true;
                            scope.selectedAttribute = scope.option.choices[0];
                        }
                    }
                }
            };

            // sets the default choice property
            scope.setSelectedChoice = function(choice) {
                scope.selectedAttribute.isDefaultChoice = false;
                choice.isDefaultChoice = true;
                scope.selectedAttribute = choice;
            };


            // Saves an option

            scope.setContentType = function() {
                scope.option.detachedContentTypeKey = scope.contentType.key;
            }

            scope.sortableOptions = {
                start : function(e, ui) {
                    ui.item.data('start', ui.item.index());
                },
                stop: function (e, ui) {
                    var choice = ui.item.scope().choice;
                    var start = ui.item.data('start'),
                        end =  ui.item.index();
                    for(var i = 0; i < scope.option.choices.length; i++) {
                        scope.option.choices[i].sortOrder = i + 1;
                    }
                },
                disabled: false,
                cursor: "move"
            }

            function validate(args) {
                if (scope.productOptionForm.$valid && scope.option.choices.length > 0) {
                    args.valid = true;
                } else {
                    scope.wasFormSubmitted = true;
                }

            };
        }
    };
}]);
