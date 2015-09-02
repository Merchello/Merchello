'use strict';
/**
 * @ngdoc controller
 * @name Merchello.Dashboards.Sales.ListController
 * @function
 *
 * @description
 * The controller for the orders list page
 */
angular.module('merchello').controller('Merchello.Backoffice.SalesListController',
    ['$scope', '$element', '$log', '$routeParams', 'angularHelper', 'assetsService', 'notificationsService', 'merchelloTabsFactory', 'settingsResource',
        'invoiceResource', 'entityCollectionResource', 'queryDisplayBuilder', 'queryResultDisplayBuilder', 'invoiceDisplayBuilder', 'settingDisplayBuilder',
        function($scope, $element, $log, $routeParams, angularHelper, assetsService, notificationService, merchelloTabsFactory, settingsResource, invoiceResource, entityCollectionResource,
                 queryDisplayBuilder, queryResultDisplayBuilder, invoiceDisplayBuilder, settingDisplayBuilder)
        {

            // expose on scope
            $scope.loaded = true;
            $scope.currentPage = 0;
            $scope.tabs = [];
            $scope.filterText = '';
            $scope.filterStartDate = '';
            $scope.filterEndDate = '';
            $scope.invoices = [];
            $scope.limitAmount = '25';
            $scope.maxPages = 0;
            $scope.orderIssues = [];
            $scope.salesLoaded = true;
            $scope.selectAllOrders = false;
            $scope.selectedOrderCount = 0;
            $scope.settings = {};
            $scope.sortOrder = "desc";
            $scope.sortProperty = "-invoiceNumber";
            $scope.visible = {};
            $scope.visible.bulkActionDropdown = false;
            $scope.currentFilters = [];
            $scope.dateFilterOpen = false;
            $scope.collectionKey = '';
            $scope.showDateFilter = true;
            $scope.entityType = 'Invoice';

            // exposed methods
            $scope.getCurrencySymbol = getCurrencySymbol;
            $scope.resetFilters = resetFilters;
            $scope.toggleDateFilterOpen = toggleDateFilterOpen;

            // for testing
            $scope.itemCount = 0;

            var allCurrencies = [];
            var globalCurrency = '$';

            //--------------------------------------------------------------------------------------
            // Event Handlers
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name changePage
             * @function
             *
             * @description
             * Changes the current page.
             */
            $scope.changePage = function (page) {
                $scope.preValuesLoaded = false;
                $scope.currentPage = page;
                var query = $scope.dateFilterOpen ? buildQuery($scope.filterText, $scope.filterStartDate, $scope.filterEndDate) : buildQuery($scope.filterText);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name changeSortOrder
             * @function
             *
             * @description
             * Helper function to set the current sort on the table and switch the
             * direction if the property is already the current sort column.
             */
            $scope.changeSortOrder = function (propertyToSort) {
                if ($scope.sortProperty == propertyToSort) {
                    if ($scope.sortOrder == "asc") {
                        $scope.sortProperty = "-" + propertyToSort;
                        $scope.sortOrder = "desc";
                    } else {
                        $scope.sortProperty = propertyToSort;
                        $scope.sortOrder = "asc";
                    }
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "asc";
                }
                $scope.currentPage = 0;
                var query = $scope.dateFilterOpen ? buildQuery($scope.filterText, $scope.filterStartDate, $scope.filterEndDate) : buildQuery($scope.filterText);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name limitChanged
             * @function
             *
             * @description
             * Helper function to set the amount of items to show per page for the paging filters and calculations
             */
            $scope.limitChanged = function (newVal) {
                $scope.preValuesLoaded = false;
                $scope.limitAmount = newVal;
                $scope.currentPage = 0;
                var query = $scope.dateFilterOpen ? buildQuery($scope.filterText, $scope.filterStartDate, $scope.filterEndDate) : buildQuery($scope.filterText);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name filterWithDates
             * @function
             *
             * @description
             * Fired when the filter button next to the filter text box at the top of the page is clicked.
             */
            $scope.filterInvoices = function(filterStartDate, filterEndDate, filterText) {
                $scope.preValuesLoaded = false;
                var query = buildQuery(filterText, filterStartDate, filterEndDate);
                loadInvoices(query);
            };

            $scope.termFilterInvoices = function(filterText) {
                $scope.preValuesLoaded = false;
                var query = buildQuery(filterText);
                loadInvoices(query);
            };

            //--------------------------------------------------------------------------------------
            // Helper Methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name setVariables
             * @function
             *
             * @description
             * Returns sort information based off the current $scope.sortProperty.
             */
            $scope.sortInfo = function() {
                var sortDirection, sortBy;
                // If the sortProperty starts with '-', it's representing a descending value.
                if ($scope.sortProperty.indexOf('-') > -1) {
                    // Get the text after the '-' for sortBy
                    sortBy = $scope.sortProperty.split('-')[1];
                    sortDirection = 'Descending';
                    // Otherwise it is ascending.
                } else {
                    sortBy = $scope.sortProperty;
                    sortDirection = 'Ascending';
                }
                return {
                    sortBy: sortBy.toLowerCase(), // We'll want the sortBy all lower case for API purposes.
                    sortDirection: sortDirection
                }
            };

            /**
             * @ngdoc method
             * @name numberOfPages
             * @function
             *
             * @description
             * Helper function to get the amount of items to show per page for the paging
             */
            $scope.numberOfPages = function () {
                return $scope.maxPages;
                //return Math.ceil($scope.products.length / $scope.limitAmount);
            };

            // PRIVATE
            function init() {
                $scope.currencySymbol = '$';
                if($routeParams.id !== 'manage') {
                    $scope.collectionKey = $routeParams.id;
                    $scope.showDateFilter = false;
                }
                resetFilters();
                $scope.tabs = merchelloTabsFactory.createSalesListTabs();
                $scope.tabs.setActive('saleslist');
                $scope.loaded = true;
                $scope.dateFilterOpen = false;
            }

            function loadInvoices(query) {
                $scope.salesLoaded = false;
                $scope.salesLoaded = false;

                var promise;
                if ($scope.collectionKey !== '') {
                    promise = entityCollectionResource.getCollectionEntities(query);
                } else {
                    promise = invoiceResource.searchInvoices(query);
                }
                promise.then(function (response) {
                    var queryResult = queryResultDisplayBuilder.transform(response, invoiceDisplayBuilder);
                    $scope.invoices = queryResult.items;
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                    $scope.salesLoaded = true;
                    $scope.maxPages = queryResult.totalPages;
                    $scope.itemCount = queryResult.totalItems;
                    loadSettings();
                }, function (reason) {
                    notificationsService.error("Failed To Load Invoices", reason.message);
                });

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
                var query = buildQuery();
                $scope.dateFilterOpen = false;
                $scope.currentFilters = [];
                $scope.filterText = '';
                setDefaultDates(new Date());
                loadInvoices(query);
                $scope.filterAction = false;
            };

            /**
             * @ngdoc method
             * @name resetFilters
             * @function
             *
             * @description
             * Fired when the open date filter button is clicked.
             */
            function toggleDateFilterOpen() {
                $scope.dateFilterOpen = !$scope.dateFilterOpen;
            };

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {
                // this is needed for the date format
                var settingsPromise = settingsResource.getAllSettings();
                settingsPromise.then(function(allSettings) {
                    $scope.settings = settingDisplayBuilder.transform(allSettings);
                }, function(reason) {
                    notificationService.error('Failed to load all settings', reason.message);
                });
                // currency matching
                var currenciesPromise = settingsResource.getAllCurrencies();
                currenciesPromise.then(function(currencies) {
                    allCurrencies = currencies;
                }, function(reason) {
                    notificationService.error('Failed to load all currencies', reason.message);
                });
                // default currency
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    globalCurrency = currencySymbol;
                }, function (reason) {
                    notificationService.error('Failed to load the currency symbol', reason.message);
                });
            };

            /**
             * @ngdoc method
             * @name buildQuery
             * @function
             *
             * @description
             * Perpares a new query object for passing to the ApiController
             */
            function buildQuery(filterText, startDate, endDate) {
                var page = $scope.currentPage;
                var perPage = $scope.limitAmount;
                var sortBy = $scope.sortInfo().sortBy;
                var sortDirection = $scope.sortInfo().sortDirection;

                if (filterText === undefined) {
                    filterText = '';
                }
                // back to page 0 if filterText or startDate change
                if (filterText !== $scope.filterText || (startDate !== $scope.filterStartDate && $scope.dateFilterOpen)) {
                    page = 0;
                    $scope.currentPage = 0;
                }

                var dateSearch = false;
                if (startDate !== undefined && endDate !== undefined && $scope.collectionKey === '') {
                    $scope.filterStartDate = startDate;
                    $scope.filterEndDate = endDate;
                    dateSearch = true;
                }

                $scope.filterText = filterText;

                var query = queryDisplayBuilder.createDefault();
                query.currentPage = page;
                query.itemsPerPage = perPage;
                query.sortBy = sortBy;
                query.sortDirection = sortDirection;
                if(dateSearch) {
                    query.addInvoiceDateParam(startDate, 'start');
                    query.addInvoiceDateParam(endDate, 'end');
                }

                if($scope.collectionKey !== '') {
                    query.addCollectionKeyParam($scope.collectionKey);
                    query.addEntityTypeParam('Invoice');
                }

                query.addFilterTermParam(filterText);

                if (query.parameters.length > 0) {
                    $scope.currentFilters = _.filter(query.parameters, function(params) {
                        return params.fieldName != 'entityType' && params.fieldName != 'collectionKey'
                    });
                }
                return query;
            };

            /**
             * @ngdoc method
             * @name getCurrencySymbol
             * @function
             *
             * @description
             * Utility method to get the currency symbol for an invoice
             */
            function getCurrencySymbol(invoice) {

                if (invoice.currency.symbol !== '') {
                    return invoice.currency.symbol;
                }

                var currencyCode = invoice.getCurrencyCode();
                var currency = _.find(allCurrencies, function(currency) {
                    return currency.currencyCode === currencyCode;
                });
                if(currency === null || currency === undefined) {
                    return globalCurrency;
                } else {
                    return currency.symbol;
                }
            }

            /**
            * @ngdoc method
            * @name setDefaultDates
            * @function
            *
            * @description
            * Sets the default dates
            */
            function setDefaultDates(actual) {
                var month = actual.getMonth() == 0 ? 11 : actual.getMonth() - 1;
                var start = new Date(actual.getFullYear(), month, actual.getDate());
                var end = new Date(actual.getFullYear(), actual.getMonth(), actual.getDate());
                $scope.filterStartDate = start.toLocaleDateString();
                $scope.filterEndDate = end.toLocaleDateString();
            }

            init();
        }]);
