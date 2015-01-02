'use strict';

(function () {

    function productSelector($scope, merchelloProductService, notificationsService, dialogService, assetsService) {

        $scope.product = {};
        $scope.loaded = false;

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name loadProduct
         * @function
         * 
         * @description
         * Load the product from the product service.
         */
        $scope.loadProduct = function (key) {

            var promise = merchelloProductService.getByKey(key);

            promise.then(function (product) {

                $scope.product = new merchello.Models.Product(product);

                $scope.loaded = true;

            }, function (reason) {
                notificationsService.error("Product Load Failed", reason.message);
            });

        };

        // Load the product from the Guid key stored in the model.value
        if (_.isString($scope.model.value)) {
            if ($scope.model.value.length > 0) {
                $scope.loadProduct($scope.model.value);
            }
        }

        //--------------------------------------------------------------------------------------
        // Event Handlers
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name selectedProductFromDialog
         * @function
         * 
         * @description
         * Handles the model update after recieving the product to add from the dialog view/controller
         */
        $scope.selectedProductFromDialog = function (selectedProduct) {

            $scope.model.value = selectedProduct.key;
            $scope.product = selectedProduct;

        };

        /**
         * @ngdoc method
         * @name selectProduct
         * @function
         * 
         * @description
         * Opens the product select dialog via the Umbraco dialogService.
         */
        $scope.selectProduct = function () {

            dialogService.open({
                template: '/App_Plugins/Merchello/PropertyEditors/ProductPicker/Views/merchelloproductdialog.html',
                show: true,
                callback: $scope.selectedProductFromDialog,
                dialogData: $scope.product
            });

        };

    };

    angular.module("umbraco").controller('Merchello.PropertyEditors.MerchelloProductSelector', productSelector);

})();