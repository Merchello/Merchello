(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Product.EditController
     * @function
     * 
     * @description
     * The controller for the product edit view
     */
    controllers.ProductEditController = function ($scope, $routeParams, $location, $q, assetsService, notificationsService, dialogService, angularHelper, serverValidationManager, merchelloProductService, merchelloWarehouseService, merchelloSettingsService) {

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
        $scope.loadSettings = function() {

            var promiseSettings = merchelloSettingsService.getAllSettings();
            promiseSettings.then(function(settings) {
                $scope.settings = new merchello.Models.StoreSettings(settings);
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

                $scope.productVariant.ensureAllCatalogInventoriesForWarehouse($scope.defaultWarehouse);

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

            var promiseWarehouses = $scope.loadAllWarehouses();
            promiseWarehouses.then(function () {

                $scope.loadSettings();
                loadProduct($routeParams.id);

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

                // Copy from master variant
                $scope.product.copyFromVariant($scope.productVariant);

                var promise = merchelloProductService.updateProduct($scope.product);

                promise.then(function (product) {
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

    };

    angular.module("umbraco").controller("Merchello.Editors.Product.EditController", ['$scope', '$routeParams', '$location', '$q', 'assetsService', 'notificationsService', 'dialogService', 'angularHelper', 'serverValidationManager', 'merchelloProductService', 'merchelloWarehouseService', 'merchelloSettingsService', merchello.Controllers.ProductEditController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

