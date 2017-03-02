angular.module('merchello.directives').directive('productListViewFilterOptions',
    [
    function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                value: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/product.listview.filteroptions.tpl.html',
            link: function(scope, elm, attr) {

                scope.name = {};
                scope.sku = {};
                scope.manufacturer = {};
                scope.hasManufacturers = false;
                scope.loaded = false;

                function init() {
                    scope.name = getField('name');
                    scope.sku = getField('sku');
                    scope.manufacturer = getField('manufacturer');
                    scope.hasManufacturers = scope.manufacturer.input.values.length > 0;

                }

                function getField(fieldName) {
                    if (scope.value === undefined) {
                        throw new Error('Value has not been set on the scope');
                    }

                    return _.find(scope.value.fields, function (f) {
                       if (f.field === fieldName) {
                           return f;
                       }
                    });
                }

                init();
            }
    }
}]);
