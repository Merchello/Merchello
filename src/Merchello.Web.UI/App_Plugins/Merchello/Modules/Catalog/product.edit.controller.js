(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Product.EditController
     * @function
     * 
     * @description
     * The controller for the product edit view
     */
    controllers.ProductEditController = function ($scope, $routeParams, $location, $q, assetsService, notificationsService, dialogService, angularHelper, serverValidationManager, merchelloProductService, merchelloProductVariantService, merchelloWarehouseService, merchelloSettingsService) {

        assetsService.loadCss("/App_Plugins/Merchello/Common/Css/merchello.css");

        //--------------------------------------------------------------------------------------
        // Declare and initialize key scope properties
        //--------------------------------------------------------------------------------------
        // 

        // warehouses - Need this to link the possible warehouses to the inventory section
        $scope.warehouses = [];
        // settings - contains defaults for the checkboxes
        $scope.settings = {};

        // These help manage state for the four possible states this page can be in:
        //   * Editing a Product
        $scope.creatingProduct = false;
        $scope.creatingVariant = false;
        $scope.editingVariant = false;
        $scope.productVariant = new merchello.Models.ProductVariant();

        // To help umbraco directives show our page
        $scope.loaded = false;
        $scope.preValuesLoaded = false;


        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name loadAllWarehouses
         * @function
         * 
         * @description
         * Loads in default warehouse and all other warehouses from server into the scope.  Called in init().
         */
        $scope.loadAllWarehouses = function() {

            var promiseWarehouse = merchelloWarehouseService.getDefaultWarehouse();
            promiseWarehouse.then(function(warehouse) {
                $scope.defaultWarehouse = new merchello.Models.Warehouse(warehouse);
                $scope.warehouses.push($scope.defaultWarehouse);
                $scope.productVariant.ensureCatalogInventory($scope.defaultWarehouse);
            }, function (reason) {
                notificationsService.error("Default Warehouse Load Failed", reason.message);
            });

            // TODO: load other warehouses when implemented
        }

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         * 
         * @description
         * Loads in store settings from server into the scope and applies the 
         * defaults to the product variant.  Called in init().
         */
        $scope.loadSettings = function() {

            var promiseSettings = merchelloSettingsService.getAllSettings();
            promiseSettings.then(function(settings) {
                $scope.settings = new merchello.Models.StoreSettings(settings);
                $scope.productVariant.shippable = $scope.settings.globalShippable;
                $scope.productVariant.taxable = $scope.settings.globalTaxable;
                $scope.productVariant.trackInventory = $scope.settings.globalTrackInventory;
            }, function (reason) {
                notificationsService.error("Settings Load Failed", reason.message);
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
        function loadProduct(key) {

            var promiseProduct = merchelloProductService.getByKey(key);
            promiseProduct.then(function (product) {

                $scope.product = new merchello.Models.Product(product);

                $scope.productVariant.copyFromProduct($scope.product);

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {

                notificationsService.error("Product Load Failed", reason.message);

            });
        }

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        $scope.init = function () {

            $scope.loadAllWarehouses();
            $scope.loadSettings();
            loadProduct($routeParams.id);

        };

        $scope.init();


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
        $scope.save = function (thisForm) {

            if (thisForm.$valid) {

                // if saving a product (not a variant)
                if ($scope.product.hasVariants) // We added options / variants to a product that previously did not have them OR on save during a create
                {
                    // Copy from master variant
                    $scope.product.copyFromVariant($scope.productVariant);

                    var promise = merchelloProductService.updateProductWithVariants($scope.product);

                    promise.then(function (product) {
                        notificationsService.success("Products and variants saved", "");

                        $scope.product = product;
                        $scope.productVariant.copyFromProduct($scope.product);

                        if ($scope.product.hasVariants) {
                            $location.url("/merchello/merchello/ProductEditWithOptions/" + $scope.product.key, true);
                        }

                    }, function (reason) {
                        notificationsService.error("Product or variants save Failed", reason.message);
                    });
                } else // Simple product save with no options or variants
                {
                    if ($scope.product.productOptions.length > 0) // The options checkbox was checked, a blank option was added, then the options checkbox was unchecked
                    {
                        $scope.product.productOptions = [];
                    }

                    // Copy from master variant
                    $scope.product.copyFromVariant($scope.productVariant);

                    var promise = merchelloProductService.updateProduct($scope.product);

                    promise.then(function (product) {
                        notificationsService.success("Product Saved", "");

                        $scope.product = product;
                        $scope.productVariant.copyFromProduct($scope.product);

                    }, function (reason) {
                        notificationsService.error("Product Save Failed", reason.message);
                    });
                }
            }
        };


        /**
         * @ngdoc method
         * @name deleteProductDialogConfirmation
         * @function
         * 
         * @description
         * Called when the Delete Product button is pressed.
         */
        $scope.deleteProductDialogConfirmation = function () {
            var promiseDel = merchelloProductService.deleteProduct($scope.product);

            promiseDel.then(function () {
                notificationsService.success("Product Deleted", "");

                $location.url("/merchello/merchello/ProductList/manage", true);

            }, function (reason) {
                notificationsService.error("Product Deletion Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name deleteProductDialog
         * @function
         * 
         * @description                                                
         * Opens the delete confirmation dialog via the Umbraco dialogService.
         */
        $scope.deleteProductDialog = function () {

            dialogService.open({
                template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
                show: true,
                callback: $scope.deleteProductDialogConfirmation,
                dialogData: $scope.product
            });
        }

        /**
        * @ngdoc method
        * @name updateVariants
        * @function
        * 
        * @description
        * Called when the Update button is pressed below the options.  This will create a new product if necessary 
        * and save the product.  Then the product variants are generated.
        * 
        * We have to create the product because the API cannot create the variants with a product with options.
        */
        $scope.updateVariants = function (thisForm) {

            // Copy from master variant
            $scope.product.copyFromVariant($scope.productVariant);

            var promise = merchelloProductService.updateProduct($scope.product);

            promise.then(function(product) {
                notificationsService.success("Product Saved", "");

                $scope.product = product;
                $scope.productVariant.copyFromProduct($scope.product);

                $scope.product = merchelloProductService.createVariantsFromOptions($scope.product);

            }, function(reason) {
                notificationsService.error("Product Save Failed", reason.message);
            });

        };

    };

    angular.module("umbraco").controller("Merchello.Editors.Product.EditController", ['$scope', '$routeParams', '$location', '$q', 'assetsService', 'notificationsService', 'dialogService', 'angularHelper', 'serverValidationManager', 'merchelloProductService', 'merchelloProductVariantService', 'merchelloWarehouseService', 'merchelloSettingsService', merchello.Controllers.ProductEditController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

