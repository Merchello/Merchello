    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ProductEditController
     * @function
     *
     * @description
     * The controller for product edit view
     */
    angular.module('merchello').controller('Merchello.Backoffice.ProductEditController',
        ['$scope', '$routeParams', '$window', '$location', '$timeout', 'assetsService', 'notificationsService', 'dialogService', 'merchelloTabsFactory', 'dialogDataFactory',
            'serverValidationManager', 'productResource', 'warehouseResource', 'settingsResource',
            'productDisplayBuilder', 'productVariantDisplayBuilder', 'warehouseDisplayBuilder', 'settingDisplayBuilder', 'catalogInventoryDisplayBuilder', 'localizationService',
        function($scope, $routeParams, $window, $location, $timeout, assetsService, notificationsService, dialogService, merchelloTabsFactory, dialogDataFactory,
            serverValidationManager, productResource, warehouseResource, settingsResource,
            productDisplayBuilder, productVariantDisplayBuilder, warehouseDisplayBuilder, settingDisplayBuilder, catalogInventoryDisplayBuilder, localizationService) {

            //--------------------------------------------------------------------------------------
            // Declare and initialize key scope properties
            //--------------------------------------------------------------------------------------
            //

            // To help umbraco directives show our page
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.entityType = 'product';

            // settings - contains defaults for the checkboxes
            $scope.settings = {};

            // this is for the slide panel directive to get rid of the close button since we'll
            // be handling it in a different way in this case
            $scope.hideClose = true;

            $scope.product = productDisplayBuilder.createDefault();
            $scope.productVariant = productVariantDisplayBuilder.createDefault();
            $scope.warehouses = [];
            $scope.defaultWarehouse = {};
            $scope.context = 'createproduct';

            // Exposed methods
            $scope.save = save;
            $scope.openCopyProductDialog = openCopyProductDialog;
            $scope.loadAllWarehouses = loadAllWarehouses;
            $scope.deleteProductDialog = deleteProductDialog;


            //--------------------------------------------------------------------------------------
            // Initialization methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                var key = $routeParams.id;
                var productVariantKey = $routeParams.variantid;
                loadSettings();
                loadAllWarehouses(key, productVariantKey);
            }

            /**
             * @ngdoc method
             * @name loadAllWarehouses
             * @function
             *
             * @description
             * Loads in default warehouse and all other warehouses from server into the scope.  Called in init().
             */
            function loadAllWarehouses(key, productVariantKey) {
                var promiseWarehouse = warehouseResource.getDefaultWarehouse();
                promiseWarehouse.then(function (warehouse) {
                    $scope.defaultWarehouse = warehouseDisplayBuilder.transform(warehouse);
                    $scope.warehouses.push($scope.defaultWarehouse);
                    loadProduct(key, productVariantKey);
                }, function (reason) {
                    notificationsService.error("Default Warehouse Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadProduct
             * @function
             *
             * @description
             * Load a product by the product key.
             */
            function loadProduct(key, productVariantKey) {
                if (key === 'create' || key === '' || key === undefined) {
                    $scope.context = 'createproduct';
                    $scope.productVariant = $scope.product.getMasterVariant();
                    $scope.tabs = merchelloTabsFactory.createNewProductEditorTabs();
                    $scope.tabs.setActive($scope.context);
                    $scope.preValuesLoaded = true;
                    return;
                }
                var promiseProduct = productResource.getByKey(key);
                promiseProduct.then(function (product) {
                    $scope.product = productDisplayBuilder.transform(product);
                    if(productVariantKey === '' || productVariantKey === undefined) {
                        // this is a product edit.
                        // we use the master variant context so that we can use directives associated with variants
                        $scope.productVariant = $scope.product.getMasterVariant();
                        $scope.context = 'productedit';

                        if (!$scope.product.hasVariants()) {
                            $scope.tabs = merchelloTabsFactory.createProductEditorTabs(key);
                        }
                        else
                        {
                            $scope.tabs = merchelloTabsFactory.createProductEditorWithOptionsTabs(key);
                        }

                    } else {
                        // this is a product variant edit
                        // in this case we need the specific variant
                        $scope.productVariant = $scope.product.getProductVariant(productVariantKey);
                        $scope.context = 'varianteditor';
                        $scope.tabs = merchelloTabsFactory.createProductVariantEditorTabs(key, productVariantKey);
                    }
                    
                    $scope.preValuesLoaded = true;
                    $scope.tabs.setActive($scope.context);
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
             * Loads in store settings from server into the scope.  Called in init().
             */
            function loadSettings() {
                var promiseSettings = settingsResource.getAllSettings();
                promiseSettings.then(function(settings) {
                    $scope.settings = settingDisplayBuilder.transform(settings);
                    $scope.loaded = true;
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
             * Called when the Save button is pressed.  See comments below.
             */
            function save(thisForm) {
                // TODO we should unbind the return click event
                // so that we can quickly add the options and remove the following
                if(thisForm === undefined) {
                    return;
                }
                if (thisForm.$valid) {
                    $scope.preValuesLoaded = false;
                    // convert the extended data objects to an array
                    switch ($scope.context) {
                        case "productedit":
                            // Copy from master variant
                            var productOptions = $scope.product.productOptions;
                            $scope.productVariant.ensureCatalogInventory();
                            $scope.product = $scope.productVariant.getProductForMasterVariant();
                            $scope.product.productOptions = productOptions;
                            saveProduct();
                            break;
                        case "varianteditor":
                            saveProductVariant();
                            break;
                        default:
                            var productOptions = $scope.product.productOptions;
                            $scope.product = $scope.productVariant.getProductForMasterVariant();
                            $scope.product.productOptions = productOptions;
                            createProduct();
                            break;
                    }
                }
            }

            /**
             * @ngdoc method
             * @name createProduct
             * @function
             *
             * @description
             * Creates a product.  See comments below.
             */
            function createProduct() {
                var promise = productResource.add($scope.product);
                promise.then(function (product) {
                    notificationsService.success(localizationService.localize("merchelloStatusNotifications_productSaveSuccess"), "");
                    $scope.product = productDisplayBuilder.transform(product);
                    // short pause to make sure examine index has a chance to update
                    $timeout(function() {
                       if ($scope.product.hasVariants()) {
                            $location.url("/merchello/merchello/producteditwithoptions/" + $scope.product.key, true);
                        } else {
                            $location.url("/merchello/merchello/productedit/" + $scope.product.key, true);
                        }
                    }, 400);
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error(localizationService.localize("merchelloStatusNotifications_productSaveError"), reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name saveProduct
             * @function
             *
             * @description
             * Saves a product.  See comments below.
             */
            function saveProduct() {
                var promise = productResource.save($scope.product);
                promise.then(function (product) {
                    notificationsService.success(localizationService.localize("merchelloStatusNotifications_productSaveSuccess"), "");
                    $scope.product = productDisplayBuilder.transform(product);
                    $scope.productVariant = $scope.product.getMasterVariant();

                     //if ($scope.product.hasVariants()) {
                    // short pause to make sure examine index has a chance to update
                    //$timeout(function() {
                        //$location.url("/merchello/merchello/productedit/" + $scope.product.key, true);
                        loadProduct($scope.product.key);
                        //$route.reload();
                    //}, 400);
                     //}

                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error(localizationService.localize("merchelloStatusNotifications_productSaveError"), reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name saveProductVariant
             * @function
             *
             * @description
             * Saves a product variant.  See comments below.
             */
            function saveProductVariant() {
                //var variant = $scope.productVariant.deepClone();
                $scope.productVariant.removeInActiveInventories();
                var variantPromise = productResource.saveVariant($scope.productVariant);
                variantPromise.then(function(resultVariant) {
                    notificationsService.success("Product Variant Saved");
                    $scope.productVariant = productVariantDisplayBuilder.transform(resultVariant);
                    $scope.preValuesLoaded = true;
                }, function(reason) {
                    notificationsService.error("Product Variant Save Failed", reason.message);
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
                        $location.url("/merchello/merchello/productedit/" + result.key, true);
                    }, 1000);
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

            // Initialize the controller
            init();
    }]);
