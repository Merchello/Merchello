(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.ProductVariant.EditController
     * @function
     * 
     * @description
     * The controller for the product editor
     */
    controllers.ProductVariantEditController = function ($scope, $routeParams, $location, $q, assetsService, notificationsService, dialogService, angularHelper, serverValidationManager, merchelloProductService, merchelloProductVariantService, merchelloWarehouseService, merchelloSettingsService) {

        assetsService.loadCss("/App_Plugins/Merchello/Common/Css/merchello.css");

        function loadProductVariant(id) {

            var promiseVariant = merchelloProductVariantService.getById(id);
            promiseVariant.then(function (productVariant) {

                $scope.productVariant = new merchello.Models.ProductVariant(productVariant);

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {

                notificationsService.error("Product Variant Load Failed", reason.message);

            });
        }

        function loadProduct(key, isVariant) {

            var promiseProduct = merchelloProductService.getByKey(key);
            promiseProduct.then(function (product) {

                $scope.product = new merchello.Models.Product(product);

                if (!isVariant)
                {
                    // create the master variant (since most form items are bound to the productVariant model)
                    $scope.productVariant.copyFromProduct($scope.product);
                }
                else
                {
                    // Let the variant load from the loadProductVariant() call
                }

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {

                notificationsService.error("Product Load Failed", reason.message);

            });
        }

        function loadProductForVariantCreate(key) {

            var promiseProduct = merchelloProductService.getByKey(key);
            promiseProduct.then(function (product) {

                $scope.product = new merchello.Models.Product(product);

                $scope.productVariant.copyFromProduct($scope.product);
                $scope.productVariant.name = "";
                $scope.productVariant.sku = "";

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {

                notificationsService.error("Parent Product Load Failed", reason.message);

            });
        }

        function isCreating() {
            return $routeParams.create;
        }

        function isCreatingVariant() {
            return $routeParams.createvariant;
        }

        ////////////////////////////////////////////////
        // Initialize state

        // Get warehouses
        $scope.warehouses = [];
        var promise = merchelloWarehouseService.getDefaultWarehouse();
        promise.then(function (warehouse) {
            $scope.defaultWarehouse = new merchello.Models.Warehouse(warehouse);
            $scope.warehouses.push($scope.defaultWarehouse);
            if (isCreating() || isCreatingVariant())
            {
                $scope.productVariant.ensureCatalogInventory($scope.defaultWarehouse);
            }
        });

        // Get settings
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

        if (isCreating()) {
            $scope.creatingProduct = true;
            $scope.creatingVariant = false;
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.productVariant = new merchello.Models.ProductVariant();
            $scope.product = new merchello.Models.Product();
            $scope.editingVariant = false;
        }
        else if (isCreatingVariant()) {
            $scope.creatingProduct = false;
            $scope.creatingVariant = true;
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.productVariant = new merchello.Models.ProductVariant();
            loadProductForVariantCreate($routeParams.id);
            $scope.editingVariant = false;
        }
        else {

            $scope.creatingProduct = false;
            $scope.creatingVariant = false;
            $scope.productVariant = new merchello.Models.ProductVariant();
            $scope.product = {};
            $scope.editingVariant = false;

            if ($routeParams.variantid == undefined)
            {
                //we are editing a product with no options so get the product from the server
                loadProduct($routeParams.id, false);
            }
            else
            {
                //we are editing a variant so get the product variant and product from the server
                loadProductVariant($routeParams.variantid);

                //loadProduct($routeParams.id, true);
                $scope.product = {};
                $scope.product.hasOptions = false;
                $scope.product.hasVariants = true;


                $scope.editingVariant = true;
            }

        }
        ////////////////////////////////////////////////


        ////////////////////////////////////////////////
        // EVENTS

        $scope.save = function (thisForm) {

            if (thisForm.$valid)
            {

                notificationsService.info("Saving...", "");


                if (isCreating()) // Save on initial create
                {
                    // Copy from master variant
                    $scope.product.copyFromVariant($scope.productVariant);

                    var promiseCreate = merchelloProductService.createProduct($scope.product, function () { $scope.creatingProduct = false; notificationsService.success("*** Product ", status); });
                    promiseCreate.then(function (product) {

                        $scope.product = product;
                        $scope.productVariant.copyFromProduct($scope.product);

                        $scope.creatingProduct = false;    // For the variant edit/create view.
                        notificationsService.success("Product Created and Saved", "H5YR!");

                    }, function (reason) {
                        notificationsService.error("Product Create Failed", reason.message);
                    });
                }
                else if (isCreatingVariant())  // Add a variant to product
                {
                    var promise = merchelloProductVariantService.create($scope.productVariant);

                    promise.then(function (productVariant) {
                        notificationsService.success("Product Variant Created and Saved", "H5YR!");

                        $location.url("/merchello/merchello/ProductEdit/" + $scope.productVariant.productKey, true);

                    }, function (reason) {
                        notificationsService.error("Product Variant Create Failed", reason.message);
                    });
                }
                else if ($scope.editingVariant)  // Save a variant that is being edited
                {
                    var promise = merchelloProductVariantService.save($scope.productVariant);

                    promise.then(function (product) {
                        notificationsService.success("Product Variant Saved", "H5YR!");

                        $location.url("/merchello/merchello/ProductEdit/" + $scope.productVariant.productKey, true);

                    }, function (reason) {
                        notificationsService.error("Product Variant Save Failed", reason.message);
                    });
                }
                else   // if saving a product (not a variant)
                {                    
                    if ($scope.product.hasVariants)   // We added options / variants to a product that previously did not have them OR on save during a create
                    {
                        // Copy from master variant
                        $scope.product.copyFromVariant($scope.productVariant);

                        var promise = merchelloProductService.updateProductWithVariants($scope.product);

                        promise.then(function (product) {
                            notificationsService.success("Products and variants saved", "");

                            $scope.product = product;
                            $scope.productVariant.copyFromProduct($scope.product);

                            if ($scope.product.hasVariants) {
                                $location.url("/merchello/merchello/ProductEdit/" + $scope.product.key, true);
                            }

                        }, function (reason) {
                            notificationsService.error("Product or variants save Failed", reason.message);
                        });
                    }
                    else  // Simple product save with no options or variants
                    {
                        // Copy from master variant
                        $scope.product.copyFromVariant($scope.productVariant);

                        var promise = merchelloProductService.updateProduct($scope.product);

                        promise.then(function (product) {
                            notificationsService.success("Product Saved", "H5YR!");

                            $scope.product = product;
                            $scope.productVariant.copyFromProduct($scope.product);

                        }, function (reason) {
                            notificationsService.error("Product Save Failed", reason.message);
                        });
                    }
                }
            }
        };

        $scope.chooseMedia = function () {

            dialogService.mediaPicker({
                multipicker: true,
                callback: function (data) {
                    _.each(data.selection, function (media, i) {
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

        $scope.ensureInitialOption = function () {

            if ($scope.product.productOptions.length == 0)
            {
                $scope.product.addBlankOption();
            }
        };

        $scope.ensureCatalogInventory = function () {

            $scope.productVariant.ensureCatalogInventory($scope.defaultWarehouse);
        };

        $scope.addOption = function () {

            $scope.product.addBlankOption();

        };
        
        $scope.updateVariants = function (thisForm) {

            // Create the product if not created
            if ($scope.creatingProduct)
            {
                if (thisForm.$valid)
                {
                    notificationsService.info("Creating and saving new product", "");

                    // Copy from master variant
                    $scope.product.copyFromVariant($scope.productVariant);

                    var promiseCreate = merchelloProductService.createProduct($scope.product, function () { $scope.creatingProduct = false; notificationsService.success("*** Product ", status); });
                    promiseCreate.then(function (product) {

                        $scope.product = product;
                        $scope.productVariant.copyFromProduct($scope.product);

                        notificationsService.success("Product Created and Saved", "H5YR!");

                        $scope.product = merchelloProductService.createVariantsFromOptions($scope.product);

                        $scope.creatingProduct = false;    // For the variant edit/create view.
                        notificationsService.success("Product Variants Created", "");

                    }, function (reason) {
                        notificationsService.error("Product Create Failed", reason.message);
                    });
                }
                else
                {
                    notificationsService.error("Please verify a valid name, sku, and price has been entered", "");
                }
            }
            else
            {
                // Copy from master variant
                $scope.product.copyFromVariant($scope.productVariant);

                var promise = merchelloProductService.updateProduct($scope.product);

                promise.then(function (product) {
                    notificationsService.success("Product Saved", "H5YR!");

                    $scope.product = product;
                    $scope.productVariant.copyFromProduct($scope.product);

                    $scope.product = merchelloProductService.createVariantsFromOptions($scope.product);

                }, function (reason) {
                    notificationsService.error("Product Save Failed", reason.message);
                });
            }
        };

        $scope.applyAllVariantInventories = function (allVariantInventories) {

            for (var i = 0; i < $scope.product.productVariants.length; i++) {
                $scope.product.productVariants[i].globalInventoryChanged(allVariantInventories);
            }

        };

        ////////////////////////////////////////////////
    }

    angular.module("umbraco").controller("Merchello.Editors.ProductVariant.EditController", merchello.Controllers.ProductVariantEditController);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

