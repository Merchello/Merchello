'use strict';

(function () {

    function productEditor($scope, $routeParams, merchelloProductService, notificationsService, dialogService, merchelloWarehouseService, merchelloSettingsService) {

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
        //   * Editing a Product

        $scope.productVariant = new merchello.Models.ProductVariant();
        $scope.product = new merchello.Models.Product();

        // To help umbraco directives show our page
        $scope.loaded = false;
        $scope.preValuesLoaded = false;

        // Property to control the template to show for the product state (product, product with options, etc)
        $scope.templatePath = "";



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

            var promiseWarehouse = merchelloWarehouseService.getDefaultWarehouse();
            promiseWarehouse.then(function (warehouse) {
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
        $scope.loadSettings = function () {

            var promiseSettings = merchelloSettingsService.getAllSettings();
            promiseSettings.then(function (settings) {
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

                if ($scope.product.hasVariants) {
                    $scope.templatePath = "/App_Plugins/Merchello/PropertyEditors/ProductEditor/Views/merchelloproducteditor.producteditwithoptions.html";
                } else {
                    $scope.templatePath = "/App_Plugins/Merchello/PropertyEditors/ProductEditor/Views/merchelloproducteditor.productedit.html";
                }

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
            // Load the product from the Guid key stored in the model.value
            if (_.isString($scope.model.value)) {
                if ($scope.model.value.length > 0) {
                    // editing a product or product with options
                    loadProduct($scope.model.value);
                    $scope.creatingProduct = false;
                    $scope.creatingVariant = false;
                    $scope.editingVariant = false;
                } else {
                    $scope.templatePath = "/App_Plugins/Merchello/PropertyEditors/ProductEditor/Views/merchelloproducteditor.productcreate.html";
                    $scope.creatingProduct = true;
                    $scope.creatingVariant = false;
                    $scope.editingVariant = false;
                }
            }

        };

        $scope.init();


        //--------------------------------------------------------------------------------------
        // Event Handlers
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name changeTemplate
         * @function
         * 
         * @description
         * Handles the template switching for the edit / create / edit with options views
         */
        $scope.changeTemplate = function (templateUrl) {

            $scope.templatePath = templateUrl;
            $scope.creatingProduct = false;
        };

        /**
         * @ngdoc method
         * @name selectedProductFromDialog
         * @function
         * 
         * @description
         * Handles the model update after recieving the product to add from the dialog view/controller
         */
        $scope.selectedProductFromDialog = function (selectedProduct) {

            $scope.model.value = selectedProduct.key;
            $scope.creatingProduct = false;
            loadProduct(selectedProduct.key);
        };

        /**
         * @ngdoc method
         * @name selectProduct
         * @function
         * 
         * @description
         * Opens the product select dialog via the Umbraco dialogService.
         */
        $scope.selectProduct = function () {

            dialogService.open({
                template: '/App_Plugins/Merchello/PropertyEditors/ProductPicker/Views/merchelloproductdialog.html',
                show: true,
                callback: $scope.selectedProductFromDialog,
                dialogData: $scope.product
            });

        };

        /**
         * @ngdoc method
         * @name removeProduct
         * @function
         * 
         * @description
         * Opens the product select dialog via the Umbraco dialogService.
         */
        $scope.removeProduct = function () {

            $scope.creatingProduct = true;
            $scope.model.value = null;
            $scope.templatePath = "";

        };

    };

    angular.module("umbraco").controller('Merchello.PropertyEditors.MerchelloProductEditor', ['$scope', '$routeParams', 'merchelloProductService', 'notificationsService', 'dialogService', 'merchelloWarehouseService', 'merchelloSettingsService', productEditor]);

})();