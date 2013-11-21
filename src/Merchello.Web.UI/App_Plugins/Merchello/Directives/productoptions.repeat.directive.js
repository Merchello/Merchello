(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name productOptionsRepeat
     * @function
     * 
     * @description
     * directive for the product options to repeat in the header of a table
     */
    directives.productOptionsRepeat = function ($parse) {
        return {
            restrict: 'EA',
            scope: { options: '=productOptionsRepeat' },
            link: function ($scope, $element, $attrs, $controller) {

                $scope.$watch('options', function(options) {
                    if (options) {

                        var scopeOptions = options;

                        if (scopeOptions != undefined) {
                            if (scopeOptions.length > 0) {
                                var cell = document.createElement($element.context.nodeName);
                                cell.innerText = scopeOptions[0].name;

                                $element.replaceWith(cell);

                                for (var i = 1; i < scopeOptions.length; i++) {
                                    var cell = document.createElement($element.context.nodeName);
                                    cell.innerText = scopeOptions[i].name;

                                    $element.after(cell);
                                }
                            }
                        }
                        else {
                            var cell = document.createElement($element.context.nodeName);
                            cell.innerText = "";
                            $element.replaceWith(cell);
                        }

                    }
                })

            }
        };
    }

    angular.module("umbraco").directive('productOptionsRepeat', merchello.Directives.productOptionsRepeat);

}(window.merchello.Directives = window.merchello.Directives || {}));

