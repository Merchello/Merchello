(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Product.CreateController
     * @function
     * 
     * @description
     * The controller for the product create view
     */
    controllers.ProductCreateController = function($scope, $routeParams, $location, $q, assetsService, notificationsService, dialogService, angularHelper, serverValidationManager, merchelloProductService, merchelloWarehouseService, merchelloSettingsService) {

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
        //   * Creating a Product
        // TODO: clean up?
        $scope.creatingProduct = true;
        $scope.creatingVariant = false;
        $scope.editingVariant = false;
        $scope.productVariant = new merchello.Models.ProductVariant();
        $scope.product = new merchello.Models.Product();

        // To help umbraco directives show our page
        $scope.loaded = true;
        $scope.preValuesLoaded = true;


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
                $scope.productVariant.ensureAllCatalogInventoriesForWarehouse($scope.defaultWarehouse);
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
         * @name init
         * @function
         * 
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        $scope.init = function () {

            $scope.loadAllWarehouses();
            $scope.loadSettings();

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

                //notificationsService.info("Saving...", "");


                if ($scope.creatingProduct) // Save on initial create
                {
                    // Copy from master variant
                    $scope.product.copyFromVariant($scope.productVariant);

                    var promiseCreate = merchelloProductService.createProduct($scope.product);
                    promiseCreate.then(function(product) {

                        $scope.product = product;
                        $scope.productVariant.copyFromProduct($scope.product);

                        $scope.creatingProduct = false; // For the variant edit/create view.

                        if ($scope.product.hasVariants) {
                            $location.url("/merchello/merchello/ProductEditWithOptions/" + $scope.product.key, true);
                        }

                        notificationsService.success("Product Created and Saved", "");

                    }, function(reason) {
                        notificationsService.error("Product Create Failed", reason.message);
                    });
                } else // if saving a product (not a variant)
                {
                    // Copy from master variant
                    $scope.product.copyFromVariant($scope.productVariant);

                    var promiseSave = merchelloProductService.updateProduct($scope.product);

                    promiseSave.then(function (product) {
                        notificationsService.success("Product Saved", "");

                        $scope.product = product;
                        $scope.productVariant.copyFromProduct($scope.product);

                        if ($scope.product.hasVariants) {
                            $location.url("/merchello/merchello/ProductEditWithOptions/" + $scope.product.key, true);
                        }

                    }, function (reason) {
                        notificationsService.error("Product Save Failed", reason.message);
                    });
                }
            }
        };

    };

    angular.module("umbraco").controller("Merchello.Editors.Product.CreateController", ['$scope', '$routeParams', '$location', '$q', 'assetsService', 'notificationsService', 'dialogService', 'angularHelper', 'serverValidationManager', 'merchelloProductService', 'merchelloWarehouseService', 'merchelloSettingsService', merchello.Controllers.ProductCreateController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

