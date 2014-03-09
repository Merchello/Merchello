'use strict';

(function () {

    function productSelector($scope, merchelloProductService, notificationsService, dialogService, assetsService) {

        // TODO: this doesn't seem to hook up the angular correctly.  Check with Per if this is possible.
        // load the dialog controller javascript on demand, then load the product if it worked.
        //assetsService.loadJs("/App_Plugins/Merchello/PropertyEditors/ProductPicker/Js/merchelloproductdialog.controller.js").then(
        //    function () {
        //        if (_.isString($scope.model.value)) {
        //            if ($scope.model.value.length > 0) {
        //                $scope.loadProduct($scope.model.value);
        //            }
        //        }
        //    });

        $scope.product = {};
        $scope.loaded = false;

        $scope.loadProduct = function (key) {

            var promise = merchelloProductService.getByKey(key);

            promise.then(function (product) {

                $scope.product = new merchello.Models.Product(product);

                $scope.loaded = true;

            }, function (reason) {
                notificationsService.error("Product Load Failed", reason.message);
            });

        };

        $scope.selectedProductFromDialog = function (selectedProduct) {

            $scope.model.value = selectedProduct.key;
            $scope.product = selectedProduct;

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
            }
        }

    };

    angular.module("umbraco").controller('Merchello.PropertyEditors.MerchelloProductSelector', productSelector);

})();