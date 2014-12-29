(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.ProductVariant.EditController
     * @function
     * 
     * @description
     * The controller for the product editor
     */
    controllers.ProductVariantEditController = function($scope, $routeParams, $location, $q, assetsService, notificationsService, dialogService, angularHelper, serverValidationManager, merchelloProductService, merchelloWarehouseService, merchelloSettingsService) {

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
        $scope.creatingVariant = false;
        $scope.editingVariant = true;
        $scope.product = {};
        $scope.productVariant = {};
        $scope.product.hasOptions = false;
        $scope.product.hasVariants = true;

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
        $scope.loadAllWarehouses = function () {

            var deferred = $q.defer();

            var promiseWarehouse = merchelloWarehouseService.getDefaultWarehouse();
            promiseWarehouse.then(function (warehouse) {

                $scope.defaultWarehouse = new merchello.Models.Warehouse(warehouse);
                $scope.warehouses.push($scope.defaultWarehouse);
                deferred.resolve();

            }, function (reason) {
                notificationsService.error("Default Warehouse Load Failed", reason.message);
                deferred.reject(reason);
            });

            // TODO: load other warehouses when implemented

            return deferred.promise;
        }

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         * 
         * @description
         * Loads in store settings from server into the scope.  Called in init().
         */
        $scope.loadSettings = function () {

            var promiseSettings = merchelloSettingsService.getAllSettings();
            promiseSettings.then(function (settings) {
                $scope.settings = new merchello.Models.StoreSettings(settings);
            }, function (reason) {
                notificationsService.error("Settings Load Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name loadProductVariant
         * @function
         * 
         * @description
         * Load a product variant by the variant key.
         */
        function loadProductVariant(id) {

            var promiseVariant = merchelloProductService.getVariant(id);
            promiseVariant.then(function (productVariant) {

                $scope.productVariant = new merchello.Models.ProductVariant(productVariant);

                //$scope.productVariant.ensureCatalogInventory($scope.defaultWarehouse);
                $scope.productVariant.ensureAllCatalogInventoriesForWarehouse($scope.defaultWarehouse);

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {

                notificationsService.error("Product Variant Load Failed", reason.message);

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

            var promiseWarehouses = $scope.loadAllWarehouses();
            promiseWarehouses.then(function () {

                $scope.loadSettings();
                loadProductVariant($routeParams.variantid);

            }, function (reason) {

                //notificationsService.error("Load Failed", reason.message);

            });

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

                var promise = merchelloProductService.saveVariant($scope.productVariant);

                promise.then(function (product) {
                    notificationsService.success("Product Variant Saved", "");

                    $location.url("/merchello/merchello/ProductEditWithOptions/" + $scope.productVariant.productKey, true);

                }, function (reason) {
                    notificationsService.error("Product Variant Save Failed", reason.message);
                });
            }
        };

        /**
         * @ngdoc method
         * @name deleteVariantDialogConfirmation
         * @function
         * 
         * @description
         * Called when the Delete Variant button is pressed.
         */
        $scope.deleteVariantDialogConfirmation = function () {
            //var promiseDel = merchelloProductVariantService.deleteVariant($scope.productVariant.key);

            //promiseDel.then(function () {
            //    notificationsService.success("Product Variant Deleted", "");

            //    $location.url("/merchello/merchello/ProductEditWithOptions/" + $scope.productVariant.productKey, true);

            //}, function (reason) {
            //    notificationsService.error("Product Variant Deletion Failed", reason.message);
            //});
        };

        /**
         * @ngdoc method
         * @name deleteVariantDialog
         * @function
         * 
         * @description
         * Opens the delete confirmation dialog via the Umbraco dialogService.
         */
        $scope.deleteVariantDialog = function () {

            dialogService.open({
                template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
                show: true,
                callback: $scope.productVariant,
                dialogData: $scope.productVariant
            });
        }
    };

    angular.module("umbraco").controller("Merchello.Editors.ProductVariant.EditController", ['$scope', '$routeParams', '$location', '$q', 'assetsService', 'notificationsService', 'dialogService', 'angularHelper', 'serverValidationManager', 'merchelloProductService', 'merchelloWarehouseService', 'merchelloSettingsService', merchello.Controllers.ProductVariantEditController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

