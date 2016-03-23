/**
 * @ngdoc controller
 * @name Merchello.Customer.Dialogs.ProductSelectionController
 * @function
 *
 * @description
 * The controller to configure the price component constraint
 */
angular.module('merchello').controller('Merchello.Customer.Dialogs.ProductSelectionFilterController',
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

                    loadProducts();
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
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
                var pi = new ProductItem();
                pi.product = product;
                if (product.hasVariants()) {
                    angular.forEach(product.productVariants, function(pv) {
                        var checked = false;
                        if (variantKeys !== undefined) {
                            var found = _.find(variantKeys, function(key) { return key === pv.key; });
                            if (found) {
                                checked = true;
                            } else {
                                checked = false;
                            }
                        }
                      var vi = new VariantItem();
                        vi.key = pv.key;
                        vi.name = pv.name;
                        vi.sku = pv.sku;
                        vi.checked = checked;
                        pi.selectedVariants.push(vi);
                    });
                }
                $scope.selectedProducts.push(pi);
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
            var ProductItem = function() {
                var self = this;
                self.product = {};
                self.variantSpecific = false;
                self.selectedVariants = [];
                self.exclude = false;
                self.editorOpen = false;
            };

            var VariantItem = function() {
                var self = this;
                self.name = '';
                self.key = '';
                self.sku = '';
                self.checked = false;
            };


            var AddItem = function() {
                var self = this;
                self.key = '';
                self.isProductVariant = false;
            };

            function save() {
                if ($scope.selectedProducts.length === 0) {
                    return;
                }

                angular.forEach($scope.selectedProducts, function(sp) {

                    if(sp.selectedVariants.length > 0) {
                        var variants = _.filter(sp.selectedVariants, function(sv) { return sv.checked; });
                        angular.forEach(variants, function(v) {
                            var variantItem = new AddItem();
                            variantItem.key = v.key;
                            variantItem.isProductVariant = true;
                           $scope.dialogData.addItems.push(variantItem);
                        });
                    } else {
                        // get the master variant key for the product
                        var masterItem = new AddItem();
                        masterItem.key = sp.product.key;
                        $scope.dialogData.addItems.push(masterItem);
                    }

                });

                $scope.submit($scope.dialogData);
            }

            // Initialize the controller
            init();

        }]);

