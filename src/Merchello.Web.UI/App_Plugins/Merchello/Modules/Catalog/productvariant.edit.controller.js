(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.ProductVariant.EditController
     * @function
     * 
     * @description
     * The controller for the product editor
     */
    controllers.ProductVariantEditController = function($scope, $routeParams, $location, $q, assetsService, notificationsService, dialogService, angularHelper, serverValidationManager, merchelloProductService, merchelloProductVariantService, merchelloWarehouseService, merchelloSettingsService) {

        assetsService.loadCss("/App_Plugins/Merchello/Common/Css/merchello.css");

        ////////////////////////////////////////////////
        // Initialize state

        // Get warehouses - Need this to link the possible warehouses to the inventory section
        $scope.warehouses = [];
        var promiseWarehouse = merchelloWarehouseService.getDefaultWarehouse();
        promiseWarehouse.then(function (warehouse) {
            $scope.defaultWarehouse = new merchello.Models.Warehouse(warehouse);
            $scope.warehouses.push($scope.defaultWarehouse);
            if (isCreating() || isCreatingVariant()) {
                $scope.productVariant.ensureCatalogInventory($scope.defaultWarehouse);
            }
        });

        // Get settings - contains defaults for the checkboxes
        $scope.settings = {};
        var promiseSettings = merchelloSettingsService.getAllSettings();
        promiseSettings.then(function (settings) {
            $scope.settings = new merchello.Models.StoreSettings(settings);
            if (isCreating() || isCreatingVariant()) {
                $scope.productVariant.shippable = $scope.settings.globalShippable;
                $scope.productVariant.taxable = $scope.settings.globalTaxable;
                $scope.productVariant.trackInventory = $scope.settings.globalTrackInventory;
            }
        });

        $scope.allVariantInventories = 0;

        // This is to help manage state for the four possible states this page can be in:
        //   * Creating a Product
        //   * Editing a Product
        //   * Creating a Product Variant
        //   * Editing a Product Variant
        if (isCreating()) {
            $scope.creatingProduct = true;
            $scope.creatingVariant = false;
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.productVariant = new merchello.Models.ProductVariant();
            $scope.product = new merchello.Models.Product();
            $scope.editingVariant = false;
        } else if (isCreatingVariant()) {
            $scope.creatingProduct = false;
            $scope.creatingVariant = true;
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.productVariant = new merchello.Models.ProductVariant();
            $scope.attributesKeys = [""];
            $scope.possibleVariants = [];
            loadProductForVariantCreate($routeParams.id);
            $scope.editingVariant = false;
        } else {

            $scope.creatingProduct = false;
            $scope.creatingVariant = false;
            $scope.productVariant = new merchello.Models.ProductVariant();
            $scope.product = {};
            $scope.editingVariant = false;

            if ($routeParams.variantid == undefined) {
                //we are editing a product with no options so get the product from the server
                loadProduct($routeParams.id, false);
            } else {
                //we are editing a variant so get the product variant and product from the server
                loadProductVariant($routeParams.variantid);

                //loadProduct($routeParams.id, true);
                $scope.product = {};
                $scope.product.hasOptions = false;
                $scope.product.hasVariants = true;


                $scope.editingVariant = true;
                $scope.parentProductId = $routeParams.id;
            }

        }
        ////////////////////////////////////////////////



        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name loadProductVariant
         * @function
         * 
         * @description
         * Load a product variant by the variant key.
         */
        function loadProductVariant(id) {

            var promiseVariant = merchelloProductVariantService.getById(id);
            promiseVariant.then(function(productVariant) {

                $scope.productVariant = new merchello.Models.ProductVariant(productVariant);

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function(reason) {

                notificationsService.error("Product Variant Load Failed", reason.message);

            });
        }

        /**
         * @ngdoc method
         * @name loadProduct
         * @function
         * 
         * @description
         * Load a product by the product key.  The isVariant flag lets us know if we are trying to edit a variant or edit a product.
         */
        function loadProduct(key, isVariant) {

            var promiseProduct = merchelloProductService.getByKey(key);
            promiseProduct.then(function(product) {

                $scope.product = new merchello.Models.Product(product);

                if (!isVariant) {
                    // create the master variant (since most form items are bound to the productVariant model)
                    $scope.productVariant.copyFromProduct($scope.product);
                } else {
                    // Let the variant load from the loadProductVariant() call
                }

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function(reason) {

                notificationsService.error("Product Load Failed", reason.message);

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
            promiseProduct.then(function(product) {

                $scope.product = new merchello.Models.Product(product);

                var promiseCreatable = merchelloProductVariantService.getVariantsByProductThatCanBeCreated(key);
                promiseCreatable.then(function (variants) {
                    $scope.possibleVariants = _.map(variants, function (v) {
                        var newVariant = new merchello.Models.ProductVariant(v);
                        newVariant.key = "";
                        return newVariant;
                    });

                    if (!_.isEmpty($scope.possibleVariants)) {
                        $scope.productVariant = $scope.possibleVariants[0];                        
                    }

                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

                }, function (reason) {
                    notificationsService.error("Product Variants Remaining Load Failed", reason.message);
                });

            }, function(reason) {

                notificationsService.error("Parent Product Load Failed", reason.message);

            });
        }


        //--------------------------------------------------------------------------------------
        // Helper methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name isCreating
         * @function
         * 
         * @description
         * Helper to get whether a product is being created.  This is indicated by a param in the url.
         */
        function isCreating() {
            return $routeParams.create;
        }

        /**
         * @ngdoc method
         * @name isCreatingVariant
         * @function
         * 
         * @description
         * Helper to get whether a variant is being created.  This is indicated by a param in the url.
         */
        function isCreatingVariant() {
            return $routeParams.createvariant;
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
        $scope.save = function (thisForm) {

            if (thisForm.$valid) {

                //notificationsService.info("Saving...", "");


                if ($scope.creatingProduct) // Save on initial create
                {
                    if (!$scope.product.hasVariants && $scope.product.productOptions.length > 0) // The options checkbox was checked, a blank option was added, then the has options was unchecked
                    {
                        $scope.product.productOptions = [];
                    }

                    // Copy from master variant
                    $scope.product.copyFromVariant($scope.productVariant);

                    var promiseCreate = merchelloProductService.createProduct($scope.product, function() {
                        $scope.creatingProduct = false;
                        //notificationsService.success("*** Product ", status);
                    });
                    promiseCreate.then(function(product) {

                        $scope.product = product;
                        $scope.productVariant.copyFromProduct($scope.product);

                        $scope.creatingProduct = false; // For the variant edit/create view.
                        notificationsService.success("Product Created and Saved", "");

                    }, function(reason) {
                        notificationsService.error("Product Create Failed", reason.message);
                    });
                } else if ($scope.creatingVariant) // Add a variant to product
                {
                    var promise = merchelloProductVariantService.create($scope.productVariant);

                    promise.then(function(productVariant) {
                        notificationsService.success("Product Variant Created and Saved", "");

                        $location.url("/merchello/merchello/ProductEdit/" + $scope.productVariant.productKey, true);

                    }, function(reason) {
                        notificationsService.error("Product Variant Create Failed", reason.message);
                    });
                } else if ($scope.editingVariant) // Save a variant that is being edited
                {
                    var promise = merchelloProductVariantService.save($scope.productVariant);

                    promise.then(function(product) {
                        notificationsService.success("Product Variant Saved", "");

                        $location.url("/merchello/merchello/ProductEdit/" + $scope.productVariant.productKey, true);

                    }, function(reason) {
                        notificationsService.error("Product Variant Save Failed", reason.message);
                    });
                } else // if saving a product (not a variant)
                {
                    if ($scope.product.hasVariants) // We added options / variants to a product that previously did not have them OR on save during a create
                    {
                        // Copy from master variant
                        $scope.product.copyFromVariant($scope.productVariant);

                        var promise = merchelloProductService.updateProductWithVariants($scope.product);

                        promise.then(function(product) {
                            notificationsService.success("Products and variants saved", "");

                            $scope.product = product;
                            $scope.productVariant.copyFromProduct($scope.product);

                            if ($scope.product.hasVariants) {
                                $location.url("/merchello/merchello/ProductEdit/" + $scope.product.key, true);
                            }

                        }, function(reason) {
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

                        promise.then(function(product) {
                            notificationsService.success("Product Saved", "");

                            $scope.product = product;
                            $scope.productVariant.copyFromProduct($scope.product);

                        }, function(reason) {
                            notificationsService.error("Product Save Failed", reason.message);
                        });
                    }
                }
            }
        };

        /**
         * @ngdoc method
         * @name deleteProduct
         * @function
         * 
         * @description
         * Called when the Delete Product button is pressed.
         *
         * TODO: Need to call a confirmation dialog for this.
         */
        $scope.deleteProduct = function () {
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
         * @name deleteVariant
         * @function
         * 
         * @description
         * Called when the Delete Variant button is pressed.
         *
         * TODO: Need to call a confirmation dialog for this.
         */
        $scope.deleteVariant = function () {
            var promiseDel = merchelloProductVariantService.deleteVariant($scope.productVariant.key);

            promiseDel.then(function () {
                notificationsService.success("Product Variant Deleted", "");

                $location.url("/merchello/merchello/ProductEdit/" + $scope.productVariant.productKey, true);

            }, function (reason) {
                notificationsService.error("Product Variant Deletion Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name chooseMedia
         * @function
         * 
         * @description
         * Called when the select media button is pressed for the digital download section.
         *
         * TODO: make a media selection dialog that works with PDFs, etc
         */
        $scope.chooseMedia = function () {

            dialogService.mediaPicker({
                multipicker: true,
                callback: function(data) {
                    _.each(data.selection, function(media, i) {
                        //shortcuts
                        //TODO, do something better then this for searching

                        var img = {};
                        img.id = media.id;
                        img.src = imageHelper.getImagePropertyValue({ imageModel: media });
                        img.thumbnail = imageHelper.getThumbnailFromPath(img.src);
                        $scope.images.push(img);
                        $scope.ids.push(img.id);
                    });

                    $scope.sync();
                }
            });

        };

        /**
         * @ngdoc method
         * @name ensureInitialOption
         * @function
         * 
         * @description
         * This is called when the "This variant has options" checkbox is checked.  It creates an initial blank option ready to 
         * fill out.  If the checkbox is unchecked, then the option will be deleted before saving the product.
         */
        $scope.ensureInitialOption = function () {

            if ($scope.product.productOptions.length == 0) {
                $scope.product.addBlankOption();
            }
        };

        /**
         * @ngdoc method
         * @name ensureCatalogInventory
         * @function
         * 
         * @description
         * This is called when the "Track inventory for this variant" checkbox is checked.  It creates an initial catalog inventory object ready to 
         * fill out.
         */
        $scope.ensureCatalogInventory = function () {

            $scope.productVariant.ensureCatalogInventory($scope.defaultWarehouse);
        };

        /**
         * @ngdoc method
         * @name addOption
         * @function
         * 
         * @description
         * Called when the Add Option button is pressed.  Creates a new option ready to fill out.
         */
        $scope.addOption = function () {

            $scope.product.addBlankOption();

        };

        /**
         * @ngdoc method
         * @name removeOption
         * @function
         * 
         * @description
         * Called when the Trash can icon button is pressed next to an option. Removes the option from the product.
         */
        $scope.removeOption = function (option) {

            $scope.product.removeOption(option);

        };

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

            // Create the product if not created
            if ($scope.creatingProduct) {
                if (thisForm.$valid) {
                    //notificationsService.info("Creating and saving new product", "");

                    // Copy from master variant
                    $scope.product.copyFromVariant($scope.productVariant);

                    var promiseCreate = merchelloProductService.createProduct($scope.product, function() {
                        $scope.creatingProduct = false;
                        //notificationsService.success("*** Product ", status);
                    });
                    promiseCreate.then(function(product) {

                        $scope.product = product;
                        $scope.productVariant.copyFromProduct($scope.product);

                        //notificationsService.success("Product Created and Saved", "");

                        $scope.product = merchelloProductService.createVariantsFromOptions($scope.product);

                        $scope.creatingProduct = false; // For the variant edit/create view.
                        notificationsService.success("Product and Product Variants Created", "");

                    }, function(reason) {
                        notificationsService.error("Product Create Failed", reason.message);
                    });
                } else {
                    notificationsService.error("Please verify a valid name, sku, and price has been entered", "");
                }
            } else {
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
            }
        };

        /**
        * @ngdoc method
        * @name updateVariants
        * @function
        * 
        * @description
        * Called when the Apply button is pressed in the Base Inventory section.  This simply sets the inventory
        * amounts for each variant to the number in the box next to the apply button.
        */
        $scope.applyAllVariantInventories = function (allVariantInventories) {

            for (var i = 0; i < $scope.product.productVariants.length; i++) {
                $scope.product.productVariants[i].globalInventoryChanged(allVariantInventories);
            }

        };

    };

    angular.module("umbraco").controller("Merchello.Editors.ProductVariant.EditController", ['$scope', '$routeParams', '$location', '$q', 'assetsService', 'notificationsService', 'dialogService', 'angularHelper', 'serverValidationManager', 'merchelloProductService', 'merchelloProductVariantService', 'merchelloWarehouseService', 'merchelloSettingsService', merchello.Controllers.ProductVariantEditController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

