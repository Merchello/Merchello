(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name dropdownList
     * @function
     * 
     * @description
     * directive for the shipping country dropdown
     */

    directives.DropdownListDirective = function () {
        return {
            restrict: 'E',
            scope: {
                selected: '=',
                countries: '='
            },
            template: '<div class="dropdown-list">'
                + '<input name="country-name" class="col-xs-6 span6" type="text" placeholder="{{placeholderString}}" data-ng-model="typedInput" />'
                + '<div class="btn-group"><button class="btn dropdown-toggle" data-ng-click="toggle()"><i data-ng-class="{ \'icon-navigation-down\': !visible, \'icon-navigation-up\': visible }"></i></button></div>'
                + '<ul class="col-xs-6 span6 options" data-ng-class="{ \'open\': visible, \'closed\': !visible }">'
                + '<li data-ng-repeat="choice in countries | filter: typedInput"><a data-ng-click="select(choice)">{{choice.name}}</a></li>'
                + '</ul>'
                + '</div>'
            ,
            link: function ($scope, $element, $attrs) {

                // Note From Kyle: There's probably a better way to do this, but using it for now to avoid headaches.
                var input = angular.element($element.find('input'));

                $scope.placeholderString = $attrs.placeholder;
                $scope.typedInput = "";

                $scope.toggle = function () {
                    if (!$scope.visible) {
                        $scope.visible = true;
                    } else {
                        $scope.visible = false;
                    }
                }

                $scope.select = function (choice) {
                    $scope.typedInput = choice.name;
                    $scope.selected = choice;
                    $scope.toggle();
                };

                input.focus(function (event) {
                    if (!$scope.visible) {
                        $scope.visible = true;
                    }
                });
            }
        };
    }

    angular.module("umbraco").directive('dropdownList', merchello.Directives.DropdownListDirective);

}(window.merchello.Directives = window.merchello.Directives || {}));