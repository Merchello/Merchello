    /**
     * @ngdoc directive
     * @name tagsManager
     * @function
     *
     * @description
     * directive for a tags manager.
     */
    angular.module('merchello.directives').directive('tagManager', function() {
        return {
            restrict: 'E',
            scope: { option: '=' },
            template:
            '<div class="tags">' +
            '<a ng-repeat="(idx, choice) in option.choices | orderBy:\'sortOrder\'" class="tag" ng-click="remove(choice.key)">{{choice.name}}</a>' +
            '</div>' +
            '<input type="text" placeholder="Add a choice..." ng-model="newChoiceName" /> ' +
            '<merchello-add-icon do-add="add()"></merchello-add-icon>',
            link: function (scope, $element) {

                var input = angular.element($element.children()[1]);
                
                // This adds the new tag to the tags array
                scope.add = function () {
                    if (scope.newChoiceName.length > 0) {
                        scope.option.addAttributeChoice(scope.newChoiceName);
                        scope.newChoiceName = "";
                    }
                };

                // This is the ng-click handler to remove an item
                scope.remove = function (key) {
                   scope.option.choices = _.reject(scope.option.choices, function(c) {
                      return c.key === key;
                   });
                };

                // Capture all keypresses
                input.bind('keypress', function (event) {
                    // But we only care when Enter was pressed
                    if (event.keyCode == 13) {
                        // There's probably a better way to handle this...
                        scope.add();
                    }
                });



            }
        };
    });
