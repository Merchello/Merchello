    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.CustomerListController
     * @function
     *
     * @description
     * The controller for customer list view
     */
    angular.module('merchello').controller('Merchello.Backoffice.CustomerListController',
        ['$scope', '$routeParams', 'dialogService', 'notificationsService', 'settingsResource', 'merchelloTabsFactory', 'dialogDataFactory', 'customerResource', 'entityCollectionResource', 'queryDisplayBuilder',
            'queryResultDisplayBuilder', 'customerDisplayBuilder',
        function($scope, $routeParams, dialogService, notificationsService, settingsResource, merchelloTabsFactory, dialogDataFactory, customerResource, entityCollectionResource,
                 queryDisplayBuilder, queryResultDisplayBuilder, customerDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.collectionKey = '';
            $scope.currentPage = 0;
            $scope.customers = [];
            $scope.filterText = '';
            $scope.limitAmount = 25;
            $scope.sortProperty = 'loginName';
            $scope.currentFilters = [];
            $scope.visible = {
                bulkActionButton: function() {
                    var result = false;
                    return result;
                },
                bulkActionDropdown: false
            };
            $scope.currencySymbol = '';

            // exposed methods
            $scope.loadCustomers = loadCustomers;
            $scope.resetFilters = resetFilters;
            $scope.openNewCustomerDialog = openNewCustomerDialog;
            $scope.numberOfPages = numberOfPages;
            $scope.limitChanged = limitChanged;
            $scope.changePage = changePage;
            $scope.changeSortOrder = changeSortOrder;
            $scope.getCurrencySymbol = getCurrencySymbol;

            var maxPages = 0;
            var globalCurrency = '';
            var allCurrencies = [];

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * initialized when the scope loads.
             */
            function init() {
                if($routeParams.id !== 'manage') {
                    $scope.collectionKey = $routeParams.id;
                }
                loadSettings();
                $scope.tabs = merchelloTabsFactory.createCustomerListTabs();
                $scope.tabs.setActive('customerlist');
                $scope.loaded = true;
            }

            function loadSettings() {
                // currency matching
                var currenciesPromise = settingsResource.getAllCurrencies();
                currenciesPromise.then(function(currencies) {
                    allCurrencies = currencies;
                    // default currency
                    var currencySymbolPromise = settingsResource.getCurrencySymbol();
                    currencySymbolPromise.then(function (currencySymbol) {
                        globalCurrency = currencySymbol;
                        loadCustomers($scope.filterText);
                    }, function (reason) {
                        notificationService.error('Failed to load the currency symbol', reason.message);
                    });
                }, function(reason) {
                    notificationService.error('Failed to load all currencies', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadCustomers
             * @function
             *
             * @description
             * Load the customers from the API using the provided filter (if any).
             */
            function loadCustomers(filterText) {
                $scope.preValuesLoaded = false;
                var query = queryDisplayBuilder.createDefault();
                query.currentPage = $scope.currentPage;
                query.itemsPerPage = $scope.limitAmount;
                query.sortBy = sortInfo().sortBy;
                query.sortDirection = sortInfo().sortDirection;
                if (filterText !== $scope.filterText) {
                    query.currentPage = 0;
                    $scope.currentPage = 0;
                }
                query.addFilterTermParam(filterText);

                var promise;
                if ($scope.collectionKey !== '') {
                    query.addCollectionKeyParam($scope.collectionKey);
                    query.addEntityTypeParam('Customer');
                    promise = entityCollectionResource.getCollectionEntities(query);
                } else {
                    promise = customerResource.searchCustomers(query);
                }

                promise.then(function (customersResponse) {
                    $scope.customers = [];
                    var queryResult = queryResultDisplayBuilder.transform(customersResponse, customerDisplayBuilder);
                    $scope.customers = queryResult.items;
                    maxPages = queryResult.totalPages;
                    if(query.parameters.length >= 0) {
                        $scope.currentFilters = _.filter(query.parameters, function(params) {
                            return params.fieldName != 'entityType' && params.fieldName != 'collectionKey'
                        });
                    } else {
                        $scope.currentFilters = [];
                    }
                    $scope.filterText = filterText;
                    $scope.preValuesLoaded = true;

                });
            }

            /**
             * @ngdoc method
             * @name changePage
             * @function
             *
             * @description
             * Changes the current page.
             */
            function changePage(page) {
                $scope.currentPage = page;
                $scope.loadCustomers($scope.filterText);
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
                $scope.loadCustomers($scope.filterText);
            }

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
                $scope.loadCustomers($scope.filterText);
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
                $scope.filterText = "";
                loadCustomers($scope.filterText);
            }

            /**
             * @ngdoc method
             * @name openNewCustomerDialog
             * @function
             *
             * @description
             * Opens the new customer dialog via the Umbraco dialogService.
             */
            function openNewCustomerDialog() {
                var dialogData = dialogDataFactory.createAddEditCustomerDialogData();
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/customer.info.addedit.html',
                    show: true,
                    callback: processNewCustomerDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name processEditInfoDialog
             * @function
             *
             * @description
             * Update the customer info and save.
             */
            function processNewCustomerDialog(dialogData) {
                var customer = customerDisplayBuilder.createDefault();
                customer.loginName = dialogData.email;
                customer.email = dialogData.email;
                customer.firstName = dialogData.firstName;
                customer.lastName = dialogData.lastName;

                var promiseSaveCustomer = customerResource.AddCustomer(customer);
                promiseSaveCustomer.then(function (customerResponse) {
                    notificationsService.success("Customer Saved", "");
                    loadCustomers($scope.filterText);
                }, function (reason) {
                    notificationsService.error("Customer Save Failed", reason.message);
                });
            }

            function numberOfPages() {
                return maxPages;
            }

            /**
             * @ngdoc method
             * @name setVariables
             * @function
             *
             * @description
             * Returns sort information based off the current $scope.sortProperty.
             */
            function sortInfo() {
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
            }

            /**
             * @ngdoc method
             * @name getCurrencySymbol
             * @function
             *
             * @description
             * Utility method to get the currency symbol for an invoice
             */
            function getCurrencySymbol(invoice) {
                if (invoice.currency.symbol !== '' && invoice.currency.symbol !== undefined) {
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

            // Initializes the controller
            init();
    }]);
