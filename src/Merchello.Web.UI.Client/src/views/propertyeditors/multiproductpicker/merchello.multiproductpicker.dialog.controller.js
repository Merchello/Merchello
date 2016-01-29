angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloMultiProductPickerDialogController',
    ['$scope', '$q', 'productResource', 'settingsResource', 'settingDisplayBuilder', 'queryDisplayBuilder', 'queryResultDisplayBuilder', 'productDisplayBuilder',
    function($scope, $q, productResource, settingsResource, settingDisplayBuilder, queryDisplayBuilder, queryResultDisplayBuilder, productDisplayBuilder) {

        $scope.loaded = true;
        $scope.currencySymbol = '';

        $scope.sortProperty = '';
        $scope.sortOrder = 'Ascending';
        $scope.filterText = '';
        $scope.limitAmount = 5;
        $scope.currentPage = 0;

        // exposed methods
        $scope.changePage = changePage;
        $scope.limitChanged = limitChanged;
        $scope.changeSortOrder = changeSortOrder;
        $scope.numberOfPages = numberOfPages;
        $scope.handleProduct = handleProduct;
        $scope.getFilteredProducts = getFilteredProducts;

        function init() {
            loadSettings();
        }

        function loadProducts() {

            $scope.preValuesLoaded = false;
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

                selectProduct($scope.products);

                $scope.maxPages = queryResult.totalPages;
                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.success("Products Load Failed:", reason.message);
            });


        }

        /**
         * @ngdoc method
         * @name selectProduct
         * @function
         *
         * @description
         * Helper to set the selected bool on a product if it is the currently selected product
         */
        function selectProduct(products) {
            if ($scope.dialogData.products.length === 0) return;

            _.each($scope.products, function(p) {
               var displayed = _.find($scope.dialogData.products, function(dp) {
                  return dp.key === p.key;
               });

                if (displayed !== null && displayed !== undefined) {
                    p.selected = true;
                } else {
                    p.selected = false;
                }

            });
        }

        function handleProduct(product) {
            if (product.selected) {
                $scope.dialogData.products = _.reject($scope.dialogData.products, function(dp) {
                    return dp.key === product.key;
                });
            } else {
                $scope.dialogData.products.push(product);
            }

            loadProducts();
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
            // this is needed for the date format
            var settingsPromise = settingsResource.getAllSettings();
            settingsPromise.then(function(allSettings) {
                $scope.settings = settingDisplayBuilder.transform(allSettings);
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                    loadProducts();
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }, function(reason) {
                notificationService.error('Failed to load all settings', reason.message);
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

        function getFilteredProducts(filter) {
            $scope.filterText = filter;
            $scope.currentPage = 0;
            loadProducts();
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

        init();

    }]);
