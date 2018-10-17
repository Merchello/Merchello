    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ProductEditWithOptionsController
     * @function
     *
     * @description
     * The controller for product edit with options view
     */
    angular.module('merchello').controller('Merchello.Backoffice.ProductEditWithOptionsController',
        ['$scope', '$routeParams', '$timeout', '$location', '$q', 'assetsService', 'notificationsService', 'dialogService', 'serverValidationManager',
            'merchelloTabsFactory', 'dialogDataFactory', 'productResource', 'settingsResource', 'productDisplayBuilder', 'localizationService',
        function($scope, $routeParams, $timeout, $location, $q, assetsService, notificationsService, dialogService, serverValidationManager,
            merchelloTabsFactory, dialogDataFactory, productResource, settingsResource, productDisplayBuilder, localizationService) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.product = productDisplayBuilder.createDefault();
            $scope.currencySymbol = '';
            $scope.reorderVariants = false;
            $scope.hideClose = true;

            // exposed methods
            $scope.save = save;
            $scope.openCopyProductDialog = openCopyProductDialog;
            $scope.deleteProductDialog = deleteProductDialog;
            $scope.init = init;

            function init() {
                var key = $routeParams.id;
                $scope.tabs = merchelloTabsFactory.createProductEditorWithOptionsTabs(key);
                $scope.tabs.hideTab('productcontent');
                $scope.tabs.setActive('variantlist');
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

            //--------------------------------------------------------------------------------------
            // Event Handlers
            //--------------------------------------------------------------------------------------

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
                        notificationsService.info(localizationService.localize("merchelloStatusNotifications_productSaveInProgress"), "");
                        var promise = productResource.save($scope.product);
                        promise.then(function (product) {
                            notificationsService.success(localizationService.localize("merchelloStatusNotifications_productSaveSuccess"), "");
                            $scope.product = productDisplayBuilder.transform(product);
                        }, function (reason) {
                            notificationsService.error(localizationService.localize("merchelloStatusNotifications_productSaveError"), reason.message);
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
                    notificationsService.success(localizationService.localize("merchelloStatusNotifications_productDeleteSuccess"), "");
                    $location.url("/merchello/merchello/productlist/manage", true);
                }, function (reason) {
                    notificationsService.error(localizationService.localize("merchelloStatusNotifications_productDeleteError"), reason.message);
                });
            }

            function openCopyProductDialog() {
                var dialogData = {
                    product: $scope.product,
                    name: '',
                    sku: ''
                };
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/product.copy.html',
                    show: true,
                    callback: processCopyProduct,
                    dialogData: dialogData
                });
            }


            function processCopyProduct(dialogData) {
                productResource.copyProduct(dialogData.product, dialogData.name, dialogData.sku).then(function(result) {
                    notificationsService.success("Product copied");
                    $timeout(function() {
                        $location.url("/merchello/merchello/productedit/" + result.key);
                    }, 1000);
                });
            }



            // Initialize the controller
            init();
    }]);