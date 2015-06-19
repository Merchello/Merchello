/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintPriceController
 * @function
 *
 * @description
 * The controller to configure the price component constraint
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintProductSelectionFilterController',
    ['$q', '$scope', 'notificationsService', 'productResource', 'settingsResource', 'productDisplayBuilder', 'queryDisplayBuilder', 'queryResultDisplayBuilder',
        function($q, $scope, notificationsService, productResource, settingsResource, productDisplayBuilder, queryDisplayBuilder, queryResultDisplayBuilder) {

            $scope.loaded = false;
            $scope.context = 'display';
            $scope.filterText = "";
            $scope.products = [];
            $scope.filteredproducts = [];
            $scope.watchCount = 0;
            $scope.sortProperty = "name";
            $scope.sortOrder = "Ascending";
            $scope.limitAmount = 10;
            $scope.currentPage = 0;
            $scope.maxPages = 0;

            // dialog properties
            $scope.selectedProducts = [];

            // exposed methods
            $scope.addProduct = addProduct;
            $scope.removeProduct = removeProduct;
            $scope.changePage = changePage;
            $scope.limitChanged = limitChanged;
            $scope.changeSortOrder = changeSortOrder;
            $scope.getFilteredProducts = getFilteredProducts;
            $scope.numberOfPages = numberOfPages;
            $scope.productIsSelected = productIsSelected;
            $scope.save = save;

            //--------------------------------------------------------------------------------------
            // Initialization methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                loadSettings();
            }

            /**
             * @ngdoc method
             * @name loadProducts
             * @function
             *
             * @description
             * Load the products from the product service, then wrap the results
             * in Merchello models and add to the scope via the products collection.
             */
            function loadProducts() {

                var page = $scope.currentPage;
                var perPage = $scope.limitAmount;
                var sortBy = $scope.sortProperty.replace("-", "");
                var sortDirection = $scope.sortOrder;

                var query = queryDisplayBuilder.createDefault();
                query.currentPage = page;
                query.itemsPerPage = perPage;
                query.sortBy = sortBy;
                query.sortDirection = sortDirection;
                query.addFilterTermParam($scope.filterText);

                var promise = productResource.searchProducts(query);
                promise.then(function (response) {
                    var queryResult = queryResultDisplayBuilder.transform(response, productDisplayBuilder);

                    $scope.products = queryResult.items;

                    $scope.maxPages = queryResult.totalPages;
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

                }, function (reason) {
                    notificationsService.success("Products Load Failed:", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;

                    loadExistingConfigurations();
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            function loadExistingConfigurations() {
                var existing = $scope.dialogData.getValue('productConstraints');
                if (existing !== undefined && existing !== '')
                {
                    var parsed = JSON.parse(existing);
                    var productKeys = _.pluck(parsed, 'productKey');

                    var    productsPromise = productResource.getByKeys(productKeys);
                    productsPromise.then(function(result) {
                     var products = productDisplayBuilder.transform(result);
                        angular.forEach(products, function(p) {
                            var constrainData = _.find(parsed, function(cd) { return cd.productKey === p.key; });
                            if(constrainData.specifiedVariants) {
                                addProduct(p, constrainData.variantKeys);
                            } else {
                                addProduct(p);
                            }
                        });
                     loadProducts();
                    });
                } else {
                    loadProducts();
                }

            }

            //--------------------------------------------------------------------------------------
            // Events methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name limitChanged
             * @function
             *
             * @description
             * Helper function to set the amount of items to show per page for the paging filters and calculations
             */
            function limitChanged(newVal) {
                $scope.limitAmount = newVal;
                $scope.currentPage = 0;
                loadProducts();
            }

            /**
             * @ngdoc method
             * @name changePage
             * @function
             *
             * @description
             * Helper function re-search the products after the page has changed
             */
            function changePage (newPage) {
                $scope.currentPage = newPage;
                loadProducts();
            }

            /**
             * @ngdoc method
             * @name changeSortOrder
             * @function
             *
             * @description
             * Helper function to set the current sort on the table and switch the
             * direction if the property is already the current sort column.
             */
            function changeSortOrder(propertyToSort) {

                if ($scope.sortProperty == propertyToSort) {
                    if ($scope.sortOrder == "Ascending") {
                        $scope.sortProperty = "-" + propertyToSort;
                        $scope.sortOrder = "Descending";
                    } else {
                        $scope.sortProperty = propertyToSort;
                        $scope.sortOrder = "Ascending";
                    }
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "Ascending";
                }

                loadProducts();
            }

            /**
             * @ngdoc method
             * @name getFilteredProducts
             * @function
             *
             * @description
             * Calls the product service to search for products via a string search
             * param.  This searches the Examine index in the core.
             */
            function getFilteredProducts(filter) {
                $scope.filterText = filter;
                $scope.currentPage = 0;
                loadProducts();
            }

            function addProduct(product, variantKeys) {
                var pc = new ProductConstraint();
                pc.product = product;
                if (product.hasVariants()) {
                    angular.forEach(product.productVariants, function(pv) {
                        var checked = true;
                        if (variantKeys !== undefined) {
                            var found = _.find(variantKeys, function(key) { return key === pv.key; });
                            if (found) {
                                checked = true;
                            } else {
                                checked = false;
                            }
                        }
                      var vc = new VariantConstraint();
                        vc.key = pv.key;
                        vc.name = pv.name;
                        vc.sku = pv.sku;
                        vc.checked = checked;
                        pc.selectedVariants.push(vc);
                    });
                }
                $scope.selectedProducts.push(pc);
                $scope.context = 'display';
            }

            function removeProduct(constraint) {
                $scope.selectedProducts = _.reject($scope.selectedProducts, function(sp) { return sp.product.key === constraint.product.key; });
            }

            function productIsSelected(product) {
                var pc = _.find($scope.selectedProducts, function(p) { return p.product.key === product.key; });
                return pc !== undefined;
            }

            //--------------------------------------------------------------------------------------
            // Calculations
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name numberOfPages
             * @function
             *
             * @description
             * Helper function to get the amount of items to show per page for the paging
             */
            function numberOfPages() {
                return $scope.maxPages;
            }

            // ---------------------------------------------------------------------------------------
            // Local scope models
            // ---------------------------------------------------------------------------------------
            var ProductConstraint = function() {
                var self = this;
                self.product = {};
                self.variantSpecific = false;
                self.selectedVariants = [];
                self.exclude = false;
                self.editorOpen = false;
            };

            var VariantConstraint = function() {
                var self = this;
                self.name = '';
                self.key = '';
                self.sku = '';
                self.checked = false;
            };


            function save() {
                if ($scope.selectedProducts.length === 0) {
                    return;
                }
                var saveData = [];
                angular.forEach($scope.selectedProducts, function(sp) {
                    var product = {};
                    product.productKey = sp.product.key;
                    product.variantKeys = [];
                    var variants = _.filter(sp.selectedVariants, function(sv) { return sv.checked; });
                    if (variants.length !== sp.product.productVariants.length) {
                        product.specifiedVariants = true;
                        angular.forEach(variants, function(v) {
                            product.variantKeys.push(v.key);
                        });
                    } else {
                        product.specifiedVariants = false;
                    }

                    saveData.push(product);
                });
                $scope.dialogData.setValue('productConstraints', JSON.stringify(saveData));
                $scope.submit($scope.dialogData);
            }

            // Initialize the controller
            init();

        }]);

