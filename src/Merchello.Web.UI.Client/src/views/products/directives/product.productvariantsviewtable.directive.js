/**
 * @ngdoc controller
 * @name productVariantsViewTable
 * @function
 *
 * @description
 * The productVariantsViewTable directive
 */
angular.module('merchello.directives').directive('productVariantsViewTable', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            product: '=',
            currencySymbol: '=',
            reload: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/product.productvariantsviewtable.tpl.html',
        controller: 'Merchello.Directives.ProductVariantsViewTableDirectiveController'
    };
});
