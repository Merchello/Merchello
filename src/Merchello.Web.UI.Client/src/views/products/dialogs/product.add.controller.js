/**
 * @ngdoc controller
 * @name Merchello.Product.Dialogs.ProductAddController
 * @function
 *
 * @description
 * The controller for the adding new products and allows for associating product content types
 */
angular.module('merchello').controller('Merchello.Product.Dialogs.ProductAddController',
    ['$scope', '$q', '$location', 'notificationsService', 'navigationService', 'contentResource', 'productResource','warehouseResource', 'settingsResource',
        'detachedContentResource', 'productDisplayBuilder', 'productVariantDetachedContentDisplayBuilder', 'warehouseDisplayBuilder',
        function($scope, $q, $location, notificationsService, navigationService, contentResource, productResource, warehouseResource, settingsResource,
                 detachedContentResource, productDisplayBuilder, productVariantDetachedContentDisplayBuilder, warehouseDisplayBuilder) {
            $scope.loaded = false;
            $scope.wasFormSubmitted = false;
            $scope.name = '';
            $scope.sku = '';
            $scope.price = 0;
            $scope.contentType = {};
            $scope.checking = false;
            $scope.isUnique = true;
            $scope.product = {};
            $scope.productVariant = {};
            $scope.languages = [];
            $scope.settings = {};
            $scope.currencySymbol = '';
            $scope.defaultWarehouseCatalog = {};
            
            // grab the sku text box to test if sku is unique
            var input = angular.element( document.querySelector( '#sku' ) );
            var currentSku = '';

            $scope.save = function() {
                $scope.wasFormSubmitted = true;
                if ($scope.addProductForm.name.$valid && $scope.addProductForm.sku.$valid && $scope.isUnique) {
                    // verify we have a detached content type to add to the variant
                    if ($scope.contentType !== undefined && $scope.contentType.umbContentType !== undefined) {
                        createDetachedContent().then(function(detached) {
                            $scope.product.detachedContents = detached;
                            finalize();
                        });
                    } else {
                        finalize();
                    }
                }


            }

            function finalize() {
                $scope.product.shippable = $scope.settings.globalShippable;
                $scope.product.taxable = $scope.settings.globalTaxable;
                $scope.product.trackInventory = $scope.settings.globalTrackInventory;
                if($scope.product.shippable || $scope.product.trackInventory)
                {
                    $scope.product.ensureCatalogInventory($scope.defaultWarehouseCatalog.key);
                }

                // make the API call to create the product
                productResource.create($scope.product).then(function(np) {
                    navigationService.hideNavigation();
                    $location.url('/merchello/merchello/productedit/' + np.key, true);
                });
            }

            function init() {
                // we need to get all the languages configured in Umbraco so that we can
                // create detached content for each one. We also need the currency symbol so we 
                // can append it to the price textbox
                $q.all([
                    detachedContentResource.getAllLanguages(),
                    settingsResource.getAllCombined(),
                    warehouseResource.getDefaultWarehouse()
                ]).then(function(data) {
                    var langArray = [];
                    if (!angular.isArray(data[0])) {
                        langArray.push(data[0]);
                    } else {
                        langArray = _.sortBy(data[0], 'name');
                    }
                    $scope.languages = langArray;
                    $scope.currencySymbol = data[1].currencySymbol;
                    $scope.settings = data[1].settings;
                    var defaultWarehouse = warehouseDisplayBuilder.transform(data[2]);
                    $scope.defaultWarehouseCatalog = _.find(defaultWarehouse.warehouseCatalogs, function (dwc) { return dwc.isDefault; });
                    $scope.loaded = true;
                });

                $scope.product = productDisplayBuilder.createDefault();

                input.bind("keyup onfocusout", function (event) {
                    var code = event.which;
                    // alpha , numbers, ! and backspace

                    if ( code === 45 ||
                        (code >47 && code <58) ||
                        (code >64 && code <91) ||
                        (code >96 && code <123) || code === 33 || code == 8) {
                        $scope.$apply(function () {
                            if ($scope.product.sku !== '') {
                                checkUniqueSku($scope.product.sku);
                            }
                        });
                    } else {
                        event.preventDefault();
                    }
                });
            }

            function createDetachedContent() {
                
                var deferred = $q.defer();

                var detachedContents = [];
                // we need to add a new productvariant detached content for each language configured
                var isoCodes =  _.pluck($scope.languages, 'isoCode');
                contentResource.getScaffold(-20, $scope.contentType.umbContentType.alias).then(function(scaffold) {
                    var contentTabs = _.filter(scaffold.tabs, function(t) { return t.id !== 0 });
                    angular.forEach(isoCodes, function(cultureName) {
                        var productVariantContent = buildProductVariantDetachedContent(cultureName, $scope.contentType, contentTabs);
                        detachedContents.push(productVariantContent);
                    });

                    deferred.resolve(detachedContents);
                });
                
                return deferred.promise;
            }

            function buildProductVariantDetachedContent(cultureName, detachedContent, tabs) {
                var productVariantContent = productVariantDetachedContentDisplayBuilder.createDefault();
                productVariantContent.cultureName = cultureName;
                productVariantContent.detachedContentType = detachedContent;
                productVariantContent.canBeRendered = true;
                angular.forEach(tabs, function(tab) {
                    angular.forEach(tab.properties, function(prop) {
                        //productVariantContent.detachedDataValues.setValue(prop.alias, angular.toJson(prop.value));
                        productVariantContent.detachedDataValues.setValue(prop.alias, '');
                    })
                });
                return productVariantContent;
            }
            
            function checkUniqueSku(sku) {
                $scope.checking = true;
                if (sku === undefined || sku === '') {
                    $scope.checking = false;
                    $scope.isUnique = true;
                } else {

                    if (sku === currentSku) {
                        $scope.checking = false;
                        return true;
                    }
                    var checkPromise = productResource.getSkuExists(sku).then(function(exists) {
                            $scope.checking = false;
                            currentSku = sku;
                            $scope.isUnique = exists === 'false';
                            $scope.checking = false;
                    });
                }
            }

            init();
        }]);
