'use strict';

(function () {

    function productEditor($scope, $routeParams, merchelloProductService, notificationsService, dialogService) {

        $scope.product = new merchello.Models.Product();
        $scope.productVariant = new merchello.Models.ProductVariant();
        $scope.loaded = false;

        $scope.loadProduct = function (key) {

            var promise = merchelloProductService.getByKey(key);

            promise.then(function (product) {

                $scope.product = new merchello.Models.Product(product);
                $scope.productVariant.copyFromProduct($scope.product);

                $scope.loaded = true;

            }, function (reason) {
                notificationsService.error("Product Load Failed", reason.message);
            });

        };

        $scope.selectedProductFromDialog = function (selectedProduct) {

            $scope.model.value = selectedProduct.key;
            $scope.loadProduct($scope.model.value);

        };

        $scope.selectProduct = function () {

            dialogService.open({
                template: '/App_Plugins/Merchello/PropertyEditors/ProductPicker/Views/merchelloproductdialog.html',
                show: true,
                callback: $scope.selectedProductFromDialog,
                dialogData: $scope.product
            });

        };

        // Load the product from the Guid key stored in the model.value
        if (_.isString($scope.model.value)) {
            if ($scope.model.value.length > 0) {
                $scope.loadProduct($scope.model.value);
                $scope.creatingProduct = false;
                $scope.creatingVariant = false;
                $scope.editingVariant = false;
            } else {
                $scope.creatingProduct = true;
                $scope.creatingVariant = false;
                $scope.editingVariant = false;
            }
        }


        function isCreating() {
            return $routeParams.create;
        }
    };

    angular.module("umbraco").controller('Merchello.PropertyEditors.MerchelloProductEditor', ['$scope', '$routeParams', 'merchelloProductService', 'notificationsService', 'dialogService', productEditor]);

})();