(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name tagManager
     * @function
     * 
     * @description
     * directive for the product options
     */
    directives.TagManagerDirective = function () {
        return {
            restrict: 'E',
            scope: { option: '=' },
            template:
                '<div class="tags">' +
                    '<a ng-repeat="(idx, choice) in option.choices" class="tag" ng-click="remove(idx)">{{choice.name}}</a>' +
                '</div>' +
                '<input type="text" placeholder="Add a choice..." ng-model="new_value"></input> ' +
                '<a class="btn btn-primary" ng-click="add()">Add</a>',
            link: function ($scope, $element) {
                // FIXME: this is lazy and error-prone
                var input = angular.element($element.children()[1]);

                // This adds the new tag to the tags array
                $scope.add = function () {
                    if ($scope.new_value.length > 0) {
                        $scope.option.addChoice($scope.new_value);
                        $scope.new_value = "";
                    }
                };

                // This is the ng-click handler to remove an item
                $scope.remove = function (idx) {
                    $scope.option.removeChoice(idx);
                };

                // Capture all keypresses
                input.bind('keypress', function (event) {
                    // But we only care when Enter was pressed
                    if (event.keyCode == 13) {
                        // There's probably a better way to handle this...
                        $scope.$apply($scope.add);
                    }
                });
            }
        };
    }

    angular.module("umbraco").directive('tagManager', merchello.Directives.TagManagerDirective);

}(window.merchello.Directives = window.merchello.Directives || {}));

