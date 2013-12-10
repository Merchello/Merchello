'use strict';

(function () {

    function productSelector($scope, merchelloProductService) {

        $scope.loadProducts = function () {

            var promise = merchelloProductService.getAllProducts();

            promise.then(function (products) {

                $scope.products = _.map(products, function (productFromServer) {
                    return new merchello.Models.Product(productFromServer);
                });

            }, function (reason) {

                notificationsService.error("Products Selector Load Failed", reason.message);

            });

        };

        $scope.loadProducts();
    };

    angular.module("umbraco").controller('Merchello.PropertyEditors.MerchelloProductSelector', productSelector);

})();