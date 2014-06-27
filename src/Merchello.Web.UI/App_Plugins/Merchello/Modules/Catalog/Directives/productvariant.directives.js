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
                 * This is called when the "Track inventory for this variant" checkbox is checked.  It creates an initial catalog inventory object ready to 
                 * fill out.
                 */
                $scope.ensureCatalogInventory = function () {

                    $scope.productVariant.ensureCatalogInventory($scope.defaultWarehouse);
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

    directives.ProductDigitalDownloadSection = function (dialogService) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                productVariant: '='
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Catalog/Directives/product-digital-download-section.html',
            //templateUrl: 'product-main-edit.html',

            link: function ($scope, $element) {

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
            }

        };
    };

    angular.module("umbraco").directive('productDigitalDownloadSection', ['dialogService', merchello.Directives.ProductDigitalDownloadSection] );




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
                productVariant: '='
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

}(window.merchello.Directives = window.merchello.Directives || {}));