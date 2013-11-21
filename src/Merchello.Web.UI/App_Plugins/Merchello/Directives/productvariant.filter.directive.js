(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name productVariantFilter
     * @function
     * 
     * @description
     * directive for the product variants to call filter methods
     */
    directives.productVariantFilter = function ($parse) {
        return {
            restrict: 'A',
            scope: { options: '=productVariantFilter' },
            link: function ($scope, $element, $attrs, $controller) {

                var tag = $element.context.nodeName;
                //var html = ['<',tag,'>{{}}</',tag,'>'].join('');

                $scope.$watch('options', function(options) {
                    if (options) {

                        var scopeOptions = options;

                        if (scopeOptions != undefined) {
                            if (scopeOptions.length > 0) {
                                var cell = document.createElement($element.context.nodeName);
                                cell.innerText = "All";

                                $element.replaceWith(cell);

                                cell = document.createElement($element.context.nodeName);
                                cell.innerText = "None";

                                $element.after(document.createTextNode( ", " ));
                                $element.after(cell);

                                for (var i = 0; i < scopeOptions.length; i++) {
                                    for (var c = 0; c < scopeOptions[i].choices.length; c++) {

                                        var cell = document.createElement($element.context.nodeName);
                                        cell.innerText = scopeOptions[i].choices[c].name;

                                        $element.after(document.createTextNode(", "));
                                        $element.after(cell);

                                    }
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

    angular.module("umbraco").directive('productVariantFilter', merchello.Directives.productVariantFilter);

}(window.merchello.Directives = window.merchello.Directives || {}));

