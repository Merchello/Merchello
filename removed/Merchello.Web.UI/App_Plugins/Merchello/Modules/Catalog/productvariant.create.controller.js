(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.ProductVariant.CreateController
     * @function
     * 
     * @description
     * The controller for the product variant create view
     */
    controllers.ProductVariantCreateController = function ($scope, $routeParams, $location, $q, assetsService, notificationsService, dialogService, angularHelper, serverValidationManager, merchelloProductService, merchelloWarehouseService, merchelloSettingsService) {

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
        $scope.creatingProduct = false;
        $scope.creatingVariant = true;
        $scope.editingVariant = false;
        $scope.productVariant = new merchello.Models.ProductVariant();
        $scope.attributesKeys = [""];
        $scope.possibleVariants = [];

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
         * @name loadProductForVariantCreate
         * @function
         * 
         * @description
         * Load a product by the product key.  This is used only for creating variants on an existing product.
         */
        function loadProductForVariantCreate(key) {

            var promiseProduct = merchelloProductService.getByKey(key);
            promiseProduct.then(function (product) {

                $scope.product = new merchello.Models.Product(product);

                //var promiseCreatable = merchelloProductVariantService.getVariantsByProductThatCanBeCreated(key);
                //promiseCreatable.then(function (variants) {
                //    $scope.possibleVariants = _.map(variants, function (v) {
                //        var newVariant = new merchello.Models.ProductVariant(v);
                //        newVariant.key = "";
                //        return newVariant;
                //    });

                //    if (!_.isEmpty($scope.possibleVariants)) {
                //        $scope.productVariant = $scope.possibleVariants[0];
                //    }

                //    $scope.productVariant.ensureAllCatalogInventoriesForWarehouse($scope.defaultWarehouse);

                //    $scope.loaded = true;
                //    $scope.preValuesLoaded = true;

                //}, function (reason) {
                //    notificationsService.error("Product Variants Remaining Load Failed", reason.message);
                //});

            }, function (reason) {

                notificationsService.error("Parent Product Load Failed", reason.message);

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
            loadProductForVariantCreate($routeParams.id);

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

                if ($scope.creatingVariant) // Add a variant to product
                {
                    //var promise = merchelloProductVariantService.create($scope.productVariant);

                    //promise.then(function (productVariant) {
                    //    notificationsService.success("Product Variant Created and Saved", "");

                    //    $location.url("/merchello/merchello/ProductEditWithOptions/" + $scope.productVariant.productKey, true);

                    //}, function (reason) {
                    //    notificationsService.error("Product Variant Create Failed", reason.message);
                    //});
                }
            }
        };

    };

    angular.module("umbraco").controller("Merchello.Editors.ProductVariant.CreateController", ['$scope', '$routeParams', '$location', '$q', 'assetsService', 'notificationsService', 'dialogService', 'angularHelper', 'serverValidationManager', 'merchelloProductService', 'merchelloWarehouseService', 'merchelloSettingsService', merchello.Controllers.ProductVariantCreateController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

