(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name shippingCatalogDirective
     * @function
     * 
     * @description
     * directive to display the shipping catalog information and to provide a parent for the directives and flyouts
     */
    directives.ShippingCatalogDirective = function() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Modules/Settings/Shipping/Directives/shipping-catalog.html'
        };
    };

    angular.module("umbraco").directive('shippingCatalog', merchello.Directives.ShippingCatalogDirective);

    /**
     * @ngdoc directive
     * @name ShippingCountryDirective
     * @function
     * 
     * @description
     * directive to display country, provider, and method information and to provide a parent for the directives and flyouts
     */
    directives.ShippingCountryDirective = function() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Modules/Settings/Shipping/Directives/shipping-country.html'
        };
    };

    angular.module("umbraco").directive('shippingCountry', merchello.Directives.ShippingCountryDirective);

    /**
     * @ngdoc directive
     * @name ShippingWarehouseDirective
     * @function
     * 
     * @description
     * directive to display warehouse information
     */
    directives.ShippingWarehouseDirective = function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                warehouse: '='
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Settings/Shipping/Directives/shipping-warehouse.html'
        };
    };

    angular.module("umbraco").directive('shippingWarehouse', merchello.Directives.ShippingWarehouseDirective);

    
}(window.merchello.Directives = window.merchello.Directives || {}));

