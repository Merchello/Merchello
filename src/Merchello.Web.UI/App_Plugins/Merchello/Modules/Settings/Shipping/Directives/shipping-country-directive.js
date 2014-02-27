(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name ShippingCountryDirective
     * @function
     * 
     * @description
     * directive to display country, provider, and method information and to provide a parent for the directives and flyouts
     */
    directives.ShippingCountryDirective = function () {
        return {
            restrict: 'E',
            transclude: true,
            replace: true,
            scope: {
                country: '=',
            },
            template: '<div ng-transclude></div>',
            controller: function ($scope) {

                this.flyoutModel = { };

                this.open = function (thismodel) {
                    this.flyoutModel.open(thismodel);
                };
            }
        };
    }

    angular.module("umbraco").directive('shippingCountry', merchello.Directives.ShippingCountryDirective);

    
}(window.merchello.Directives = window.merchello.Directives || {}));

