    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ProductListController
     * @function
     *
     * @description
     * The controller for product list view controller
     */
    angular.module('merchello').controller('Merchello.Backoffice.ProductListController',
        ['$scope', '$routeParams', '$location', 'assetsService', 'notificationsService', 'settingsResource', 'entityCollectionResource',
            'merchelloTabsFactory', 'dialogDataFactory', 'productResource', 'productDisplayBuilder',
            'queryDisplayBuilder', 'queryResultDisplayBuilder',
        function($scope, $routeParams, $location, assetsService, notificationsService, settingsResource, entityCollectionResource,
                 merchelloTabsFactory, dialogDataFactory, productResource, productDisplayBuilder,
        queryDisplayBuilder, queryResultDisplayBuilder) {

            $scope.filterText = '';
            $scope.tabs = [];
            $scope.products = [];
            $scope.currentFilters = [];
            $scope.sortProperty = 'name';
            $scope.sortOrder = 'Ascending';
            $scope.limitAmount = 25;
            $scope.currentPage = 0;
            $scope.maxPages = 0;

            // collections
            $scope.collectionKey = '';

            // exposed methods
            $scope.getEditUrl = getEditUrl;
            $scope.limitChanged = limitChanged;
            $scope.numberOfPages = numberOfPages;
            $scope.changePage = changePage;
            $scope.changeSortOrder = changeSortOrder;
            $scope.getFilteredProducts = getFilteredProducts;
            $scope.resetFilters = resetFilters;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                if($routeParams.id !== 'manage') {
                    $scope.collectionKey = $routeParams.id;
                }

                loadProducts();
                loadSettings();
                $scope.tabs = merchelloTabsFactory.createProductListTabs();
                $scope.tabs.setActive('productlist');
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
                $scope.currentFilters = _.filter(query.parameters, function(params) {
                    return params.fieldName != 'entityType' && params.fieldName != 'collectionKey'
                });

                var promise;
                if ($scope.collectionKey !== '') {
                    query.addCollectionKeyParam($scope.collectionKey);
                    query.addEntityTypeParam('Product');
                    var promise = entityCollectionResource.getCollectionEntities(query);
                } else {
                    var promise = productResource.searchProducts(query);
                }

                promise.then(function (response) {
                    var queryResult = queryResultDisplayBuilder.transform(response, productDisplayBuilder);
                    $scope.products = queryResult.items;
                    $scope.maxPages = queryResult.totalPages;
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function(reason) {
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
                currencySymbolPromise.then(function(currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
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
            function changePage(newPage) {
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
                $scope.preValuesLoaded = false;
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

            /**
             * @ngdoc method
             * @name resetFilters
             * @function
             *
             * @description
             * Fired when the reset filter button is clicked.
             */
            function resetFilters() {
                $scope.preValuesLoaded = false;
                $scope.currentFilters = [];
                $scope.filterText = '';
                loadProducts();
            }

            function getEditUrl(product) {
                return "#/merchello/merchello/productedit/" + product.key;
                //return product.hasVariants() ? "#/merchello/merchello/producteditwithoptions/" + product.key :
                //    "#/merchello/merchello/productedit/" + product.key;
            }

            // Initialize the controller
            init();

        }]);
