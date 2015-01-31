
    angular.module('merchello').controller('Merchello.Backoffice.ProductOptionsEditorController',
        ['$scope', '$routeParams', 'notificationsService', 'merchelloTabsFactory', 'productResource', 'settingsResource', 'productDisplayBuilder',
        function($scope, $routeParams, notificationsService, merchelloTabsFactory, productResource, settingsResource, productDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.product = productDisplayBuilder.createDefault();
            $scope.currencySymbol = '';

            // Exposed methods
            $scope.save = save;
            $scope.deleteProductDialog = deleteProductDialog;

            function init() {
                var key = $routeParams.id;
                $scope.tabs = merchelloTabsFactory.createProductEditorWithOptionsTabs(key);
                $scope.tabs.setActive('optionslist');
                loadSettings();
                loadProduct(key);
            }

            /**
             * @ngdoc method
             * @name loadProduct
             * @function
             *
             * @description
             * Load a product by the product key.
             */
            function loadProduct(key) {
                var promise = productResource.getByKey(key);
                promise.then(function (product) {
                    $scope.product = productDisplayBuilder.transform(product);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

                }, function (reason) {
                    notificationsService.error("Product Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function(currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Saves the product - used for changing the master variant name
             */
            function save(thisForm) {
                if (thisForm) {
                    if (thisForm.$valid) {
                        notificationsService.info("Saving Product...", "");
                        var promise = productResource.save($scope.product);
                        promise.then(function (product) {
                            notificationsService.success("Product Saved", "");
                            $scope.product = productDisplayBuilder.transform(product);
                        }, function (reason) {
                            notificationsService.error("Product Save Failed", reason.message);
                        });
                    }
                }
            }

            /**
             * @ngdoc method
             * @name deleteProductDialog
             * @function
             *
             * @description
             * Opens the delete confirmation dialog via the Umbraco dialogService.
             */
            function deleteProductDialog() {
                var dialogData = dialogDataFactory.createDeleteProductDialogData();
                dialogData.product = $scope.product;
                dialogData.name = $scope.product.name + ' (' + $scope.product.sku + ')';
                dialogData.warning = 'This action cannot be reversed.';

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: deleteProductDialogConfirmation,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name deleteProductDialogConfirmation
             * @function
             *
             * @description
             * Called when the Delete Product button is pressed.
             */
            function deleteProductDialogConfirmation() {
                var promiseDel = productResource.deleteProduct($scope.product);
                promiseDel.then(function () {
                    notificationsService.success("Product Deleted", "");
                    $location.url("/merchello/merchello/productlist/manage", true);
                }, function (reason) {
                    notificationsService.error("Product Deletion Failed", reason.message);
                });
            }


            // Initializes the controller
            init();
    }]);
