'use strict';

(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.PropertyDialogs.MerchelloProductDialog
     * @function
     * 
     * @description
     * The controller for the selecting products on the Umbraco dialog flyout
     */
    controllers.MerchelloProductDialog = function ($scope, merchelloProductService, notificationsService) {

        $scope.selectedProduct = {};
        $scope.products = [];

        $scope.loadProducts = function () {

            var promise = merchelloProductService.getAllProducts();

            promise.then(function (products) {

                $scope.products = _.map(products, function (productFromServer) {
                    return new merchello.Models.Product(productFromServer, true);
                });

            }, function (reason) {

                notificationsService.error("Products Dialog Load Failed", reason.message);

            });

        };

        $scope.loadProducts();
    };

    angular.module("umbraco").controller("Merchello.PropertyDialogs.MerchelloProductDialog", merchello.Controllers.MerchelloProductDialog);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
