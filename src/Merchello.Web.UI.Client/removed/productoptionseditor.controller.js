
    angular.module('merchello').controller('Merchello.Backoffice.ProductOptionsEditorController',
        ['$scope', '$routeParams', '$location', '$timeout', 'notificationsService', 'dialogService', 'merchelloTabsFactory', 'dialogDataFactory', 'productResource', 'settingsResource', 'productDisplayBuilder', 'localizationService',
        function($scope, $routeParams, $location, $timeout, notificationsService, dialogService, merchelloTabsFactory, dialogDataFactory, productResource, settingsResource, productDisplayBuilder, localizationService) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.product = productDisplayBuilder.createDefault();
            $scope.currencySymbol = '';

            // Exposed methods
            $scope.save = save;
            $scope.deleteProductDialog = deleteProductDialog;

            function init() {

                var key = $routeParams.id;
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
                    setTabs();
                    console.info($scope.product.productOptions);
                }, function (reason) {
                    notificationsService.error("Product Load Failed", reason.message);
                });
            }

            function setTabs() {
                $scope.tabs = merchelloTabsFactory.createProductEditorTabs($scope.product.key, $scope.product.hasVariants());
                $scope.tabs.hideTab('productcontent');
                $scope.tabs.setActive('optionslist');
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
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
                // TODO we should unbind the return click event
                // so that we can quickly add the options and remove the following
                if(thisForm === undefined) {
                    return;
                }
                if (thisForm.$valid) {
                    notificationsService.info("Saving Product...", "");

                    var promise = productResource.save($scope.product);
                    promise.then(function (product) {
                        notificationsService.success("Product Saved", "");
                        $scope.product = productDisplayBuilder.transform(product);
                        setTabs();
                    }, function (reason) {
                        notificationsService.error("Product Save Failed", reason.message);
                    });
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
                dialogData.warning = localizationService.localize('merchelloDelete_actionNotReversible');

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
