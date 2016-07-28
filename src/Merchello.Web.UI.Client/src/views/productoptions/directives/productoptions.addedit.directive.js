angular.module('merchello.directives').directive("productOptionsAddEdit",
    ['$timeout', 'eventsService', 'dialogService', 'productOptionResource', 'productAttributeDisplayBuilder',
    function($timeout, eventsService, dialogService, productOptionResource, productAttributeDisplayBuilder) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            option: '=',
            doSave: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/productoptions.addedit.tpl.html',
        link: function (scope, elm, attr) {

            scope.contentType = undefined;
            scope.choiceName = '';
            scope.wasFormSubmitted = false;
            scope.ready = false;
            scope.counts = undefined;

            scope.selectedAttribute = {
                current: undefined,
                previous: undefined,
                isSet: false
            }

            if (scope.option.key === '') {
                scope.ready = true;
            } else {
                 productOptionResource.getUseCounts(scope.option).then(function(counts) {
                     scope.counts = counts;

                     scope.ready = true;
                 });
            }


            if (scope.option.choices.length > 0) {
                scope.selectedAttribute.current = _.find(scope.option.choices, function(c) {
                   if (c.isDefaultChoice) {
                       return c;
                   }
                });

                if (scope.selectedAttribute.current) {
                    scope.selectedAttribute.previous = scope.selectedAttribute.current;
                } else {
                    scope.selectedAttribute.current = scope.selectedAttribute.previous = scope.option.choices[0];
                }
            }

            eventsService.on('merchNewProductOptionSave', function(name, args) {
                validate(args);
            });



            // adds a choice to the dialog data collection of choicds
            scope.addChoice = function() {

                if (scope.choiceName !== '') {
                    var choice = productAttributeDisplayBuilder.createDefault();
                    choice.name = scope.choiceName;
                    choice.sku = scope.choiceName.replace(/\W+/g, " ").replace(/\s+/g, '-').toLocaleLowerCase();
                    choice.sortOrder = scope.option.choices.length + 1;

                    if (scope.option.choices.length === 0) {
                        choice.isDefaultChoice = true;
                        scope.selectedAttribute.current = scope.selectedAttribute.previous = choice;
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
                            scope.selectedAttribute.current = scope.selectedAttribute.previous = scope.option.choices[0];
                        }
                    }
                }
            };

            scope.showDelete = function(choice) {
              if (scope.counts) {
                  var fnd = _.find(scope.counts.choices, function(cc) {
                      return cc.key === choice.key;
                  });
                  if (fnd) {
                      return fnd.useCount === 0;
                  } else {
                      return true;
                  }
              } else {
                  return false;
              }
            };


            // sets the default choice property
            scope.setSelectedChoice = function() {
                scope.selectedAttribute.previous.isDefaultChoice = false;
                scope.selectedAttribute.current.isDefaultChoice = true;
                scope.selectedAttribute.previous = scope.selectedAttribute.current;
            };


            scope.addDetachedContent = function(choice) {
                var dialogData = {
                    choice: choice,
                    contentType: scope.contentType
                };

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/productoption.choicecontent.html',
                    show: true,
                    callback: void(0),
                    dialogData: dialogData
                });
            }

            // Saves an option

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
                    if (scope.contentType) {
                        scope.option.detachedContentTypeKey = scope.contentType.key;
                    } else {
                        scope.option.detachedContentTypeKey = '';
                    }

                    args.valid = true;

                } else {
                    scope.wasFormSubmitted = true;
                }
            };


        }
    };
}]);
