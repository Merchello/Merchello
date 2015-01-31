    /**
     * @ngdoc controller
     * @name Merchello.PropertyEditors.MerchelloProductSelectorController
     * @function
     *
     * @description
     * The controller for product product selector property editor view
     */
    angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloProductSelectorController',
        ['$scope', 'notificationsService', 'dialogService', 'assetsService', 'dialogDataFactory', 'productResource', 'settingsResource', 'productDisplayBuilder',
        function($scope, notificationsService, dialogService, assetsService, dialogDataFactory, productResource, settingsResource, productDisplayBuilder) {

            $scope.product = {};
            $scope.currencySymbol = '';
            $scope.loaded = false;

            // exposed methods
            $scope.selectProduct = selectProduct;
            $scope.disableProduct = disableProduct;

            //--------------------------------------------------------------------------------------
            // Initialization methods
            //--------------------------------------------------------------------------------------
            // Load the product from the Guid key stored in the model.value
            if (_.isString($scope.model.value)) {
                loadSettings();
                if ($scope.model.value.length > 0) {
                    loadProduct($scope.model.value);
                }
            }

            /**
             * @ngdoc method
             * @name loadProduct
             * @function
             *
             * @description
             * Load the product from the product service.
             */
            function loadProduct(key) {
                var promise = productResource.getByKey(key);
                promise.then(function (product) {
                    $scope.product = productDisplayBuilder.transform(product);
                    $scope.loaded = true;
                }, function (reason) {
                    notificationsService.error("Product Load Failed", reason.message);
                });
            }

            function loadSettings() {
                var promise = settingsResource.getCurrencySymbol();
                promise.then(function(symbol) {
                    $scope.currencySymbol = symbol;
                }, function (reason) {
                    notificationsService.error('Could not retrieve currency symbol', reason.message);
                });
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
            function selectedProductFromDialog(dialogData) {
                $scope.model.value = dialogData.product.key;
                $scope.product = dialogData.product;
            }

            /**
             * @ngdoc method
             * @name selectProduct
             * @function
             *
             * @description
             * Opens the product select dialog via the Umbraco dialogService.
             */
            function selectProduct() {
                var dialogData = dialogDataFactory.createProductSelectorDialogData();
                dialogData.product = $scope.product;
                dialogService.open({
                    template: '/App_Plugins/Merchello/propertyeditors/productpicker/merchello.productselector.dialog.html',
                    show: true,
                    callback: selectedProductFromDialog,
                    dialogData: dialogData
                });
            }

            function disableProduct() {
                $scope.model.value = '';
                $scope.product = {};
            }

    }]);
