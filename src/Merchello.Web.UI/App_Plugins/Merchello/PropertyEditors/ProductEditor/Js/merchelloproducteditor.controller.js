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
                    loadProduct($scope.model.value);
                    $scope.creatingProduct = false;
                    $scope.creatingVariant = false;
                    $scope.editingVariant = false;
                } else {
                    $scope.creatingProduct = true;
                    $scope.creatingVariant = false;
                    $scope.editingVariant = false;
                }
            }

            // Handle the form submitting event to save the product information to Merchello tables
            $scope.$on("formSubmitting", function (e, args) {
                notificationsService.success("Saving merchello product", "");
                $scope.save();
            });

        };

        $scope.init();


        //--------------------------------------------------------------------------------------
        // Event Handlers
        //--------------------------------------------------------------------------------------

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
            $scope.product = selectedProduct;
            $scope.productVariant.copyFromProduct($scope.product);
            $scope.creatingProduct = false;
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
         * @name save
         * @function
         * 
         * @description
         * Called when the Save button is pressed.  See comments below.
         */
        $scope.save = function () {

            if ($scope.creatingProduct) // Save on initial create
            {
                if (!$scope.product.hasVariants && $scope.product.productOptions.length > 0) // The options checkbox was checked, a blank option was added, then the has options was unchecked
                {
                    $scope.product.productOptions = [];
                }

                // Copy from master variant
                $scope.product.copyFromVariant($scope.productVariant);

                var promiseCreate = merchelloProductService.createProduct($scope.product, function () {
                    $scope.creatingProduct = false;
                    //notificationsService.success("*** Product ", status);
                });
                promiseCreate.then(function (product) {

                    $scope.product = product;
                    $scope.productVariant.copyFromProduct($scope.product);

                    $scope.creatingProduct = false; // For the variant edit/create view.

                    $scope.model.value = $scope.product.key;

                   // $location.url("/merchello/merchello/ProductEdit/" + $scope.product.key, true);

                    notificationsService.success("Product Created and Saved", "");

                }, function (reason) {
                    notificationsService.error("Product Create Failed", reason.message);
                });
            } else // if saving a product (not a variant)
            {
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
                            //$location.url("/merchello/merchello/ProductEditWithOptions/" + $scope.product.key, true);
                        }

                    }, function (reason) {
                        notificationsService.error("Product or variants save Failed", reason.message);
                    });
                } else // Simple product save with no options or variants
                {
                    if ($scope.product.productOptions.length > 0) // The options checkbox was checked, a blank option was added, then the has options was unchecked
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


    };

    angular.module("umbraco").controller('Merchello.PropertyEditors.MerchelloProductEditor', ['$scope', '$routeParams', 'merchelloProductService', 'notificationsService', 'dialogService', 'merchelloWarehouseService', 'merchelloSettingsService', productEditor]);

})();