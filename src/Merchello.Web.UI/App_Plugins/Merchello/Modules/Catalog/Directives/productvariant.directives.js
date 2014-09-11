(function (directives, undefined) {


    //angular.module("umbraco").run(["$templateCache", function ($templateCache) {
    //    $templateCache.put("product-main-edit.html", '<div><div class="row-fluid"><div class="form-group col-xs-6 span6"><label for="price"><localize key="merchelloGeneral_price"/></label><input id="price" name="price" required data-ng-pattern="/^\d+(\.\d+)?$/" type="text" class="form-control col-xs-8 span8" data-ng-model="productVariant.price" /></div><div class="form-group col-xs-6 span6"><label for="saleprice"><localize key="merchelloGeneral_salePrice"/></label><input id="saleprice" name="saleprice" type="text" class="form-control col-xs-8 span8" data-ng-model="productVariant.salePrice" /></div></div><div class="row-fluid"><div class="form-group col-xs-6 span6"><label for="sku"><localize key="merchelloVariant_baseSku"/> <small><localize key="merchelloVariant_mustUniqueSku" /></small></label><input id="sku" data-ng-required="!creatingVariant" data-ng-disabled="creatingVariant" name="sku" required type="text" class="form-control col-xs-8 span8" data-ng-model="productVariant.sku" /></div><div class="form-group col-xs-6 span6"><label for="barcode"><localize key="merchelloVariant_barcode"/> <small><localize key="merchelloVariant_barcodeExamples"/></small></label><input id="barcode" name="barcode" type="text" class="form-control col-xs-8 span8" data-ng-model="productVariant.barcode" /></div></div><div class="row-fluid"><div class="form-group col-xs-6 span6"><label for="manufacturer"><localize key="merchelloVariant_manufacturer"/></label><input id="manufacturer" name="manufacturer" type="text" class="form-control col-xs-8 span8" data-ng-model="productVariant.manufacturer" /></div><div class="form-group col-xs-6 span6"><label for="manufacturermodelnumber"><localize key="merchelloVariant_manufacturerModel"/></label><input id="manufacturermodelnumber" name="manufacturermodelnumber" type="text" class="form-control col-xs-8 span8" data-ng-model="productVariant.manufacturerModelNumber" /></div></div><div class="row-fluid"><div class="form-group col-xs-6 span6"><label class="label-checkbox"><input name="digital" type="checkbox" data-ng-model="productVariant.hasDigitalDownload"> <span><localize key="merchelloVariant_hasDigitalDownload"/></span></label><label class="label-checkbox" data-ng-show="!creatingVariant && !editingVariant"><input name="hasoptions" type="checkbox" data-ng-model="product.hasOptions" data-ng-click="ensureInitialOption()"> <span><localize key="merchelloVariant_hasOptions" /></span></label><label class="label-checkbox"><input name="taxable" type="checkbox" data-ng-model="productVariant.taxable"> <span><localize key="merchelloVariant_isTaxable"/></span></label></div><div class="form-group col-xs-6 span6"><label class="label-checkbox"><input name="trackinventory" type="checkbox" data-ng-model="productVariant.trackInventory" data-ng-disabled="product.hasOptions || productVariant.hasDigitalDownload" data-ng-click="ensureCatalogInventory()"> <span><localize key="merchelloVariant_trackInventory"/></span></label><label class="label-checkbox"><input name="shippable" type="checkbox" data-ng-model="productVariant.shippable"> <span><localize key="merchelloVariant_isShippable" /></span></label></div></div></div>');
    //}]);

    /**
     * @ngdoc directive
     * @name ProductVariantMainProperties
     * @function
     * 
     * @description
     * directive to collect the main information for the product / product variant (sku, price, etc)
     */
    directives.ProductVariantMainProperties = function (merchelloWarehouseService) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                productVariant: '=',
                creatingVariant: '=',
                editingVariant: '='
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Catalog/Directives/product-main-edit.html',
            //templateUrl: 'product-main-edit.html',
            
            link: function($scope, $element) {

                // Get the default warehouse for the ensureCatalogInventory() function below
                $scope.defaultWarehouse = {};
                var promiseWarehouse = merchelloWarehouseService.getDefaultWarehouse();
                promiseWarehouse.then(function (warehouse) {
                    $scope.defaultWarehouse = new merchello.Models.Warehouse(warehouse);
                });

                /**
                 * @ngdoc method
                 * @name ensureCatalogInventory
                 * @function
                 * 
                 * @description
                 * This is called when the "is Shippable" checkbox is checked.  It creates an initial catalog inventory object ready to 
                 * fill out.
                 */
                $scope.ensureCatalogInventory = function () {

                    $scope.productVariant.ensureCatalogInventory($scope.defaultWarehouse);
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

            }

        };
    };

    angular.module("umbraco").directive('productVariantMainProperties', ['merchelloWarehouseService', merchello.Directives.ProductVariantMainProperties]);




    /**
     * @ngdoc directive
     * @name ProductDigitalDownloadSection
     * @function
     * 
     * @description
     * directive to pick the media file or files for digital download
     */
    directives.ProductDigitalDownloadSection = function (dialogService, mediaHelper, mediaResource) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                productVariant: '='
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Catalog/Directives/product-digital-download-section.html',

            link: function ($scope, $element) {

                $scope.id = $scope.productVariant.downloadMediaId;
                if ($scope.productVariant.download && $scope.id != -1) {
                    mediaResource.getById($scope.id).then(function (media) {
                        if (!media.thumbnail) {
                            media.thumbnail = mediaHelper.resolveFile(media, true);
                        }

                        $scope.mediaItem = media;
                        $scope.mediaItem.umbracoFile = mediaHelper.resolveFile(media, false);
                    });
                }

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
                        onlyImages: false,
                        callback: function (media) {

                            if (!media.thumbnail) {
                                media.thumbnail = mediaHelper.resolveFile(media, true);
                            }

                            $scope.mediaItem = media;
                            $scope.mediaItem.umbracoFile = mediaHelper.resolveFile(media, false);
                            $scope.id = media.id;
                            $scope.productVariant.downloadMediaId = media.id;
                        }
                    });

                };
            }

        };
    };

    angular.module("umbraco").directive('productDigitalDownloadSection', ['dialogService', 'mediaHelper', 'mediaResource', merchello.Directives.ProductDigitalDownloadSection]);




    /**
     * @ngdoc directive
     * @name ProductShippingSection
     * @function
     * 
     * @description
     * directive to set the shipping information on a product or product variant
     */
    directives.ProductShippingSection = function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                productVariant: '=',
                defaultWarehouse: '=',
                warehouses: '='
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Catalog/Directives/product-shipping-section.html'
        };
    };

    angular.module("umbraco").directive('productShippingSection', merchello.Directives.ProductShippingSection);




    /**
     * @ngdoc directive
     * @name ProductInventorySection
     * @function
     * 
     * @description
     * directive to set the inventory information on a product or product variant
     */
    directives.ProductInventorySection = function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                productVariant: '=',
                defaultWarehouse: '=',
                warehouses: '='
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Catalog/Directives/product-inventory-section.html'
        };
    };

    angular.module("umbraco").directive('productInventorySection', merchello.Directives.ProductInventorySection);




    /**
     * @ngdoc directive
     * @name ProductVariantBulkInventory
     * @function
     * 
     * @description
     * directive to set the inventory information on all variants to a specific value
     */
    directives.ProductVariantBulkInventory = function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '='
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Catalog/Directives/product-variant-bulk-inventory.html',
        
            link: function ($scope, $element) {

                // helper to bulk set the inventories for the product variants
                $scope.allVariantInventories = 0;

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

            }
        };
    };

    angular.module("umbraco").directive('productVariantBulkInventory', merchello.Directives.ProductVariantBulkInventory);




    /**
     * @ngdoc directive
     * @name ProductVariantCreateTable
     * @function
     * 
     * @description
     * directive to set the show the variants allowed by the options during the create process and allow editing before saving
     */
    directives.ProductVariantCreateTable = function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '='
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Catalog/Directives/product-variants-create-table.html'
        };
    };

    angular.module("umbraco").directive('productVariantCreateTable', merchello.Directives.ProductVariantCreateTable);




    /**
     * @ngdoc directive
     * @name ProductOptionsManage
     * @function
     * 
     * @description
     * directive to add/edit/delete variants from a product during create or edit of a product
     */
    directives.ProductOptionsManage = function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                parentForm: '=',
                classes: '=',
                'update': '&onUpdate'
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Catalog/Directives/product-options-manage.html',

            link: function ($scope, $element) {
                $scope.rebuildVariants = false;

                /**
                 * @ngdoc method
                 * @name addOption
                 * @function
                 * 
                 * @description
                 * Called when the Add Option button is pressed.  Creates a new option ready to fill out.
                 */
                $scope.addOption = function () {
                    $scope.rebuildVariants = true;
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
                    $scope.rebuildVariants = true;
                    $scope.product.removeOption(option);
                };

                /**
                 * @ngdoc method
                 * @name updateOptions
                 * @function
                 * 
                 * @description
                 * Called when the update options button is pressed
                 */
                $scope.updateOptions = function () {
                    $scope.update({ form: $scope.parentForm, rebuild: $scope.rebuildVariants });
                    $scope.rebuildVariants = false;
                };
            }

        };
    };

    angular.module("umbraco").directive('productOptionsManage', merchello.Directives.ProductOptionsManage);



    /**
     * @ngdoc directive
     * @name ProductVariantMainSimpleCreate
     * @function
     * 
     * @description
     * directive to collect the main information for the product create (sku, price, etc)
     */
    directives.ProductVariantMainSimpleCreate = function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                productVariant: '=',
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Catalog/Directives/product-main-simple-create.html'
        };
    };

    angular.module("umbraco").directive('productVariantMainSimpleCreate', merchello.Directives.ProductVariantMainSimpleCreate);


}(window.merchello.Directives = window.merchello.Directives || {}));