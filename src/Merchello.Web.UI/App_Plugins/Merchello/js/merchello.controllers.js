/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */

(function() { 

    /**
     * @ngdoc controller
     * @name Merchello.Common.Dialogs.DeleteConfirmationController
     * @function
     *
     * @description
     * The controller for the delete confirmations
     */
    angular.module('merchello')
        .controller('Merchello.Common.Dialogs.DeleteConfirmationController',
        ['$scope', function($scope) {

        }]);

    /**
     * @ngdoc controller
     * @name Merchello.Common.Dialogs.EditAddressController
     * @function
     * 
     * @description
     * The controller for adding a country
     */
    angular.module('merchello')
        .controller('Merchello.Common.Dialogs.EditAddressController',
        function ($scope) {

            // public methods
            $scope.save = save;

            function init() {
                $scope.address = $scope.dialogData.address;
            };

            function save() {
                $scope.dialogData.address.countryCode = $scope.dialogData.selectedCountry.countryCode;
                if($scope.dialogData.selectedCountry.provinces.length > 0) {
                    $scope.dialogData.address.region = $scope.dialogData.selectedProvince.code;
                }
                $scope.submit($scope.dialogData);
            };

        init();
    });


    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.CustomerListController
     * @function
     *
     * @description
     * The controller for customer list view
     */
    angular.module('merchello').controller('Merchello.Backoffice.CustomerListController',
        ['$scope', 'dialogService', 'notificationsService', 'merchelloTabsFactory', 'customerResource', 'queryDisplayBuilder',
            'queryResultDisplayBuilder', 'customerDisplayBuilder',
        function($scope, dialogService, notificationsService, merchelloTabsFactory, customerResource,
                 queryDisplayBuilder, queryResultDisplayBuilder, customerDisplayBuilder) {

            $scope.loaded = true;
            $scope.preValuesLoaded = true;

            $scope.currentPage = 0;
            $scope.customers = [];
            $scope.filterText = '';
            $scope.limitAmount = 25;
            $scope.maxPages = 0;
            $scope.sortProperty = 'loginName';
            $scope.visible = {
                bulkActionButton: function() {
                    var result = false;
                    return result;
                },
                bulkActionDropdown: false
            };

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * initialized when the scope loads.
             */
            function init() {
                loadCustomers($scope.filterText);
                $scope.tabs = merchelloTabsFactory.createCustomerListTabs();
                $scope.tabs.setActive('customerlist');
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

                var promiseAllCustomers = customerResource.searchCustomers(query);
                promiseAllCustomers.then(function (customersResponse) {
                    $scope.customers = [];
                    var queryResult = queryResultDisplayBuilder.transform(customersResponse, customerDisplayBuilder);
                    $scope.customers = queryResult.items;
                    console.info($scope.customers);
                    $scope.maxPages = queryResult.totalPages;

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
             * @name loadMostRecentOrders
             * @function
             *
             * @description
             * Iterate through all the customers in the list, and acquire their most recent order.
             */
            function loadMostRecentOrders() {
                _.each($scope.customers, function (customer) {
                    var promiseOrder = merchelloInvoiceService.getByCustomerKey(customer.key);
                    promiseOrder.then(function (response) {
                        // TODO: Finish function acquiring the most recent order total for each customer once the merchelloInvoiceService.getByCustomerKey API endpoint returns valid results.
                    });
                });
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
                var dialogData = {
                    firstName: '',
                    lastName: '',
                    email: ''
                };
                dialogService.open({
                    template: '/App_Plugins/Merchello/Modules/Customer/Dialogs/customer.editinfo.html',
                    show: true,
                    callback: $scope.processNewCustomerDialog,
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
            function processNewCustomerDialog(data) {
                var newCustomer = new merchello.Models.Customer();
                newCustomer.firstName = data.firstName;
                newCustomer.lastName = data.lastName;
                newCustomer.email = data.email;
                newCustomer.loginName = data.email;
                var promiseSaveCustomer = merchelloCustomerService.AddCustomer(newCustomer);
                promiseSaveCustomer.then(function (customerResponse) {
                    notificationsService.success("Customer Saved", "");
                    $scope.init();
                }, function (reason) {
                    notificationsService.error("Customer Save Failed", reason.message);
                });
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

            // Initializes the controller
            init();
    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.CustomerOverviewController
     * @function
     *
     * @description
     * The controller for customer overview view
     */
    angular.module('merchello').controller('Merchello.Backoffice.CustomerOverviewController',
        ['$scope', '$routeParams', 'dialogService', 'notificationsService', 'settingsResource', 'invoiceHelper', 'merchelloTabsFactory', 'dialogDataFactory',
            'customerResource', 'customerDisplayBuilder', 'countryDisplayBuilder', 'currencyDisplayBuilder', 'settingDisplayBuilder',
        function($scope, $routeParams, dialogService, notificationsService, settingsResource, invoiceHelper, merchelloTabsFactory, dialogDataFactory,
                 customerResource, customerDisplayBuilder, countryDisplayBuilder, currencyDisplayBuilder, settingDisplayBuilder) {

            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.tabs = [];
            $scope.avatarUrl = "";
            $scope.billingAddresses = [];
            $scope.countries = [];
            $scope.customer = {};
            $scope.filters = {
                province: {}
            };
            $scope.invoiceTotals = [];

            // exposed methods
            $scope.getCurrency = getCurrency;

            // private properties
            var settings = {};
            var defaultCurrency = {};
            var countries = [];
            var currencies = [];

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Inititalizes the scope.
             */
            function init() {
                var key = $routeParams.id;
                loadSettings();
                $scope.tabs = merchelloTabsFactory.createCustomerOverviewTabs(key);
                $scope.tabs.setActive('overview');
                loadCustomer(key);
            }

            /**
             * @ngdoc method
             * @name loadCustomer
             * @function
             *
             * @description
             * Load the customer information if needed.
             */
            function loadCustomer(key) {
                var promiseLoadCustomer = customerResource.GetCustomer(key);
                promiseLoadCustomer.then(function(customerResponse) {
                    $scope.customer = customerDisplayBuilder.transform(customerResponse);
                    $scope.invoiceTotals = invoiceHelper.getTotalsByCurrencyCode($scope.customer.invoices);
                    console.info($scope.invoiceTotals);
                    $scope.loaded = true;
                    //$scope.avatarUrl = merchelloGravatarService.avatarUrl($scope.customer.email);
                }, function(reason) {
                    notificationsService.error("Failed to load customer", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadCountries
             * @function
             *
             * @description
             * Load a list of countries and provinces from the API.
             */
            function loadSettings() {
                // gets all of the countries
                var promiseCountries = settingsResource.getAllCountries();
                promiseCountries.then(function(countriesResponse) {
                    countries = countryDisplayBuilder.transform(countriesResponse);
                });

                // gets all of the settings
                var promiseSettings = settingsResource.getAllSettings();
                promiseSettings.then(function(settingsResponse) {
                    settings = settingDisplayBuilder.transform(settingsResponse);

                    // we need all of the currencies since invoices may be billed in various currencies
                    var promiseCurrencies = settingsResource.getAllCurrencies();
                    promiseCurrencies.then(function(currenciesResponse) {
                        currencies = currencyDisplayBuilder.transform(currenciesResponse);

                        // get the default currency from the settings in case we cannot determine
                        // the currency used in an invoice
                        defaultCurrency = _.find(currencies, function(c) {
                            return c.currencyCode === settings.currencyCode;
                        });
                    });
                });
            }

            /**
             * @ngdoc method
             * @name openAddressEditDialog
             * @function
             *
             * @description
             * Opens the edit address dialog via the Umbraco dialogService.
             */
            function openAddressEditDialog(type) {
                var dialogData = {};
                dialogData.addressToReturn = new merchello.Models.CustomerAddress();
                dialogData.countries = $scope.countries;
                dialogData.shouldDelete = false;
                $scope.currentAddress = new merchello.Models.CustomerAddress();
                dialogData.addressType = type;
                if (type == 'billing') {
                    dialogData.addresses = _.map($scope.billingAddresses, function (billingAddress) {
                        return new merchello.Models.CustomerAddress(billingAddress);
                    });
                } else {
                    dialogData.addresses = _.map($scope.shippingAddresses, function (shippingAddress) {
                        return new merchello.Models.CustomerAddress(shippingAddress);
                    });
                }
                var aliases = [];
                for (var i = 0; i < dialogData.addresses.length; i++) {
                    var address = dialogData.addresses[i];
                    var alias = address.label;
                    aliases.push({ id: i, name: alias });
                };
                aliases.unshift({ id: -1, name: 'New Address' });
                dialogData.addressAliases = aliases;
                dialogData.addresses.unshift(new merchello.Models.CustomerAddress());
                dialogData.type = [
                    { id: 0, name: 'Billing' },
                    { id: 1, name: 'Shipping' }
                ];
                dialogData.filters = {
                    address: dialogData.addressAliases[0],
                    country: $scope.countries[0],
                    province: {},
                    type: {}
                };
                if (type === 'billing') {
                    dialogData.filters.type = dialogData.type[0];
                } else {
                    dialogData.filters.type = dialogData.type[1];
                }
                dialogService.open({
                    template: '/App_Plugins/Merchello/Modules/Customer/Dialogs/customer.editaddress.html',
                    show: true,
                    callback: $scope.processEditAddressDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name openDeleteCustomerDialog
             * @function
             *
             * @description
             * Opens the delete customer dialog via the Umbraco dialogService.
             */
            function openDeleteCustomerDialog() {
                var dialogData = {};
                dialogData.name = $scope.customer.firstName + ' ' + $scope.customer.lastName;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
                    show: true,
                    callback: $scope.processDeleteCustomerDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name openEditInfoDialog
             * @function
             *
             * @description
             * Opens the edit customer info dialog via the Umbraco dialogService.
             */
            function openEditInfoDialog() {
                var dialogData = {
                    firstName: $scope.customer.firstName,
                    lastName: $scope.customer.lastName,
                    email: $scope.customer.email
                };
                dialogService.open({
                    template: '/App_Plugins/Merchello/Modules/Customer/Dialogs/customer.editinfo.html',
                    show: true,
                    callback: $scope.processEditInfoDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name prepareAddressesForSave
             * @function
             *
             * @description
             * Prepare a list of addresses to save with the customer
             */
            function prepareAddressesForSave() {
                var addresses = [];
                _.each($scope.billingAddresses, function(address) {
                    addresses.push(new merchello.Models.CustomerAddress(address));
                });
                _.each($scope.shippingAddresses, function(address) {
                    addresses.push(new merchello.Models.CustomerAddress(address));
                });
                return addresses;
            };

            /**
             * @ngdoc method
             * @name processEditAddressDialog
             * @function
             *
             * @description
             * Edit an address and update the associated lists.
             */
            function processEditAddressDialog(data) {
                var addresses = data.addresses;
                //  Filter out an address if it's marked to be deleted.
                if (data.shouldDelete) {
                    addresses = _.reject(addresses, function(address) {
                        return address.key == data.addressToReturn.key;
                    });
                }
                // Insert the applicable customer, billing, and shipping keys and types into new addresses.
                _.each(addresses, function(address) {
                    address.customerKey = $scope.customer.key;
                    if (data.addressType.toLowerCase() === 'billing') {
                        address.addressType = 'Billing';
                    } else {
                        address.addressType = 'Shipping';
                    }
                });
                // Update the appropriate address list.
                if (data.addressType.toLowerCase() === 'billing') {
                    $scope.billingAddresses = _.map(addresses, function(address) {
                        return new merchello.Models.CustomerAddress(address);
                    });
                } else {
                    $scope.shippingAddresses = _.map(addresses, function (address) {
                        return new merchello.Models.CustomerAddress(address);
                    });
                }
                notificationsService.info("Preparing addresses for updating...", "");
                // Combine the address lists and update the customer.
                $scope.customer.addresses = $scope.prepareAddressesForSave();
                $scope.saveCustomer();
            }

            /**
             * @ngdoc method
             * @name processDeleteCustomerDialog
             * @function
             *
             * @description
             * Delete a customer.
             */
            function processDeleteCustomerDialog(dialogData) {
                notificationsService.info("Deleting " + $scope.customer.firstName + " " + $scope.customer.lastName, "");
                var promiseDeleteCustomer = merchelloCustomerService.DeleteCustomer($scope.customer.key);
                promiseDeleteCustomer.then(function() {
                    notificationsService.success("Customer deleted.", "");
                    window.location.hash = "#/merchello/merchello/CustomerList/manage";
                }, function(reason) {
                    notificationsService.error("Customer Deletion Failed", reason.message);
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
            function processEditInfoDialog(data) {
                $scope.customer.firstName = data.firstName;
                $scope.customer.lastName = data.lastName;
                $scope.customer.email = data.email;
                $scope.saveCustomer();
            }

            /**
             * @ngdoc method
             * @name saveCustomer
             * @function
             *
             * @description
             * Save the customer with the new note.
             */
            function saveCustomer() {
                notificationsService.info("Saving...", "");
                var promiseSaveCustomer = merchelloCustomerService.SaveCustomer($scope.customer);
                promiseSaveCustomer.then(function(customerResponse) {
                    $scope.customer = new merchello.Models.Customer(customerResponse);
                    notificationsService.success("Customer Saved", "");
                    $scope.getDefaultAddresses();
                }, function(reason) {
                    notificationsService.error("Customer  Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name getCurrencySymbol
             * @function
             *
             * @description
             * Gets the currency symbol for an invoice.
             */
            function getCurrency(currencyCode) {
                var currency = _.find(currencies, function(c) {
                    return c.currencyCode === currencyCode;
                });
                if (currency === null || currency === undefined) {
                    currency = defaultCurrency;
                }
                return currency;
            }

            // Initializes the controller
            init();
    }]);

/**
 * @ngdoc controller
 * @name Merchello.Backoffice.SettingsController
 * @function
 *
 * @description
 * The controller for the settings management page
 */
angular.module('merchello').controller('Merchello.Backoffice.SettingsController',
    ['$scope', '$log', 'serverValidationManager', 'notificationsService', 'settingsResource', 'settingDisplayBuilder',
        'currencyDisplayBuilder', 'countryDisplayBuilder',
        function($scope, $log, serverValidationManager, notificationsService, settingsResource, settingDisplayBuilder, currencyDisplayBuilder) {

            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.savingStoreSettings = false;
            $scope.settingsDisplay = settingDisplayBuilder.createDefault();
            $scope.currencies = [];
            $scope.selectedCurrency = {};

            // exposed methods
            $scope.currencyChanged = currencyChanged;
            $scope.save = save;

            function loadCurrency() {
                var promise = settingsResource.getAllCurrencies();
                promise.then(function(currenices) {
                    $scope.currencies = _.sortBy(currencyDisplayBuilder.transform(currenices), function(currency) {
                        return currency.name;
                    });
                    $scope.selectedCurrency = _.find($scope.currencies, function(currency) {
                      return currency.currencyCode === $scope.settingsDisplay.currencyCode;
                    });

                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    alert('Failed: ' + reason.message);
                });
            }

            function loadSettings() {
                var promise = settingsResource.getCurrentSettings();
                promise.then(function (settings) {
                    $scope.settingsDisplay = settingDisplayBuilder.transform(settings);
                    loadCurrency();
                }, function (reason) {
                    alert('Failed: ' + reason.message);
                });
            }

            function save () {
                $scope.preValuesLoaded = false;

                notificationsService.info("Saving...", "");
                $scope.savingStoreSettings = true;
                $scope.$watch($scope.storeSettingsForm, function(value) {
                    var promise = settingsResource.save($scope.settingsDisplay);
                    promise.then(function(settingDisplay) {
                        notificationsService.success("Store Settings Saved", "");
                        $scope.savingStoreSettings = false;
                        $scope.settingDisplay = settingDisplayBuilder.transform(settingDisplay);
                        loadSettings();
                    }, function(reason) {
                        notificationsService.error("Store Settings Save Failed", reason.message);
                    });
                });

            }

            function currencyChanged(currency) {
                $scope.settingsDisplay.currencyCode = currency.currencyCode;
            }

            loadSettings();
}]);

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProviders.Dialogs.NotificationsMessageAddController
     * @function
     *
     * @description
     * The controller for the adding / editing Notification messages on the Notifications page
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.NotificationsMessageAddController',
        ['$scope',
        function($scope) {

            // exposed methods
            $scope.save = save;

            console.info($scope.dialogData);

            function save() {
                $scope.dialogData.notificationMessage.monitorKey = $scope.dialogData.selectedMonitor.monitorKey;
                $scope.submit($scope.dialogData);
            }

    }]);

angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.NotificationsMethodAddEditController',
    ['$scope',
    function($scope) {

}]);

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProvider.Dialogs.NotificationsProviderSettingsSmtpController
     * @function
     *
     * @description
     * The controller for configuring the SMTP provider
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.NotificationsProviderSettingsSmtpController',
        ['$scope', function($scope) {

            $scope.notificationProviderSettings = {};

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                if ($scope.dialogData.provider.extendedData.items.length > 0) {
                    var extendedDataKey = 'merchSmtpProviderSettings';
                    var settingsString = $scope.dialogData.provider.extendedData.getValue(extendedDataKey);
                    $scope.notificationProviderSettings = angular.fromJson(settingsString);
                    console.info($scope.dialogData);
                    console.info($scope.notificationProviderSettings);

                    // Watch with object equality to convert back to a string for the submit() call on the Save button
                    $scope.$watch(function () {
                        return $scope.notificationProviderSettings;
                    }, function (newValue, oldValue) {
                        $scope.dialogData.provider.extendedData.setValue(extendedDataKey, angular.toJson(newValue));
                    }, true);
                }
            }

            // Initialize
            init();

        }]);

    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.PaymentMethodAddEditController',
        ['$scope',
            function($scope) {

        }]);

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProvider.Dialogs.ShippingAddCountryController
     * @function
     *
     * @description
     * The controller for associating countries with shipping providers and warehouse catalogs
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.ShippingAddCountryController',
        ['$scope', function($scope) {



        }]);

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProviders.Dialogs.ShippingFixedRateShipMethodController
     * @function
     *
     * @description
     * The controller for configuring a fixed rate ship method
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.ShippingFixedRateShipMethodController',
        ['$scope', 'notificationsService',
        'shippingFixedRateProviderResource', 'shippingGatewayProviderResource',
        'shipFixedRateTableDisplayBuilder', 'shipRateTierDisplayBuilder',
        function($scope, notificationsService, shippingFixedRateProviderResource, shippingGatewayProviderResource,
                 shipFixedRateTableDisplayBuilder, shipRateTierDisplayBuilder) {

            $scope.loaded = false;
            $scope.isAddNewTier = false;
            $scope.newTier = {};
            $scope.filters = {};
            $scope.rateTable = {}; //shipFixedRateTableDisplayBuilder.createDefault();
            $scope.rateTable.shipMethodKey = ''; //$scope.dialogData.method.key;

            // exposed methods
            $scope.addRateTier = addRateTier;
            $scope.insertRateTier = insertRateTier;
            $scope.cancelRateTier = cancelRateTier;
            $scope.save = save;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Runs when the scope is initialized.
             */
            function init() {
                getRateTable();
            }

            /**
             * @ngdoc method
             * @name getRateTableIfRequired
             * @function
             *
             * @description
             * Get the rate table if it exists.
             */
            function getRateTable() {
                var promise = shippingFixedRateProviderResource.getRateTable($scope.dialogData.shippingGatewayMethod.shipMethod);
                promise.then(function(rateTable) {
                    $scope.rateTable = shipFixedRateTableDisplayBuilder.transform(rateTable);
                    $scope.rateTable.shipMethodKey = $scope.dialogData.shippingGatewayMethod.getKey();
                    $scope.loaded = true;
                }, function(reason) {
                    notificationsService.error('Could not retrieve rate table', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name addRateTier
             * @function
             *
             * @description
             * Adds the edited, new rate tier to the method.
             */
            function addRateTier() {
                $scope.rateTable.addRow($scope.newTier);
                $scope.isAddNewTier = false;
            }

            /**
             * @ngdoc method
             * @name insertRateTier
             * @function
             *
             * @description
             * Inserts a new, blank row in the rate table.
             */
            function insertRateTier() {
                $scope.isAddNewTier = true;
                $scope.newTier = shipRateTierDisplayBuilder.createDefault();
            }

            /**
             * @ngdoc method
             * @name cancelRateTier
             * @function
             *
             * @description
             * Cancels the insert of a new blank row in the rate table.
             */
            function cancelRateTier() {
                $scope.isAddNewTier = false;
                $scope.newTier = {};
            }


            /**
             * @ngdoc method
             * @name removeRateTier
             * @function
             *
             * @description
             * Remove a rate tier from the method.
             */
            function removeRateTier(tier) {
                $scope.rateTable.removeRow(tier);
            }

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Saves the rate table and then submits.
             */
            function save() {
                var promiseSaveRateTable = shippingFixedRateProviderResource.saveRateTable($scope.rateTable);
                promiseSaveRateTable.then(function() {
                    $scope.submit($scope.dialogData);
                }, function(reason) {
                    notificationsService.error('Rate Table Save Failed', reason.message);
                });
            }

            // Initializes the controller
            init();
    }]);

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProviders.Dialogs.ShippingAddCountryProviderController
     * @function
     *
     * @description
     * The controller for the adding / editing shipping providers on the Shipping page
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.ShippingAddCountryProviderController',
        function($scope) {
        console.info($scope.dialogData);
    });

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProviders.Dialogs.ShipMethodRegionsController
     * @function
     *
     * @description
     * The controller for the adding / editing ship methods regions
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.ShipMethodRegionsController',
        ['$scope', function($scope) {

            $scope.allProvinces = false;

            // exposed methods
            $scope.toggleAllProvinces = toggleAllProvinces;


            /**
             * @ngdoc method
             * @name toggleAllProvinces
             * @function
             *
             * @description
             * Toggle the provinces.
             */
            function toggleAllProvinces() {
                _.each($scope.dialogData.shippingGatewayMethod.shipMethod.provinces, function (province)
                {
                    province.allowShipping = $scope.allProvinces;
                });
            }
    }]);

angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.WarehouseAddEditController',
    ['$scope',
    function($scope) {

        // exposed methods
        $scope.save = save;

        function save() {
            if($scope.dialogData.selectedCountry.provinces.length > 0) {
                $scope.dialogData.warehouse.region = $scope.dialogData.selectedProvince.code;
            }
            $scope.dialogData.warehouse.countryCode = $scope.dialogData.selectedCountry.countryCode;
            $scope.submit($scope.dialogData);
        }
}]);

    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.TaxationEditTaxMethodController',
        ['$scope', function($scope) {



    }]);

angular.module('merchello').controller('Merchello.Directives.ShipCountryGatewaysProviderDirectiveController',
    ['$scope', 'notificationsService', 'dialogService',
        'shippingGatewayProviderResource', 'shippingGatewayProviderDisplayBuilder', 'shipMethodDisplayBuilder',
        'shippingGatewayMethodDisplayBuilder', 'gatewayResourceDisplayBuilder', 'dialogDataFactory',
        function($scope, notificationsService, dialogService,
                 shippingGatewayProviderResource, shippingGatewayProviderDisplayBuilder, shipMethodDisplayBuilder,
                 shippingGatewayMethodDisplayBuilder, gatewayResourceDisplayBuilder, dialogDataFactory) {

            $scope.providersLoaded = false;
            $scope.allProviders = [];
            $scope.assignedProviders = [];
            $scope.availableProviders = [];

            // exposed methods
            $scope.deleteCountry = deleteCountry;
            $scope.addShippingProviderDialogOpen = addShippingProviderDialogOpen;
            $scope.addAddShipMethodDialogOpen = addAddShipMethodDialogOpen;
            $scope.deleteShipMethodOpen = deleteShipMethodOpen;
            $scope.editShippingMethodDialogOpen = editShippingMethodDialogOpen;
            $scope.editShippingMethodRegionsOpen = editShippingMethodRegionsOpen;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Initializes the controller
             */
            function init() {
                loadCountryProviders();
            }

            /**
             * @ngdoc method
             * @name loadCountryProviders
             * @function
             *
             * @description
             * Load the shipping gateway providers from the shipping gateway service, then wrap the results
             * in Merchello models and add to the scope via the shippingGatewayProviders collection on the country model.  After
             * load is complete, it calls the loadProviderMethods to load in the methods.
             */
            function loadCountryProviders() {
                var promiseAllProviders = shippingGatewayProviderResource.getAllShipGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.allProviders = shippingGatewayProviderDisplayBuilder.transform(allProviders);

                    var promiseProviders = shippingGatewayProviderResource.getAllShipCountryProviders($scope.country);
                    promiseProviders.then(function (assigned) {
                        if (angular.isArray(assigned)) {
                            $scope.assignedProviders = shippingGatewayProviderDisplayBuilder.transform(assigned);

                            var available = _.filter($scope.allProviders, function(provider) {
                                var found = _.find($scope.assignedProviders, function(ap) {
                                    return ap.key === provider.key;
                                });
                                return found === undefined || found === null;
                            });
                            angular.forEach(available, function(pusher) {
                                $scope.availableProviders.push(pusher);
                            });
                            //console.info($scope.assignedProviders);
                            loadProviderMethods();
                        }
                    }, function (reason) {
                        notificationsService.error("Fixed Rate Shipping Countries Providers Load Failed", reason.message);
                    });
                }, function (reason) {
                    notificationsService.error("Available Ship Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadShipMethods
             * @function
             *
             * @description
             * Load the shipping methods from the shipping gateway service, then wrap the results
             * in Merchello models and add to the scope via the provider in the shipMethods collection.
             */
            function loadProviderMethods() {
                angular.forEach($scope.assignedProviders, function(shipProvider) {
                    var promiseShipMethods = shippingGatewayProviderResource.getShippingGatewayMethodsByCountry(shipProvider, $scope.country);
                    promiseShipMethods.then(function (shipMethods) {
                        var shippingGatewayMethods = shippingGatewayMethodDisplayBuilder.transform(shipMethods);
                        shipProvider.shippingGatewayMethods = _.sortBy(shippingGatewayMethods, function(gatewayMethod) {
                            return gatewayMethod.getName();
                        });
                    }, function (reason) {
                        notificationsService.error("Available Shipping Methods Load Failed", reason.message);
                    });
                });
                $scope.providersLoaded = true;
            }

            /**
             * @ngdoc method
             * @name addEditShippingProviderDialogOpen
             * @function
             *
             * @description
             * Opens the shipping provider dialog via the Umbraco dialogService.
             */
             function addShippingProviderDialogOpen() {
                var dialogData = dialogDataFactory.createAddShipCountryProviderDialogData();
                //dialogData.country = country;
                dialogData.availableProviders = $scope.availableProviders;
                dialogData.selectedProvider = dialogData.availableProviders[0];
                dialogData.selectedResource = dialogData.selectedProvider.availableResources[0];
                dialogData.shipMethodName = 'New ship method';
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.shipcountry.addprovider.html',
                    show: true,
                    callback: shippingProviderDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name addAddShipMethodDialogOpen
             * @function
             *
             * @description
             * Opens the shipping provider dialog via the Umbraco dialogService.
             */
            function addAddShipMethodDialogOpen(provider) {
                var dialogData = dialogDataFactory.createAddShipCountryProviderDialogData();
                dialogData.selectedProvider = provider;
                dialogData.selectedResource = dialogData.selectedProvider.availableResources[0];
                dialogData.shipMethodName = $scope.country.name + " " + dialogData.selectedResource.name;
                dialogData.country = $scope.country;
                dialogData.showProvidersDropDown = false;
                console.info(dialogData);
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.shipcountry.addprovider.html',
                    show: true,
                    callback: shippingProviderDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name editShippingMethodDialogOpen
             * @function
             *
             * @description
             * Opens an injected dialog for editing a the shipping provider's ship method
             */
            function editShippingMethodRegionsOpen(gatewayMethod) {
                var dialogData = dialogDataFactory.createEditShippingGatewayMethodDialogData();
                dialogData.shippingGatewayMethod = gatewayMethod;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.shipmethod.regions.html',
                    show: true,
                    callback: shippingMethodDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name shippingProviderDialogConfirm
             * @function
             *
             * @description
             * Handles the edit after receiving the dialogData from the dialog view/controller
             */
            function shippingProviderDialogConfirm(dialogData) {
                var newShippingMethod = shipMethodDisplayBuilder.createDefault();
                if(dialogData.shipMethodName ==='') {
                    newShippingMethod.name = $scope.country.name + " " + dialogData.selectedResource.name;
                } else {
                    newShippingMethod.name = dialogData.shipMethodName;
                }
                newShippingMethod.providerKey = dialogData.selectedProvider.key;
                newShippingMethod.serviceCode = dialogData.selectedResource.serviceCode;
                newShippingMethod.shipCountryKey = $scope.country.key;
                var promiseAddMethod;
                promiseAddMethod = shippingGatewayProviderResource.addShipMethod(newShippingMethod);
                promiseAddMethod.then(function () {
                    reload();
                }, function (reason) {
                    notificationsService.error("Shipping Provider / Initial Method Create Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deleteShipMethodOpen
             * @function
             *
             * @description
             * Opens the delete confirmation dialog for deleting ship methods
             */
            function deleteShipMethodOpen(shipMethod) {
                var dialogData = dialogDataFactory.createDeleteShipCountryDialogData();
                dialogData.shipMethod = shipMethod;
                dialogData.name = shipMethod.name;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: deleteShipMethodDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name deleteShipMethodOpen
             * @function
             *
             * @description
             * Processes the deleting of a ship method
             */
            function deleteShipMethodDialogConfirm(dialogData) {
                var shipMethod = dialogData.shipMethod;
                var promise = shippingGatewayProviderResource.deleteShipMethod(shipMethod);
                promise.then(function() {
                    reload();
                });
            }

            // injected dialogs

            /**
             * @ngdoc method
             * @name editShippingMethodDialogOpen
             * @function
             *
             * @description
             * Opens an injected dialog for editing a the shipping provider's ship method
             */
            function editShippingMethodDialogOpen(gatewayMethod) {
                var dialogData = dialogDataFactory.createEditShippingGatewayMethodDialogData();
                dialogData.shippingGatewayMethod = gatewayMethod;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.fixedrate.shipmethod.html',
                    show: true,
                    callback: shippingMethodDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name shippingMethodDialogConfirm
             * @function
             *
             * @description
             * Handles the edit after recieving the dialogData from the dialog view/controller
             */
            function shippingMethodDialogConfirm(dialogData) {
                var promiseShipMethodSave = shippingGatewayProviderResource.saveShipMethod(dialogData.shippingGatewayMethod.shipMethod);
                promiseShipMethodSave.then(function() {
                }, function (reason) {
                    notificationsService.error("Shipping Method Save Failed", reason.message);
                });
                reload();
            }

            /**
             * @ngdoc method
             * @name reload
             * @function
             *
             * @description
             * Handles the reload after receiving the modifying ship country information
             */
            function reload() {
                $scope.reload();
            }

            /**
             * @ngdoc method
             * @name delete
             * @function
             *
             * @description
             * Handles the delete of a ship country view/controller
             */
            function deleteCountry() {
                $scope.delete();
            }

            // initialize the directive
            init();

        }]);

/**
 * @ngdoc controller
 * @name Merchello.Backoffice.GatewayProvidersListController
 * @function
 *
 * @description
 * The controller for the gateway providers list view controller
 */
angular.module("umbraco").controller("Merchello.Backoffice.GatewayProvidersListController",
    ['$scope', 'assetsService', 'notificationsService', 'dialogService', 'merchelloTabsFactory',
        'gatewayProviderResource', 'gatewayProviderDisplayBuilder',
        function($scope, assetsService, notificationsService, dialogService, merchelloTabsFactory, gatewayProviderResource, gatewayProviderDisplayBuilder)
        {
            // load the css file
            assetsService.loadCss('/App_Plugins/Merchello/assets/css/merchello.css');

            $scope.loaded = true;
            $scope.notificationGatewayProviders = [];
            $scope.paymentGatewayProviders = [];
            $scope.shippingGatewayProviders = [];
            $scope.taxationGatewayProviders = [];
            $scope.tabs = [];

            // exposed methods
            $scope.activateProvider = activateProvider;
            $scope.deactivateProvider = deactivateProvider;
            $scope.editProviderConfigDialogOpen = editProviderConfigDialogOpen;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                loadAllNotificationGatwayProviders();
                loadAllPaymentGatewayProviders();
                loadAllShippingGatewayProviders();
                loadAllTaxationGatewayProviders();
                $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
                $scope.tabs.setActive('providers');
            }

            /**
             * @ngdoc method
             * @name loadAllNotificationGatwayProviders
             * @function
             *
             * @description
             * Loads in notification gateway providers from server into the scope.  Called in init().
             */
            function loadAllNotificationGatwayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedNotificationGatewayProviders();
                promiseAllProviders.then(function(allProviders) {
                    $scope.notificationGatewayProviders = gatewayProviderDisplayBuilder.transform(allProviders);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function(reason) {
                    notificationsService.error("Available Notification Gateway Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllPaymentGatewayProviders
             * @function
             *
             * @description
             * Loads in payment gateway providers from server into the scope.  Called in init().
             */
            function loadAllPaymentGatewayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedPaymentGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.paymentGatewayProviders = gatewayProviderDisplayBuilder.transform(allProviders);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Payment Gateway Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllShippingGatewayProviders
             * @function
             *
             * @description
             * Loads in shipping gateway providers from server into the scope.  Called in init().
             */
            function loadAllShippingGatewayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedShippingGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.shippingGatewayProviders = gatewayProviderDisplayBuilder.transform(allProviders);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Shipping Gateway Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllTaxationGatewayProviders
             * @function
             *
             * @description
             * Loads in taxation gateway providers from server into the scope.  Called in init().
             */
            function loadAllTaxationGatewayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedTaxationGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.taxationGatewayProviders = gatewayProviderDisplayBuilder.transform(allProviders);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Taxation Gateway Providers Load Failed", reason.message);
                });
            }

            /* -------------------------------------------------------------------
                Events
            ----------------------------------------------------------------------- */

            /**
             * @ngdoc method
             * @name activateProvider
             * @param {GatewayProvider} provider The GatewayProvider to activate
             * @function
             *
             * @description
             * Calls the merchelloGatewayProviderService to activate the provider.
             */
            function activateProvider(provider) {
                var promiseActivate = gatewayProviderResource.activateGatewayProvider(provider);
                promiseActivate.then(function () {
                    provider.activated = true;
                    init();
                    notificationsService.success("Payment Method Activated");
                }, function (reason) {
                    notificationsService.error("Payment Method Activate Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deactivateProvider
             * @param {GatewayProvider} provider The GatewayProvider to deactivate
             * @function
             *
             * @description
             * Calls the merchelloGatewayProviderService to deactivate the provider.
             */
            function deactivateProvider(provider) {
                var promiseDeactivate = gatewayProviderResource.deactivateGatewayProvider(provider);
                promiseDeactivate.then(function () {
                    provider.activated = false;
                    notificationsService.success("Payment Method Deactivated");
                }, function (reason) {
                    notificationsService.error("Payment Method Deactivate Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name editProviderConfigDialogOpen
             * @param {GatewayProvider} provider The GatewayProvider to configure
             * @function
             *
             * @description
             * Opens the dialog to allow user to add provider configurations
             */
            function editProviderConfigDialogOpen(provider) {
                var dialogProvider = provider;
                if (!provider) {
                    return;
                }
                var myDialogData = {
                    provider: dialogProvider
                };
                dialogService.open({
                    template: provider.dialogEditorView.editorView,
                    show: true,
                    callback: providerConfigDialogConfirm,
                    dialogData: myDialogData
                });
            }

            /**
             * @ngdoc method
             * @name providerConfigDialogConfirm
             * @param {dialogData} model returned from the dialog view
             * @function
             *
             * @description
             * Handles the data passed back from the provider editor dialog and saves it to the database
             */
            function providerConfigDialogConfirm(data) {
                var promise = gatewayProviderResource.saveGatewayProvider(data.provider);
                promise.then(function (provider) {
                        notificationsService.success("Gateway Provider Saved", "");
                    },
                    function (reason) {
                        notificationsService.error("Gateway Provider Save Failed", reason.message);
                    }
                );
            }

            // Initialize the controller

            init();

        }]);








    angular.module('merchello').controller('Merchello.Backoffice.NotificationMessageEditorController',
    ['$scope', '$routeParams', 'dialogService', 'notificationsService', 'merchelloTabsFactory',
    'notificationGatewayProviderResource', 'notificationMessageDisplayBuilder',
    function($scope, $routeParams, dialogService, notificationsService, merchelloTabsFactory,
    notificationGatewayProviderResource, notificationMessageDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.tabs = [];
        $scope.rteProperties = {
            label: 'bodyText',
            view: 'rte',
            config: {
                editor: {
                    toolbar: ["code", "undo", "redo", "cut", "styleselect", "bold", "italic", "alignleft", "aligncenter", "alignright", "bullist", "numlist", "link", "umbmediapicker", "umbmacro", "table", "umbembeddialog"],
                    stylesheets: [],
                    dimensions: { height: 350 }
                }
            },
            value: ""
        };

        // exposed methods
        $scope.save = save;

        function init() {
            var key = $routeParams.id;
            loadNotificationMessage(key);
            loadAllNotificationMonitors();
            $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
            $scope.tabs.insertTab('messageEditor', 'Message', '#/merchello/merchello/notification.messageeditor/' + key, 2);
            $scope.tabs.setActive('messageEditor');
        }

        /**
         * @ngdoc method
         * @name loadNotificationMessage
         * @function
         *
         * @description
         * Loads all of the Notification Message
         */
        function loadNotificationMessage(key) {
            var promise = notificationGatewayProviderResource.getNotificationMessagesByKey(key);
            promise.then(function (notification) {
                $scope.notificationMessage = notificationMessageDisplayBuilder.transform(notification);
                $scope.rteProperties.value = notification.bodyText;
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            });
            return promise;
        }

        /**
         * @ngdoc method
         * @name loadAllNotificationMonitors
         * @function
         *
         * @description
         * Loads all of the Notification Monitors
         */
        function loadAllNotificationMonitors() {
            var promise = notificationGatewayProviderResource.getAllNotificationMonitors();
            promise.then(function (monitors) {
                $scope.notificationTriggers = notificationMessageDisplayBuilder.transform(monitors);
            });
        }

        /**
         * @ngdoc method
         * @name save
         * @function
         *
         * @description
         * Saves the notification message
         */
        function save() {
            $scope.preValuesLoaded = false;
            $scope.notificationMessage.bodyText = $scope.rteProperties.value;
            var promiseSave = notificationGatewayProviderResource.updateNotificationMessage($scope.notificationMessage);
            promiseSave.then(function () {
                notificationsService.success("Payment Method Saved");
                init();
            }, function (reason) {
                notificationsService.error("Payment Method Save Failed", reason.message);
            });
        }

        // Initialize the controller
        init();

    }]);

    angular.module('merchello').controller('Merchello.Backoffice.NotificationProvidersController',
        ['$scope', 'notificationsService', 'dialogService', 'merchelloTabsFactory', 'dialogDataFactory', 'gatewayResourceDisplayBuilder',
        'notificationGatewayProviderResource', 'notificationGatewayProviderDisplayBuilder', 'notificationMethodDisplayBuilder',
        'notificationMonitorDisplayBuilder', 'notificationMessageDisplayBuilder',
        function($scope, notificationsService, dialogService, merchelloTabsFactory, dialogDataFactory, gatewayResourceDisplayBuilder,
        notificationGatewayProviderResource, notificationGatewayProviderDisplayBuilder, notificationMethodDisplayBuilder,
        notificationMonitorDisplayBuilder, notificationMessageDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.notificationMonitors = [];
            $scope.tabs = [];

            $scope.notificationGatewayProviders = [];

            // exposed methods
            $scope.addNotificationMethod = addNotificationMethod;
            $scope.deleteNotificationMethod = deleteNotificationMethod;
            $scope.addNotificationMessage = addNotificationMessage;
            $scope.deleteNotificationMessage = deleteNotificationMessage;

            function init() {
                loadAllNotificationGatewayProviders();
                loadAllNotificationMonitors();
                $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
                $scope.tabs.setActive('notification');
            }

            /**
             * @ngdoc method
             * @name getProviderByKey
             * @function
             *
             * @description
             * Helper method to get a provider from the notificationGatewayProviders array using the provider key passed in.
             */
            function getProviderByKey(providerkey) {
                return _.find($scope.notificationGatewayProviders, function (gatewayprovider) { return gatewayprovider.key == providerkey; });
            }

            /**
             * @ngdoc method
             * @name loadAllNotificationGatewayProviders
             * @function
             *
             * @description
             * Load the notification gateway providers from the notification gateway service, then wrap the results
             * in Merchello models and add to the scope via the notificationGatewayProviders collection.
             */
            function loadAllNotificationGatewayProviders() {
                var promiseAllProviders = notificationGatewayProviderResource.getAllGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.notificationGatewayProviders = notificationGatewayProviderDisplayBuilder.transform(allProviders);
                    angular.forEach($scope.notificationGatewayProviders, function(provider) {
                        loadNotificationGatewayResources(provider.key);
                        loadNotificationMethods(provider.key);
                    });

                }, function (reason) {
                    notificationsService.error("Available Notification Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadNotificationGatewayResources
             * @function
             *
             * @description
             * Load the notification gateway resources from the notification gateway service, then wrap the results
             * in Merchello models and add to the provider in the resources collection.  This will only
             * return resources that haven't already been added via other methods on the provider.
             */
            function loadNotificationGatewayResources(providerKey) {
                var provider = getProviderByKey(providerKey);
                var promiseAllResources = notificationGatewayProviderResource.getGatewayResources(provider.key);
                promiseAllResources.then(function (allResources) {
                    provider.gatewayResources = gatewayResourceDisplayBuilder.transform(allResources);
                    if (provider.gatewayResources.length > 0) {
                        provider.selectedGatewayResource = provider.gatewayResources[0];
                    }
                }, function (reason) {
                    notificationsService.error("Available Notification Provider Resources Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllNotificationTriggers
             * @function
             *
             * @description
             * Loads the triggers for the notification messages.
             */
            function loadAllNotificationMonitors() {
                var promise = notificationGatewayProviderResource.getAllNotificationMonitors();
                promise.then(function (notificationMonitors) {
                    $scope.notificationMonitors = notificationMonitorDisplayBuilder.transform(notificationMonitors);
                });
            }

            /**
             * @ngdoc method
             * @name loadNotificationMethods
             * @function
             *
             * @description
             * Load the notification gateway methods from the notification gateway service, then wrap the results
             * in Merchello models and add to the provider in the methods collection.
             */
            function loadNotificationMethods(providerKey) {

                var provider = getProviderByKey(providerKey);
                var promiseAllResources = notificationGatewayProviderResource.getNotificationProviderNotificationMethods(providerKey);
                promiseAllResources.then(function (allMethods) {
                    provider.notificationMethods = notificationMethodDisplayBuilder.transform(allMethods);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Notification Methods Load Failed", reason.message);
                });
            }

            //--------------------------------------------------------------------------------------
            // Dialog methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name addNotificationsDialogConfirm
             * @function
             *
             * @description
             * Handles the save after recieving the notification to add from the dialog view/controller
             */
            function addNotificationsDialogConfirm(dialogData) {
                $scope.preValuesLoaded = false;
                var promiseNotificationMethod = notificationGatewayProviderResource.saveNotificationMethod(dialogData.notificationMethod);
                promiseNotificationMethod.then(function(notificationFromServer) {
                    notificationsService.success("Notification Method Created!", "");
                    init();
                }, function(reason) {
                    notificationsService.error("Notification Method Create Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name addNotificationMethod
             * @function
             *
             * @description
             * Opens the add notification method dialog via the Umbraco dialogService.
             */
            function addNotificationMethod(provider, resource) {
                var dialogData = dialogDataFactory.createAddEditNotificationMethodDialogData();
                var method = notificationMethodDisplayBuilder.createDefault();
                method.name = resource.name;
                method.serviceCode = resource.serviceCode;
                method.providerKey = provider.key;
                dialogData.notificationMethod = method;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/notification.notificationmethod.addedit.html',
                    show: true,
                    callback: addNotificationsDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name notificationsMethodDeleteDialogConfirm
             * @function
             *
             * @description
             * Handles the delete after recieving the deleted command from the dialog view/controller
             */
            function notificationsMethodDeleteDialogConfirm(dialogData) {
                $scope.preValuesLoaded = false;
                var promiseNotificationMethod = notificationGatewayProviderResource.deleteNotificationMethod(dialogData.notificationMethod.key);
                promiseNotificationMethod.then(function () {
                    notificationsService.success("Notification Deleted");
                    init();
                }, function (reason) {
                    notificationsService.error("Notification Method Deletion Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deleteNotificationMethod
             * @function
             *
             * @description
             * Opens the delete dialog via the Umbraco dialogService
             */
            function deleteNotificationMethod(method) {
                var dialogData = dialogDataFactory.createDeleteNotificationMethodDialogData();
                dialogData.notificationMethod = method;
                dialogData.name = method.name;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: notificationsMethodDeleteDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name notificationsMessageDeleteDialogConfirm
             * @function
             *
             * @description
             * Handles the delete after recieving the deleted command from the dialog view/controller
             */
            function notificationsMessageDeleteDialogConfirm(dialogData) {
                console.info(dialogData);
                var promiseNotificationMethod = notificationGatewayProviderResource.deleteNotificationMessage(dialogData.notificationMessage.key);
                promiseNotificationMethod.then(function () {
                    notificationsService.success("Notification Deleted");
                    init();
                }, function (reason) {
                    notificationsService.error("Notification Method Deletion Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deleteNotificationMessage
             * @function
             *
             * @description
             * Opens the delete dialog via the Umbraco dialogService
             */
            function deleteNotificationMessage(message) {
                var dialogData = dialogDataFactory.createDeleteNotificationMessageDialogData();
                dialogData.notificationMessage = message;
                dialogData.name = message.name;

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: notificationsMessageDeleteDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name notificationsMessageAddDialogConfirm
             * @function
             *
             * @description
             * Handles the save after recieving the save command from the dialog view/controller
             */
            function notificationsMessageAddDialogConfirm(dialogData) {
                console.info(dialogData);
                var promiseNotificationMethod = notificationGatewayProviderResource.saveNotificationMessage(dialogData.notificationMessage);
                promiseNotificationMethod.then(function (keyFromServer) {
                    notificationsService.success("Notification Saved", "");
                    init();
                }, function (reason) {
                    notificationsService.error("Notification Message Saved Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name addNotificationMessage
             * @function
             *
             * @description
             * Opens the add notification dialog via the Umbraco dialogService
             */
            function addNotificationMessage(method) {
                var dialogData = dialogDataFactory.createAddEditNotificationMessageDialogData();
                dialogData.notificationMessage = notificationMessageDisplayBuilder.createDefault();
                console.info(method);
                dialogData.notificationMessage.methodKey = method.key;
                dialogData.notificationMessage.name = method.name;
                dialogData.notificationMonitors = $scope.notificationMonitors;
                dialogData.selectedMonitor = $scope.notificationMonitors[0];
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/notification.notificationmessage.add.html',
                    show: true,
                    callback: notificationsMessageAddDialogConfirm,
                    dialogData: dialogData
                });
            }

            // Initialize the controller
            init();
    }]);

    angular.module('merchello').controller('Merchello.Backoffice.PaymentProvidersController',
        ['$scope', 'notificationsService', 'dialogService', 'paymentGatewayProviderResource', 'dialogDataFactory', 'merchelloTabsFactory',
           'gatewayResourceDisplayBuilder', 'paymentGatewayProviderDisplayBuilder', 'paymentMethodDisplayBuilder',
        function($scope, notificationsService, dialogService, paymentGatewayProviderResource, dialogDataFactory, merchelloTabsFactory,
                 gatewayResourceDisplayBuilder, paymentGatewayProviderDisplayBuilder, paymentMethodDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.paymentGatewayProviders = [];
            $scope.tabs = [];

            // exposed methods
            $scope.addEditPaymentMethod = addEditPaymentMethod;
            $scope.deletePaymentMethod = deletePaymentMethod;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                loadAllPaymentGatewayProviders();
                $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
                $scope.tabs.setActive('payment');
            }

            /**
             * @ngdoc method
             * @name getProviderByKey
             * @function
             *
             * @description
             * Helper method to get a provider from the paymentGatewayProviders array using the provider key passed in.
             */
            function getProviderByKey(providerkey) {
                return _.find($scope.paymentGatewayProviders, function (gatewayprovider) { return gatewayprovider.key == providerkey; });
            }

            //--------------------------------------------------------------------------------------
            // Initialization methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name loadAllPaymentGatewayProviders
             * @function
             *
             * @description
             * Load the payment gateway providers from the payment gateway service, then wrap the results
             * in Merchello models and add to the scope via the paymentGatewayProviders collection.
             */
            function loadAllPaymentGatewayProviders() {

                var promiseAllProviders = paymentGatewayProviderResource.getAllGatewayProviders();
                promiseAllProviders.then(function(allProviders) {

                    $scope.paymentGatewayProviders = paymentGatewayProviderDisplayBuilder.transform(allProviders);

                    angular.forEach($scope.paymentGatewayProviders, function(provider) {
                        loadPaymentGatewayResources(provider.key);
                        loadPaymentMethods(provider.key);
                    });

                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

                }, function(reason) {
                    notificationsService.error("Available Payment Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadPaymentGatewayResources
             * @function
             *
             * @description
             * Load the payment gateway resources from the payment gateway service, then wrap the results
             * in Merchello models and add to the provider in the resources collection.  This will only
             * return resources that haven't already been added via other methods on the provider.
             */
            function loadPaymentGatewayResources(providerKey) {

                var provider = getProviderByKey(providerKey);
                provider.showSelectResource = false;
                var promiseAllResources = paymentGatewayProviderResource.getGatewayResources(provider.key);
                promiseAllResources.then(function (allResources) {
                    console.info(allResources);
                    provider.gatewayResources = gatewayResourceDisplayBuilder.transform(allResources);
                    if (provider.gatewayResources.length > 0) {
                        provider.selectedGatewayResource = provider.gatewayResources[0];
                    }

                }, function (reason) {
                    notificationsService.error("Available Payment Provider Resources Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadPaymentMethods
             * @function
             *
             * @description
             * Load the payment gateway methods from the payment gateway service, then wrap the results
             * in Merchello models and add to the provider in the methods collection.
             */
            function loadPaymentMethods(providerKey) {

                var provider = getProviderByKey(providerKey);
                var promiseAllResources = paymentGatewayProviderResource.getPaymentProviderPaymentMethods(providerKey);
                promiseAllResources.then(function (allMethods) {
                    provider.paymentMethods = paymentMethodDisplayBuilder.transform(allMethods);
                }, function (reason) {
                    notificationsService.error("Payment Methods Load Failed", reason.message);
                });
            }



            //--------------------------------------------------------------------------------------
            // Event Handlers
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name removeMethod
             * @function
             *
             * @description
             * Calls the payment gateway service to delete the method passed in via the method parameter
             */
            function removeMethod(method) {
                $scope.preValuesLoaded = false;
                var promiseDelete = paymentGatewayProviderResource.deletePaymentMethod(method.key);
                promiseDelete.then(function () {
                    loadAllPaymentGatewayProviders();
                    notificationsService.success("Payment Method Deleted");
                }, function (reason) {
                    notificationsService.error("Payment Method Delete Failed", reason.message);
                });
            }


            //--------------------------------------------------------------------------------------
            // Dialogs
            //--------------------------------------------------------------------------------------

            /// Method add/edit Dialog

            /**
             * @ngdoc method
             * @name paymentMethodDialogConfirm
             * @function
             *
             * @description
             * Handles the save after recieving the edited method from the dialog view/controller
             */
            function paymentMethodDialogConfirm(method) {
                $scope.preValuesLoaded = false;
                var promiseSave;
                if (method.key.length > 0) {
                    // Save existing method
                    promiseSave = paymentGatewayProviderResource.savePaymentMethod(method);
                } else {
                    // Create new method
                    promiseSave = paymentGatewayProviderResource.addPaymentMethod(method);
                }

                var provider = getProviderByKey(method.providerKey);
                provider.showSelectResource = false;

                promiseSave.then(function () {
                    loadPaymentMethods(method.providerKey);
                    loadPaymentGatewayResources(method.providerKey);
                    $scope.preValuesLoaded = true;
                    notificationsService.success("Payment Method Saved");
                }, function (reason) {
                    notificationsService.error("Payment Method Save Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name addEditPaymentMethod
             * @function
             *
             * @description
             * Opens the payment method dialog via the Umbraco dialogService.  This will default to the dialog view in Merchello
             * unless specified on the custom method in the payment provider.  Also, if it is an add (not edit) then it will
             * initialize a new method and pass that to the dialog service.
             */
            function addEditPaymentMethod(provider, method) {
                if (method == undefined) {
                    method = paymentMethodDisplayBuilder.createDefault();
                    method.providerKey = provider.key; //Todo: When able to add external providers, make this select the correct provider
                    method.paymentCode = provider.selectedGatewayResource.serviceCode;
                    method.name = provider.selectedGatewayResource.name;
                    method.dialogEditorView.editorView = '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.paymentmethod.addedit.html';
                }

                dialogService.open({
                    template: method.dialogEditorView.editorView,
                    show: true,
                    callback: paymentMethodDialogConfirm,
                    dialogData: method
                });
            }

            /// Method delete Dialog

            /**
             * @ngdoc method
             * @name paymentMethodDeleteDialogConfirm
             * @function
             *
             * @description
             * Handles the save after recieving the deleted method from the dialog view/controller
             */
            function paymentMethodDeleteDialogConfirm(dialogData) {
                removeMethod(dialogData.paymentMethod);
            }

            /**
             * @ngdoc method
             * @name deletePaymentMethod
             * @function
             *
             * @description
             * Opens the delete dialog via the Umbraco dialogService
             */
            function deletePaymentMethod(method) {
                var dialogData = dialogDataFactory.createDeletePaymentMethodDialogData();
                dialogData.paymentMethod = method;
                dialogData.name = method.name;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: paymentMethodDeleteDialogConfirm,
                    dialogData: dialogData
                });
            }

            // Initializes the controller
            init();

    }]);

/**
 * @ngdoc service
 * @name Merchello.Backoffice.ShippingProvidersController
 *
 * @description
 * The controller for the shipment provider view
 */
angular.module('merchello').controller('Merchello.Backoffice.ShippingProvidersController',
    ['$scope', 'notificationsService', 'dialogService', 'merchelloTabsFactory',
    'settingsResource', 'warehouseResource', 'shippingGatewayProviderResource', 'dialogDataFactory',
    'settingDisplayBuilder', 'warehouseDisplayBuilder', 'warehouseCatalogDisplayBuilder', 'countryDisplayBuilder',
        'shippingGatewayProviderDisplayBuilder', 'shipCountryDisplayBuilder',
    function($scope, notificationsService, dialogService, merchelloTabsFactory,
             settingsResource, warehouseResource, shippingGatewayProviderResource, dialogDataFactory,
             settingDisplayBuilder, warehouseDisplayBuilder, warehouseCatalogDisplayBuilder, countryDisplayBuilder,
             shippingGatewayProviderDisplayBuilder, shipCountryDisplayBuilder) {

        $scope.loaded = true;
        $scope.tabs = [];
        $scope.countries = [];
        $scope.warehouses = [];
        $scope.primaryWarehouse = {};
        $scope.selectedCatalog = {};
        $scope.providers = [];
        $scope.primaryWarehouseAddress = {};
        $scope.visible = {
            catalogPanel: true,
            shippingMethodPanel: true,
            warehouseInfoPanel: false,
            warehouseListPanel: true
        };

        // exposed methods
        $scope.loadCountries = loadCountries;
        $scope.addCountry = addCountry;
        $scope.deleteCountryDialog = deleteCountryDialog;
        $scope.addEditWarehouseDialogOpen = addEditWarehouseDialogOpen;
        $scope.addEditWarehouseCatalogDialogOpen = addEditWarehouseCatalogDialogOpen;
        $scope.changeSelectedCatalogOpen = changeSelectedCatalogOpen;
        $scope.deleteWarehouseCatalogOpen = deleteWarehouseCatalogOpen;

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
            loadWarehouses();
            $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
            $scope.tabs.setActive('shipping');
        }

        /**
         * @ngdoc method
         * @name loadWarehouses
         * @function
         *
         * @description
         * Load the warehouses from the warehouse service, then wrap the results
         * in Merchello models and add to the scope via the warehouses collection.
         * Once loaded, it calls the loadCountries method.
         */
        function loadWarehouses() {
            var promiseWarehouses = warehouseResource.getDefaultWarehouse(); // Only a default warehouse in v1
            promiseWarehouses.then(function (warehouses) {
                $scope.warehouses.push(warehouseDisplayBuilder.transform(warehouses));
                changePrimaryWarehouse();
                loadCountries();
                //loadAllShipProviders();
            }, function (reason) {
                notificationsService.error("Warehouses Load Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name loadCountries
         * @function
         *
         * @description
         * Load the countries from the shipping service, then wrap the results
         * in Merchello models and add to the scope via the countries collection.
         * Once loaded, it calls the loadCountryProviders method for each
         * country.
         */
        function loadCountries() {
            if ($scope.primaryWarehouse.warehouseCatalogs.length > 0) {
                var catalogKey = $scope.selectedCatalog.key;
                var promiseShipCountries = shippingGatewayProviderResource.getWarehouseCatalogShippingCountries(catalogKey);
                promiseShipCountries.then(function (shipCountries) {
                    var transformed = shipCountryDisplayBuilder.transform(shipCountries);
                    $scope.countries = _.sortBy(
                        transformed, function(country) {
                        return country.name;
                    });
                    var elseCountry = _.find($scope.countries, function(country) {
                        return country.countryCode === 'ELSE';
                    });
                    if(elseCountry !== null && elseCountry !== undefined) {
                        $scope.countries = _.reject($scope.countries, function(country) {
                            return country.countryCode === 'ELSE';
                        });
                        $scope.countries.push(elseCountry);
                    }
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Shipping Countries Load Failed", reason.message);
                });
            }
        }

        //--------------------------------------------------------------------------------------
        // Helper methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name changePrimaryWarehouse
         * @function
         *
         * @description
         * Helper method to set the primary warehouse on the scope and to make sure the isDefault flag on
         * all warehouses is set properly.  If a warehouse is passed in, then it will find that warehouse
         * and set it as the primary and set isDefault to true.  All other warehouses will have their
         * isDefault flag reset to false.  If no warehouse is passed in (usually on loading data) then the
         * warehouse that has the isDefault == true will be set as the primary warehouse on the scope.
         */
        function changePrimaryWarehouse(warehouse) {
            $scope.primaryWarehouse = _.find($scope.warehouses, function(warehouse) {
                   return warehouse.isDefault;
            });
            $scope.primaryWarehouseAddress = $scope.primaryWarehouse.getAddress();
            //changeSelectedCatalog();
            $scope.selectedCatalog = _.find($scope.primaryWarehouse.warehouseCatalogs, function(catalog) {
                return catalog.isDefault === true;
            });
        }

        /**
         * @ngdoc method
         * @name changeSelectedCatalog
         * @function
         *
         * @description
         *
         */
        function changeSelectedCatalogOpen() {
            var dialogData = dialogDataFactory.createChangeWarehouseCatalogDialogData();
            dialogData.warehouse = $scope.primaryWarehouse;
            dialogData.selectedWarehouseCatalog = $scope.selectedCatalog;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.warehousecatalog.select.html',
                show: true,
                callback: selectCatalogDialogConfirm,
                dialogData: dialogData
            });
        }


        //--------------------------------------------------------------------------------------
        // Dialog methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name addCountry
         * @function
         *
         * @description
         * Opens the add country dialog via the Umbraco dialogService.
         */
        function addCountry() {
            var promiseAllCountries = settingsResource.getAllCountries();
            promiseAllCountries.then(function (allCountries) {
                var countries = countryDisplayBuilder.transform(allCountries);

                // Add Everywhere Else as an option
                var elseCountry = countryDisplayBuilder.createDefault();
                elseCountry.key = '7501029f-5ab3-4733-935d-1dd37b37bf8';
                elseCountry.countryCode = 'ELSE';
                // TODO this should be localized
                elseCountry.name = 'Everywhere Else';
                countries.push(elseCountry);

                // we only want available countries that are not already in use
                var availableCountries = _.sortBy(
                    _.filter(countries, function(country) {
                        var found = _.find($scope.countries, function(assigned) {
                            return country.countryCode === assigned.countryCode;
                        });
                        return found === undefined || found === null;
                    }), function(country) {
                        return country.name;
                    });

                // construct the dialog data for the add ship country dialog
                var dialogData = dialogDataFactory.createAddShipCountryDialogData();
                dialogData.availableCountries = availableCountries;
                dialogData.selectedCountry = availableCountries[0];

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.addcountry.html',
                    show: true,
                    callback: addCountryDialogConfirm,
                    dialogData: dialogData
                });

            }, function (reason) {
                notificationsService.error("Available Countries Load Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name addCountryDialogConfirm
         * @function
         *
         * @description
         * Handles the save after recieving the country to add from the dialog view/controller
         */
        function addCountryDialogConfirm(dialogData) {
            $scope.preValuesLoaded = false;
            var catalogKey = $scope.selectedCatalog.key;
            var promiseShipCountries = shippingGatewayProviderResource.newWarehouseCatalogShippingCountry(catalogKey, dialogData.selectedCountry.countryCode);
            promiseShipCountries.then(function () {
                loadCountries()
            }, function (reason) {
                notificationsService.error("Shipping Countries Create Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name addEditWarehouseDialogOpen
         * @function
         *
         * @description
         * Handles opening a dialog for adding or editing a warehouse
         */
        function addEditWarehouseDialogOpen(warehouse) {
            // todo this will need to be refactored once we expose multiple warehouse
            var dialogData = dialogDataFactory.createAddEditWarehouseDialogData();
            dialogData.warehouse = warehouse;
            var promise = settingsResource.getAllCountries();
            promise.then(function(countries) {
                dialogData.availableCountries  = countryDisplayBuilder.transform(countries);
                dialogData.selectedCountry = _.find(dialogData.availableCountries, function(country) {
                    return country.countryCode === warehouse.countryCode;
                });
                if (dialogData.selectedCountry === undefined) {
                    dialogData.selectedCountry = dialogData.availableCountries[0];
                }
                if(dialogData.selectedCountry.provinces.length > 0) {
                    dialogData.selectedProvince = _.find(dialogData.selectedCountry.provinces, function(province) {
                        return province.code === warehouse.region;
                    });
                }
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.warehouse.addedit.html',
                    show: true,
                    callback: addEditWarehouseDialogConfirm,
                    dialogData: dialogData
                });
            });
        }

        /**
         * @ngdoc method
         * @name addEditWarehouseDialogConfirm
         * @function
         *
         * @description
         * Handles the saving of warehouse information
         */
        function addEditWarehouseDialogConfirm(dialogData) {
            $scope.preValuesLoaded = false;
            var promise = warehouseResource.save(dialogData.warehouse);
            promise.then(function() {
              loadWarehouses();
            });
        }

        /**
         * @ngdoc method
         * @name addEditWarehouseCatalogDialogOpen
         * @function
         *
         * @description
         * Opens the warehouse catalog dialog via the Umbraco dialogService.
         */
        function addEditWarehouseCatalogDialogOpen(catalog) {
            var dialogData = dialogDataFactory.createAddEditWarehouseCatalogDialogData();
            dialogData.warehouse = $scope.primaryWarehouse;
            if(catalog === undefined || catalog === null) {
                dialogData.warehouseCatalog = warehouseCatalogDisplayBuilder.createDefault();
                dialogData.warehouseCatalog.warehouseKey = dialogData.warehouse.key;
            } else {
                dialogData.warehouseCatalog = catalog;
            }

            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.warehousecatalog.addedit.html',
                show: true,
                callback: warehouseCatalogDialogConfirm,
                dialogData: dialogData
            });

        }

        /**
         * @ngdoc method
         * @name deleteCatalog
         * @function
         *
         * @description
         * Opens the delete catalog dialog via the Umbraco dialogService.
         */
        function deleteWarehouseCatalogOpen() {
            var dialogData = dialogDataFactory.createDeleteWarehouseCatalogDialogData();
            dialogData.name = $scope.selectedCatalog.name;
            dialogData.warehouseCatalog = $scope.selectedCatalog;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                show: true,
                callback: deleteWarehouseCatalogConfirm,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name deleteCatalogConfirm
         * @function
         *
         * @description
         * Handles the delete after recieving the catalog to delete from the dialog view/controller
         */
        function deleteWarehouseCatalogConfirm(dialogData) {
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            if(dialogData.warehouseCatalog.isDefault === false)
            {
                var promiseDeleteCatalog = warehouseResource.deleteWarehouseCatalog(dialogData.warehouseCatalog.key);
                promiseDeleteCatalog.then(function (responseCatalog) {
                    $scope.warehouses = [];
                    init();
                }, function (reason) {
                    notificationsService.error('Catalog Delete Failed', reason.message);
                });
            } else {
                notificationsService.warning('Cannot delete the default catalog.')
            }
        }

        /**
         * @ngdoc method
         * @name addCountry
         * @function
         *
         * @description
         * Opens the add country dialog via the Umbraco dialogService.
         */
        function deleteCountryDialog(country) {
            var dialogData = dialogDataFactory.createDeleteShipCountryDialogData();
            dialogData.country = country;
            dialogData.name = country.name;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                show: true,
                callback: deleteCountryDialogConfirm,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name deleteCountry
         * @function
         *
         * @description
         * Calls the shipping service to delete the country passed in via the country parameter.
         * When complete, the countries are reloaded from the api to get the latest from the database.
         *
         */
        function deleteCountryDialogConfirm(dialogData) {
            $scope.preValuesLoaded = true;
            var promiseDelete = shippingGatewayProviderResource.deleteShipCountry(dialogData.country.key);
            promiseDelete.then(function () {
                notificationsService.success("Shipping Country Deleted");
                loadCountries();
            }, function (reason) {
                notificationsService.error("Shipping Country Delete Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name selectCatalogDialogConfirm
         * @function
         *
         * @description
         * Handles the catalog selection after recieving the dialogData from the dialog view/controller
         */
        function selectCatalogDialogConfirm(dialogData) {
            $scope.preValuesLoaded = false;
            $scope.selectedCatalog = _.find($scope.primaryWarehouse.warehouseCatalogs, function(catalog) {
                return catalog.key === dialogData.selectedWarehouseCatalog.key;
            });

            // Load the countries associated with this catalog.
            loadCountries();
        }

        /**
         * @ngdoc method
         * @name warehouseCatalogDialogConfirm
         * @function
         *
         * @description
         * Handles the add/edit after recieving the dialogData from the dialog view/controller.
         * If the selectedCatalog is set to be default, ensure that original default is no longer default.
         */
        function warehouseCatalogDialogConfirm(dialogData) {
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            var promiseUpdateCatalog;
            if (dialogData.warehouseCatalog.key === "") {
                promiseUpdateCatalog = warehouseResource.addWarehouseCatalog(dialogData.warehouseCatalog);
            } else {
                promiseUpdateCatalog = warehouseResource.putWarehouseCatalog(dialogData.warehouseCatalog);
            }
            promiseUpdateCatalog.then(function(response) {
                $scope.warehouses = [];
                init();
            }, function(reason) {
                notificationsService.error('Catalog Update Failed', reason.message);
            });
        }

        // initialize the controller
        init();


    }]);

angular.module('merchello').controller('Merchello.Backoffice.TaxationProvidersController',
    ['$scope', '$timeout', 'settingsResource', 'notificationsService', 'dialogService', 'taxationGatewayProviderResource', 'dialogDataFactory', 'merchelloTabsFactory',
        'settingDisplayBuilder', 'countryDisplayBuilder', 'taxCountryDisplayBuilder',
        'gatewayResourceDisplayBuilder', 'gatewayProviderDisplayBuilder', 'taxMethodDisplayBuilder',
    function($scope, $timeout, settingsResource, notificationsService, dialogService, taxationGatewayProviderResource, dialogDataFactory, merchelloTabsFactory,
             settingDisplayBuilder, countryDisplayBuilder, taxCountryDisplayBuilder,
             gatewayResourceDisplayBuilder, gatewayProviderDisplayBuilder, taxMethodDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.tabs = [];
        $scope.allCountries = [];
        $scope.availableCountries = [];
        $scope.taxationGatewayProviders = [];

        // exposed methods
        $scope.save = save;
        $scope.editTaxMethodProvinces = editTaxMethodProvinces;

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
            $scope.availableCountries = [];
            $scope.taxationGatewayProviders = [];
            loadAllAvailableCountries();
            loadAllTaxationGatewayProviders();
            $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
            $scope.tabs.setActive('taxation');
            _.sortBy($scope.availableCountries, function(country) {
                return country.name;
            });
        }

        /**
         * @ngdoc method
         * @name loadAllAvailableCountries
         * @function
         *
         * @description
         * Method called on initial page load.  Loads in data from server and sets up scope.
         */
        function loadAllAvailableCountries() {
            var promiseAllCountries = settingsResource.getAllCountries();
            promiseAllCountries.then(function (allCountries) {
                $scope.allCountries = countryDisplayBuilder.transform(allCountries);
            }, function (reason) {
                notificationsService.error("Available Countries Load Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name loadAllTaxationGatewayProviders
         * @function
         *
         * @description
         * Loads all taxation gateway providers.
         */
        function loadAllTaxationGatewayProviders() {

            var promiseAllProviders = taxationGatewayProviderResource.getAllGatewayProviders();
            promiseAllProviders.then(function (allProviders) {
                $scope.taxationGatewayProviders = gatewayProviderDisplayBuilder.transform(allProviders);

                var noTaxProvider = gatewayProviderDisplayBuilder.createDefault();
                noTaxProvider.key = -1;
                noTaxProvider.name = 'Not Taxed';

                if($scope.taxationGatewayProviders.length > 0) {
                    for(var i = 0; i < $scope.taxationGatewayProviders.length; i++) {
                        loadAvailableCountriesWithoutMethod($scope.taxationGatewayProviders[i], noTaxProvider);
                        loadTaxMethods($scope.taxationGatewayProviders[i]);
                    }
                }

                $scope.taxationGatewayProviders.unshift(noTaxProvider);

            }, function (reason) {
                notificationsService.error("Available Taxation Providers Load Failed", reason.message);
            });
        }

        function loadAvailableCountriesWithoutMethod(taxationGatewayProvider, noTaxProvider) {

            var promiseGwResources = taxationGatewayProviderResource.getGatewayResources(taxationGatewayProvider.key);
            promiseGwResources.then(function (resources) {

                var newAvailableCountries = _.map(resources, function (resourceFromServer) {
                    var taxCountry = taxCountryDisplayBuilder.transform(resourceFromServer);
                    taxCountry.country = _.find($scope.allCountries, function (c) { return c.countryCode == taxCountry.gatewayResource.serviceCode; });
                    if (taxCountry.country) {
                        taxCountry.setCountryName(taxCountry.country.name);
                    } else {
                        if (taxCountry.gatewayResource.serviceCodeStartsWith('ELSE')) {
                            taxCountry.country = countryDisplayBuilder.createDefault();
                            taxCountry.setCountryName('Everywhere Else');
                            taxCountry.country.countryCode = 'ELSE';
                            taxCountry.sortOrder = 9999;
                        } else {
                            taxCountry.setCountryName(taxCountry.name);
                        }
                    }
                    return taxCountry;
                });
                angular.forEach(newAvailableCountries, function(country) {
                    country.taxMethod = taxMethodDisplayBuilder.createDefault();
                    country.provider = noTaxProvider;
                    country.taxMethod.providerKey = '-1';
                    $scope.availableCountries.push(country);
                });

                console.info($scope.availableCountries);

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.error("Available Gateway Resources Load Failed", reason.message);
            });
        }

        function loadTaxMethods(taxationGatewayProvider) {

            var promiseGwResources = taxationGatewayProviderResource.getTaxationProviderTaxMethods(taxationGatewayProvider.key);
            promiseGwResources.then(function (methods) {

                var newCountries = _.map(methods, function(methodFromServer) {
                    var taxMethod = taxMethodDisplayBuilder.transform(methodFromServer);

                    var taxCountry = taxCountryDisplayBuilder.createDefault();
                    taxCountry.provider = taxationGatewayProvider;
                    taxCountry.taxMethod = taxMethod;
                    taxCountry.taxMethod.providerKey = taxationGatewayProvider.key;
                    if(taxCountry.taxMethod.countryCode === 'ELSE') {
                        taxCountry.country = countryDisplayBuilder.createDefault();
                        taxCountry.country.name = 'Everywhere Else';
                        taxCountry.country.countryCode = 'ELSE';
                    } else {
                        taxCountry.country = _.find($scope.allCountries, function(c) { return c.countryCode == taxMethod.countryCode; });
                    }
                    if (taxCountry.country) {
                        taxCountry.setCountryName(taxCountry.country.name);
                    } else {
                        taxCountry.setCountryName(taxMethod.name);
                    }
                    return taxCountry;
                });

                _.each(newCountries, function(country) {
                    if (country.country.countryCode === 'ELSE') {
                        country.sortOrder = 9999;
                    }
                    $scope.availableCountries.push(country);
                });


                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.error("Available Gateway Resources Load Failed", reason.message);
            });

        }


        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------
        function save() {
            $scope.preValuesLoaded = false;
            angular.forEach($scope.availableCountries, function(country) {
                if(country.provider.key === -1) {
                    // delete the provider
                    if(country.taxMethod.key !== '') {
                        var promiseDelete = taxationGatewayProviderResource.deleteTaxMethod(country.taxMethod.key);
                        promiseDelete.then(function() {
                            notificationsService.success("TaxMethod Removed", "");
                        }, function(reason) {
                            notificationsService.error("TaxMethod Save Failed", reason.message);
                        });
                    }
                } else {
                    if(country.taxMethod.providerKey !== country.provider.key) {
                            country.taxMethod.providerKey = country.provider.key;
                            country.taxMethod.countryCode = country.country.countryCode;
                            var promiseSave = taxationGatewayProviderResource.addTaxMethod(country.taxMethod);
                            promiseSave.then(function() {
                                notificationsService.success("TaxMethod Saved", "");
                            }, function(reason) {
                                notificationsService.error("TaxMethod Save Failed", reason.message);
                        });
                    } else {
                        var promiseSave = taxationGatewayProviderResource.saveTaxMethod(country.taxMethod);
                        promiseSave.then(function() {
                            notificationsService.success("TaxMethod Saved", "");
                        }, function(reason) {
                            notificationsService.error("TaxMethod Save Failed", reason.message);
                        });
                    }
                }
            });
            $timeout(function() {
                init();
            }, 400);
        }

        //--------------------------------------------------------------------------------------
        // Dialogs
        //--------------------------------------------------------------------------------------

        function taxMethodDialogConfirm(dialogData) {
            var promiseSave;

            // Save existing method
            promiseSave = taxationGatewayProviderResource.saveTaxMethod(dialogData.country.taxMethod);
            promiseSave.then(function () {
                notificationsService.success("Taxation Method Saved");
            }, function (reason) {
                notificationsService.error("Taxation Method Save Failed", reason.message);
            });
        }

        function editTaxMethodProvinces(country) {
            if (country) {
                var dialogData = dialogDataFactory.createEditTaxCountryDialogData();
                dialogData.country = country;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/dialogs/taxation.edittaxmethod.html',
                    show: true,
                    callback: taxMethodDialogConfirm,
                    dialogData: dialogData
                });
            }
        }

        // Initialize the controller
        init();
}]);

    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ReportsViewReportController
     * @function
     *
     * @description
     * The controller for the ViewReport page
     *
     * This is a bootstrapper to allow reports that are plugins to be loaded using the merchello application route.
     */
    angular.module('merchello').controller('Merchello.Backoffice.ReportsViewReportController',
        ['$scope', '$routeParams',
         function($scope, $routeParams) {

             $scope.loaded = true;
             $scope.preValuesLoaded = true;

             // Property to control the report to show
             $scope.reportParam = $routeParams.id;

             var re = /(\\)/g;
             var subst = '/';

             var result = $scope.reportParam.replace(re, subst);

             //$scope.reportPath = "/App_Plugins/Merchello.ExportOrders|ExportOrders.html";
             $scope.reportPath = "/App_Plugins/" + result + ".html";

    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ReportsListController
     * @function
     *
     * @description
     * The controller for the invoice payments view
     */
    angular.module('merchello').controller('Merchello.Backoffice.ReportsListController',
    ['$scope', 'merchelloTabsFactory',
    function($scope, merchelloTabsFactory) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;
        $scope.tabs = [];

        function init() {
            $scope.tabs = merchelloTabsFactory.createReportsTabs();
            $scope.tabs.setActive('reportslist');
        }

        // initialize the controller
        init();

    }]);

    'use strict';
    /**
     * @ngdoc controller
     * @name Merchello.Sales.Dialog.CapturePaymentController
     * @function
     *
     * @description
     * The controller for the dialog used in capturing payments on the sales overview page
     */
    angular.module('merchello')
        .controller('Merchello.Sales.Dialogs.CapturePaymentController',
        ['$scope', function($scope) {

            function round(num, places) {
                return +(Math.round(num + "e+" + places) + "e-" + places);
            }

            $scope.dialogData.amount = round($scope.dialogData.invoiceBalance, 2)

    }]);
'use strict';
/**
 * @ngdoc controller
 * @name Merchello.Sales.Dialog.CreateShipmentController
 * @function
 *
 * @description
 * The controller for the dialog used in creating a shipment
 */
angular.module('merchello')
    .controller('Merchello.Sales.Dialogs.CreateShipmentController',
    ['$scope', 'ShipmentRequestDisplay', 'OrderDisplay', function($scope, ShipmentRequestDisplay, OrderDisplay) {

        $scope.save = save;
        $scope.loaded = false;

        function init() {
            _.each($scope.dialogData.order.items, function(item) {
                item.selected = true;
            });
            //$scope.dialogData.shipMethods.alternatives = _.sortBy($scope.dialogData.shipMethods.alternatives, function(methods) { return method.name; } );
            if($scope.dialogData.shipMethods.selected === null || $scope.dialogData.shipMethods.selected === undefined) {
                $scope.dialogData.shipMethods.selected = $scope.dialogData.shipMethods.alternatives[0];
            }
            if($scope.dialogData)
            $scope.dialogData.shipmentRequest = new ShipmentRequestDisplay();
            $scope.dialogData.shipmentRequest.order = angular.extend($scope.dialogData.order, OrderDisplay);
            $scope.loaded = true;
        }

        function save() {
            $scope.dialogData.shipmentRequest.shipMethodKey = $scope.dialogData.shipMethods.selected.key;
            $scope.dialogData.shipmentRequest.shipmentStatusKey = $scope.dialogData.shipmentStatus.key;
            $scope.dialogData.shipmentRequest.trackingNumber = $scope.dialogData.trackingNumber;
            $scope.dialogData.shipmentRequest.order.items = _.filter($scope.dialogData.order.items, function (item) {
                return item.selected === true;
            });
            $scope.submit($scope.dialogData);
        }

        init();

    }]);

'use strict';
/**
 * @ngdoc controller
 * @name Merchello.Sales.Dialog.CreateShipmentController
 * @function
 *
 * @description
 * The controller for the dialog used in creating a shipment
 */
angular.module('merchello')
    .controller('Merchello.Sales.Dialogs.EditShipmentController',
    ['$scope', 'ShipmentRequestDisplay', 'OrderDisplay', function($scope, ShipmentRequestDisplay, OrderDisplay) {

        $scope.save = save;
        $scope.loaded = false;

        $scope.checkboxDisabled = checkboxDisabled;

        function init() {
            _.each($scope.dialogData.shipment.items, function(item) {
                item.selected = true;
            });
            $scope.loaded = true;
        }

        function checkboxDisabled() {
            return $scope.dialogData.shipment.shipmentStatus.name === 'Shipped' || $scope.dialogData.shipment.shipmentStatus.name === 'Delivered'
        }

        function save() {
            $scope.dialogData.shipment.items = _.filter($scope.dialogData.shipment.items, function (item) {
                return item.selected === true;
            });
            $scope.submit($scope.dialogData);
        }

        init();

    }]);


/**
 * @ngdoc controller
 * @name Merchello.Dashboards.InvoicePaymentsController
 * @function
 *
 * @description
 * The controller for the invoice payments view
 */
angular.module('merchello').controller('Merchello.Backoffice.InvoicePaymentsController',
    ['$scope', '$log', '$routeParams', 'merchelloTabsFactory',
        'invoiceResource', 'paymentResource', 'settingsResource',
        'invoiceDisplayBuilder', 'paymentDisplayBuilder',
        function($scope, $log, $routeParams, merchelloTabsFactory, invoiceResource, paymentResource, settingsResource,
        invoiceDisplayBuilder, paymentDisplayBuilder) {

            $scope.loaded = false;
            $scope.tabs = [];
            $scope.invoice = {};
            $scope.payments = [];
            $scope.settings = {};
            $scope.currencySymbol = '';
            $scope.remainingBalance = 0;

        function init() {
            var key = $routeParams.id;
            loadInvoice(key);
            $scope.tabs = merchelloTabsFactory.createSalesTabs(key);
            $scope.tabs.setActive('payments');
        }

        /**
         * @ngdoc method
         * @name loadInvoice
         * @function
         *
         * @description - Load an invoice with the associated id.
         */
        function loadInvoice(id) {
            var promise = invoiceResource.getByKey(id);
            promise.then(function (invoice) {
                $scope.invoice = invoiceDisplayBuilder.transform(invoice);
                $scope.billingAddress = $scope.invoice.getBillToAddress();
                // append the customer tab if needed
                $scope.tabs.appendCustomerTab($scope.invoice.customerKey);
                loadPayments(id);
                loadSettings();
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                //console.info($scope.invoice);
            }, function (reason) {
                notificationsService.error("Invoice Load Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         *
         * @description - Load the Merchello settings.
         */
        function loadSettings() {

            var settingsPromise = settingsResource.getAllSettings();
            settingsPromise.then(function(settings) {
                $scope.settings = settings;
            }, function(reason) {
                notificationsService.error('Failed to load global settings', reason.message);
            })

            var currencySymbolPromise = settingsResource.getAllCurrencies();
            currencySymbolPromise.then(function (symbols) {
                var currency = _.find(symbols, function(symbol) {
                    return symbol.currencyCode === $scope.invoice.getCurrencyCode()
                });
                $scope.currencySymbol = currency.symbol;
            }, function (reason) {
                alert('Failed: ' + reason.message);
            });
        };

        function loadPayments(key)
        {
            var paymentsPromise = paymentResource.getPaymentsByInvoice(key);
            paymentsPromise.then(function(payments) {
                $scope.payments = paymentDisplayBuilder.transform(payments);
                $scope.remainingBalance = $scope.invoice.remainingBalance($scope.payments);
            }, function(reason) {
                notificationsService.error('Failed to load payments for invoice', reason.message);
            });
        }

        init();
}]);

/**
 * @ngdoc controller
 * @name Merchello.Dashboards.OrderShipmentsController
 * @function
 *
 * @description
 * The controller for the order shipments view
 */
angular.module('merchello').controller('Merchello.Backoffice.OrderShipmentsController',
    ['$scope', '$routeParams', '$log', 'notificationsService', 'dialogService', 'dialogDataFactory', 'merchelloTabsFactory',
        'invoiceResource', 'settingsResource', 'shipmentResource',
        'invoiceDisplayBuilder', 'shipmentDisplayBuilder',
        function($scope, $routeParams, $log, notificationsService, dialogService, dialogDataFactory, merchelloTabsFactory, invoiceResource,
                 settingsResource, shipmentResource, invoiceDisplayBuilder, shipmentDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.invoice = {};
            $scope.settings = {};
            $scope.shipments = [];

            // methods
            $scope.isEditableAddress = isEditableAddress;
            $scope.updateShippingAddressLineItem = updateShippingAddressLineItem;

            // dialogs
            $scope.openShipmentDialog = openShipmentDialog;
            $scope.openAddressDialog = openAddressDialog;
            $scope.openDeleteShipmentDialog = openDeleteShipmentDialog;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description - Controller initialization.
             */
            function init() {
                var key = $routeParams.id;
                loadInvoice(key);
                $scope.tabs = merchelloTabsFactory.createSalesTabs(key);
                $scope.tabs.setActive('shipments');
                $scope.loaded = true;
            }

            /**
             * @ngdoc method
             * @name loadInvoice
             * @function
             *
             * @description - responsible for loading the invoice.
             */
            function loadInvoice(key) {
                var invoicePromise = invoiceResource.getByKey(key);
                invoicePromise.then(function(invoice) {
                    $scope.invoice = invoice;
                    // append the customer tab
                    $scope.tabs.appendCustomerTab($scope.invoice.customerKey);
                    loadSettings();
                    var shipmentsPromise = shipmentResource.getShipmentsByInvoice(invoice);
                    shipmentsPromise.then(function(shipments) {
                        $scope.shipments = shipmentDisplayBuilder.transform(shipments);
                        $scope.preValuesLoaded = true;
                    })
                }, function(reason) {
                    notificationsService.error('Failed to load invoice', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {
                var settingsPromise = settingsResource.getAllSettings();
                settingsPromise.then(function (settings) {
                    $scope.settings = settings;
                }, function (reason) {
                    notificationsService.error('Failed to load global settings', reason.message);
                })
            }

            /**
             * @ngdoc method
             * @name isEditableStatus
             * @function
             *
             * @description - Returns a value indicating whether or not the shipment address can be edited.
             */
            function isEditableAddress(shipmentStatus) {
                if (shipmentStatus.name === 'Delivered' || shipmentStatus.name === 'Shipped') {
                    return false;
                }
                return true;
            }

            /*--------------------------------------------------------------------------------
                Dialogs
            ----------------------------------------------------------------------------------*/

            function updateShippingAddressLineItem(shipment) {
                var promise = shipmentResource.updateShippingAddressLineItem(shipment);
                promise.then(function() {
                    loadInvoice($scope.invoice.key);
                    notificationsService.success('Successfully updated sales shipping address.')
                }, function(reason) {
                    notificationsService.error('Failed to update shipping addresses on invoice', reason.message);
                })
            }


            function openDeleteShipmentDialog(shipment) {
                var dialogData = {};
                dialogData.name = 'Shipment #' + shipment.shipmentNumber;
                dialogData.shipment = shipment;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteShipmentDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name openShipmentDialog
             * @function
             *
             * @description - responsible for opening the edit shipment dialog and passing the selected shipment.
             */
            function openShipmentDialog(shipment) {
                var promiseStatuses = shipmentResource.getAllShipmentStatuses();
                promiseStatuses.then(function(statuses) {
                    var dialogData = dialogDataFactory.createEditShipmentDialogData();
                    dialogData.shipment = shipment;
                    dialogData.shipmentStatuses = statuses;
                    dialogData.shipment.shipmentStatus = _.find(statuses, function(status) {
                      return status.key === dialogData.shipment.shipmentStatus.key;
                    });

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.edit.shipment.html',
                        show: true,
                        callback: processUpdateShipment,
                        dialogData: dialogData
                    });
                });
            }

            /**
             * @ngdoc method
             * @name openAddressDialog
             * @function
             *
             * @description - responsible for opening the edit address dialog with the appropriate address to be edited
             */
            function openAddressDialog(shipment, addressType) {
                var dialogData = dialogDataFactory.createEditAddressDialogData();
                if (addressType === 'destination') {
                    dialogData.address = shipment.getDestinationAddress();
                    dialogData.showPhone = true;
                    dialogData.showEmail = true;
                    dialogData.showIsCommercial = true;
                }
                else
                {
                    dialogData.address = shipment.getOriginAddress();
                }

                // add the shipment -- this modifies the EditAddressDialogData model with an extra property
                dialogData.shipment = shipment;

                // get the list of countries to populate the countries drop down
                var countryPromise = settingsResource.getAllCountries();
                countryPromise.then(function(countries) {
                    dialogData.countries = countries;

                    dialogData.selectedCountry = _.find(countries, function(country) {
                        return country.countryCode === dialogData.address.countryCode;
                    });

                    // if this address has a region ... we need to get that too.
                    if(dialogData.address.region !== '' && dialogData.address.region !== null && dialogData.selectedCountry.provinces.length > 0) {
                        dialogData.selectedProvince = _.find(dialogData.selectedCountry.provinces, function(region) {
                            return region.code === dialogData.address.region;
                        });
                    }

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/edit.address.html',
                        show: true,
                        callback: addressType === 'destination' ? processUpdateDestinationAddress : processUpdateOriginAddress,
                        dialogData: dialogData
                    });
                });
            }

            /**
             * @ngdoc method
             * @name processUpdateOriginAddres
             * @function
             *
             * @description - updates the origin address on the shipment.
             */
            function processUpdateOriginAddress(dialogData) {
                $scope.preValuesLoaded = false;
                var shipment = dialogData.shipment;
                shipment.setOriginAddress(dialogData.address);
                saveShipment(shipment);
            }

            /**
             * @ngdoc method
             * @name processUpdateDestinationAddress
             * @function
             *
             * @description - updates the destination address of a shipment.
             */
            function processUpdateDestinationAddress(dialogData) {
                $scope.preValuesLoaded = false;
                var shipment = dialogData.shipment;
                shipment.setDestinationAddress(dialogData.address);
                saveShipment(shipment);
            }

            /**
             * @ngdoc method
             * @name processUpdateShipment
             * @function
             *
             * @description - responsible for handling dialog data for updating a shipment.
             */
            function processUpdateShipment(dialogData) {
                $scope.preValuesLoaded = false;
                if(dialogData.shipment.items.length > 0) {
                    saveShipment(dialogData.shipment);
                } else {
                    notificationsService.warning('Cannot remove all items from the shipment.  Instead, consider deleting the shipment.');
                    loadInvoice($scope.invoice.key);
                };
            }

            /**
             * @ngdoc method
             * @name processDeleteShipmentDialog
             * @function
             *
             * @description - responsible for deleting a shipment.
             */
            function processDeleteShipmentDialog(dialogData) {
                var promise = shipmentResource.deleteShipment(dialogData.shipment);
                promise.then(function() {
                    loadInvoice($scope.invoice.key);
                }, function(reason) {
                  notificationsService.error('Failed to delete the invoice.', reason.message)
                });
            }

            /**
             * @ngdoc method
             * @name saveShipment
             * @function
             *
             * @description - responsible for saving a shipment.
             */
            function saveShipment(shipment) {

                var promise = shipmentResource.saveShipment(shipment);
                promise.then(function(shipment) {
                    loadInvoice($scope.invoice.key);
                });
            }


            // initializes the controller
            init();
    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Sales.OverviewController
     * @function
     *
     * @description
     * The controller for the sales overview view
     */
    angular.module('merchello').controller('Merchello.Backoffice.SalesOverviewController',
        ['$scope', '$routeParams', '$timeout', '$log', 'assetsService', 'dialogService', 'localizationService', 'notificationsService',
            'auditLogResource', 'invoiceResource', 'settingsResource', 'paymentResource', 'shipmentResource',
            'orderResource', 'dialogDataFactory', 'merchelloTabsFactory', 'addressDisplayBuilder', 'salesHistoryDisplayBuilder',
            'invoiceDisplayBuilder', 'paymentDisplayBuilder', 'shipMethodsQueryDisplayBuilder',
        function($scope, $routeParams, $timeout, $log, assetsService, dialogService, localizationService, notificationsService,
                 auditLogResource, invoiceResource, settingsResource, paymentResource, shipmentResource, orderResource, dialogDataFactory,
                 merchelloTabsFactory, addressDisplayBuilder, salesHistoryDisplayBuilder, invoiceDisplayBuilder, paymentDisplayBuilder, shipMethodsQueryDisplayBuilder) {

            // exposed properties
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.invoice = {};
            $scope.tabs = [];
            $scope.historyLoaded = false;
            $scope.remainingBalance = 0.0;
            $scope.shippingTotal = 0.0;
            $scope.taxTotal = 0.0;
            $scope.currencySymbol = '';
            $scope.settings = {};
            $scope.salesHistory = {};
            $scope.payments = [];
            $scope.billingAddress = {};
            $scope.hasShippingAddress = false;
            $scope.authorizedCapturedLabel = '';
            $scope.shipmentLineItems = [];

            // exposed methods
            //  dialogs
            $scope.capturePayment = capturePayment;
            $scope.showFulfill = true;
            $scope.capturePaymentDialogConfirm = capturePaymentDialogConfirm,
            $scope.openDeleteInvoiceDialog = openDeleteInvoiceDialog;
            $scope.processDeleteInvoiceDialog = processDeleteInvoiceDialog,
            $scope.openFulfillShipmentDialog = openFulfillShipmentDialog;
            $scope.processFulfillShipmentDialog = processFulfillShipmentDialog;

            // localize the sales history message
            $scope.localizeMessage = localizeMessage;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description - Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init () {
                loadInvoice($routeParams.id);
                $scope.tabs = merchelloTabsFactory.createSalesTabs($routeParams.id);
                $scope.tabs.setActive('overview');
                $scope.loaded = true;
            }

            function localizeMessage(msg) {
                return msg.localize(localizationService);
            }

            /**
             * @ngdoc method
             * @name loadAuditLog
             * @function
             *
             * @description
             * Load the Audit Log for the invoice via API.
             */
            function loadAuditLog(key) {
                if (key !== undefined) {
                    var promise = auditLogResource.getSalesHistoryByInvoiceKey(key);
                    promise.then(function (response) {
                        var history = salesHistoryDisplayBuilder.transform(response);
                        // TODO this is a patch for a problem in the API
                        if (history.dailyLogs.length) {
                            $scope.salesHistory = history.dailyLogs;
                            angular.forEach(history.dailyLogs, function(daily) {
                              angular.forEach(daily.logs, function(log) {
                                 localizationService.localize(log.message.localizationKey(), log.message.localizationTokens()).then(function(value) {
                                    log.message.formattedMessage = value;
                                 });
                              });
                            });
                        }
                        $scope.historyLoaded = history.dailyLogs.length > 0;
                    }, function (reason) {
                        notificationsService.error('Failed to load sales history', reason.message);
                    });
                }
            }

            /**
             * @ngdoc method
             * @name loadInvoice
             * @function
             *
             * @description - Load an invoice with the associated id.
             */
            function loadInvoice(id) {
                var promise = invoiceResource.getByKey(id);
                promise.then(function (invoice) {
                    $scope.invoice = invoiceDisplayBuilder.transform(invoice);
                    $scope.billingAddress = $scope.invoice.getBillToAddress();
                    $scope.taxTotal = $scope.invoice.getTaxLineItem().price;
                    $scope.shippingTotal = $scope.invoice.shippingTotal();
                    loadSettings();
                    loadPayments(id);
                    loadAuditLog(id);
                    loadShippingAddress(id);
                    $scope.showFulfill = hasUnPackagedLineItems();
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                    var shipmentLineItem = $scope.invoice.getShippingLineItems();
                    if (shipmentLineItem) {
                        $scope.shipmentLineItems.push(shipmentLineItem);
                    }

                   $scope.tabs.appendCustomerTab($scope.invoice.customerKey);

                }, function (reason) {
                    notificationsService.error("Invoice Load Failed", reason.message);
                });
            }


           /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {

               var settingsPromise = settingsResource.getAllSettings();
               settingsPromise.then(function(settings) {
                   $scope.settings = settings;
               }, function(reason) {
                   notificationsService.error('Failed to load global settings', reason.message);
               })

                var currencySymbolPromise = settingsResource.getAllCurrencies();
                currencySymbolPromise.then(function (symbols) {
                    var currency = _.find(symbols, function(symbol) {
                        return symbol.currencyCode === $scope.invoice.getCurrencyCode()
                    });
                    $scope.currencySymbol = currency.symbol;
                }, function (reason) {
                    alert('Failed: ' + reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadPayments
             * @function
             *
             * @description - Load the Merchello payments for the invoice.
             */
            function loadPayments(key) {
                var paymentsPromise = paymentResource.getPaymentsByInvoice(key);
                paymentsPromise.then(function(payments) {
                    $scope.payments = paymentDisplayBuilder.transform(payments);
                    $scope.remainingBalance = $scope.invoice.remainingBalance($scope.payments);
                    $scope.authorizedCapturedLabel  = $scope.remainingBalance == '0' ? 'merchelloOrderView_captured' : 'merchelloOrderView_authorized';

                }, function(reason) {
                    notificationsService.error('Failed to load payments for invoice', reason.message);
                });
            }

            function loadShippingAddress(key) {
                var shippingAddressPromise = orderResource.getShippingAddress(key);
                shippingAddressPromise.then(function(result) {
                      $scope.shippingAddress = addressDisplayBuilder.transform(result);
                      $scope.hasShippingAddress = true;
                }, function(reason) {
                    notificationsService.error('Failed to load shipping address', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name capturePayment
             * @function
             *
             * @description - Open the capture shipment dialog.
             */
            function capturePayment() {
                var data = dialogDataFactory.createCapturePaymentDialogData();
                data.setPaymentData($scope.payments[0]);
                data.setInvoiceData($scope.payments, $scope.invoice, $scope.currencySymbol);
                if (!data.isValid()) {
                    return false;
                }
                // TODO inject the template for the capture payment dialog so that we can
                // have different fields for other providers
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.capture.payment.html',
                    show: true,
                    callback: $scope.capturePaymentDialogConfirm,
                    dialogData: data
                });
            }

            /**
             * @ngdoc method
             * @name capturePaymentDialogConfirm
             * @function
             *
             * @description - Capture the payment after the confirmation dialog was passed through.
             */
            function capturePaymentDialogConfirm(paymentRequest) {
                $scope.preValuesLoaded = false;
                var promiseSave = paymentResource.capturePayment(paymentRequest);
                promiseSave.then(function (payment) {
                    // added a timeout here to give the examine index
                    $timeout(function() {
                        notificationsService.success("Payment Captured");
                        loadInvoice(paymentRequest.invoiceKey);
                    }, 400);
                }, function (reason) {
                    notificationsService.error("Payment Capture Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name openDeleteInvoiceDialog
             * @function
             *
             * @description - Open the delete payment dialog.
             */
            function openDeleteInvoiceDialog() {
                var dialogData = {};
                dialogData.name = 'Invoice #' + $scope.invoice.invoiceNumber;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteInvoiceDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name openFulfillShipmentDialog
             * @function
             *
             * @description - Open the fufill shipment dialog.
             */
            function openFulfillShipmentDialog() {
                var promiseStatuses = shipmentResource.getAllShipmentStatuses();
                promiseStatuses.then(function(statuses) {
                    var data = dialogDataFactory.createCreateShipmentDialogData();
                    data.order = $scope.invoice.orders[0]; // todo: pull from current order when multiple orders is available
                    data.order.items = data.order.getUnShippedItems();
                    data.shipmentStatuses = statuses;

                    // packaging
                    var quotedKey = '7342dcd6-8113-44b6-bfd0-4555b82f9503';
                    data.shipmentStatus = _.find(data.shipmentStatuses, function(status) {
                        return status.key === quotedKey;
                    });
                    data.invoiceKey = $scope.invoice.key;

                    // TODO this could eventually turn into an array
                    var shipmentLineItem = $scope.invoice.getShippingLineItems();
                    if ($scope.shipmentLineItems[0]) {
                        var shipMethodKey = $scope.shipmentLineItems[0].extendedData.getValue('merchShipMethodKey');
                        var request = { shipMethodKey: shipMethodKey, invoiceKey: data.invoiceKey, lineItemKey: $scope.shipmentLineItems[0].key };
                        var shipMethodPromise = shipmentResource.getShipMethodAndAlternatives(request);
                        shipMethodPromise.then(function(result) {
                            data.shipMethods = shipMethodsQueryDisplayBuilder.transform(result);
                            data.shipMethods.selected = _.find(data.shipMethods.alternatives, function(method) {
                                return method.key === shipMethodKey;
                            });
                            dialogService.open({
                                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.create.shipment.html',
                                show: true,
                                callback: $scope.processFulfillShipmentDialog,
                                dialogData: data
                            });

                        });
                    }
                });
            }

            /**
             * @ngdoc method
             * @name processDeleteInvoiceDialog
             * @function
             *
             * @description - Delete the invoice.
             */
            function processDeleteInvoiceDialog() {
                var promiseDeleteInvoice = invoiceResource.deleteInvoice($scope.invoice.key);
                promiseDeleteInvoice.then(function (response) {
                    notificationsService.success('Invoice Deleted');
                    window.location.href = '#/merchello/merchello/saleslist/manage';
                }, function (reason) {
                    notificationsService.error('Failed to Delete Invoice', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name processFulfillPaymentDialog
             * @function
             *
             * @description - Process the fulfill shipment functionality on callback from the dialog service.
             */
            function processFulfillShipmentDialog(data) {
                $scope.preValuesLoaded = false;
                if(data.shipmentRequest.order.items.length > 0) {
                    var promiseNewShipment = shipmentResource.newShipment(data.shipmentRequest);
                    promiseNewShipment.then(function (shipment) {
                        $timeout(function() {
                            notificationsService.success('Shipment #' + shipment.shipmentNumber + ' created');
                            loadInvoice(data.invoiceKey);
                        }, 400);

                    }, function (reason) {
                        notificationsService.error("New Shipment Failed", reason.message);
                    });
                } else {
                    $scope.preValuesLoaded = true;
                    notificationsService.warning('Shipment would not contain any items', 'The shipment was not created as it would not contain any items.');
                }
            }

            /**
             * @ngdoc method
             * @name hasUnPackagedLineItems
             * @function
             *
             * @description - Process the fulfill shipment functionality on callback from the dialog service.
             */
            function hasUnPackagedLineItems() {
                var fulfilled = $scope.invoice.getFulfillmentStatus() === 'Fulfilled';
                if (fulfilled) {
                    return false;
                }
                var i = 0; // order count
                var found = false;
                while(i < $scope.invoice.orders.length && !found) {
                    var item = _.find($scope.invoice.orders[ i ].items, function(item) {
                      return item.shipmentKey === '' || item.shipmentKey === null;
                    });
                    if(item !== null && item !== undefined) {
                        found = true;
                    } else {
                        i++;
                    }
                }

                return found;
            }

            // initialize the controller
            init();
    }]);

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
    ['$scope', '$element', '$log', 'angularHelper', 'assetsService', 'notificationsService', 'merchelloTabsFactory', 'settingsResource',
        'invoiceResource', 'queryDisplayBuilder', 'queryResultDisplayBuilder', 'invoiceDisplayBuilder',
        function($scope, $element, $log, angularHelper, assetsService, notificationService, merchelloTabsFactory, settingsResource, invoiceResource,
                 queryDisplayBuilder, queryResultDisplayBuilder, invoiceDisplayBuilder)
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
            //$scope.currencySymbol = '$';
            $scope.settings = {};
            $scope.sortOrder = "desc";
            $scope.sortProperty = "-invoiceNumber";
            $scope.visible = {};
            $scope.visible.bulkActionDropdown = false;
            $scope.currentFilters = [];

            // exposed methods
            $scope.getCurrencySymbol = getCurrencySymbol;

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
                var query = buildQuery($scope.filterText);
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
                var query = buildQuery($scope.filterText);
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
                var query = buildQuery($scope.filterText);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name filterWithWildcard
             * @function
             *
             * @description
             * Fired when the filter button next to the filter text box at the top of the page is clicked.
             */
            $scope.filterWithWildcard = function (filterText) {
                var query = buildQuery(filterText);
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
            $scope.filterWithDates = function(filterStartDate, filterEndDate) {
                var query = buildQueryDates(filterStartDate, filterEndDate);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name resetFilters
             * @function
             *
             * @description
             * Fired when the reset filter button is clicked.
             */
            $scope.resetFilters = function () {
                var query = buildQuery();
                $scope.currentFilters = [];
                $scope.filterText = "";
                $scope.filterStartDate = "";
                $scope.filterEndDate = "";
                loadInvoices(query);
                $scope.filterAction = false;
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
                loadInvoices(buildQuery());
                $scope.tabs = merchelloTabsFactory.createSalesListTabs();
                $scope.tabs.setActive('saleslist');
                $scope.loaded = true;
            }

            function loadInvoices(query) {
                $scope.salesLoaded = false;
                $scope.salesLoaded = false;

                var promise = invoiceResource.searchInvoices(query);
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
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {
                var currenciesPromise = settingsResource.getAllCurrencies();
                currenciesPromise.then(function(currencies) {
                    allCurrencies = currencies;
                }, function(reason) {
                    alert('Failed' + reason.message);
                });

                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    globalCurrency = currencySymbol;
                }, function (reason) {
                    alert('Failed: ' + reason.message);
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
            function buildQuery(filterText) {
                var page = $scope.currentPage;
                var perPage = $scope.limitAmount;
                var sortBy = $scope.sortInfo().sortBy;
                var sortDirection = $scope.sortInfo().sortDirection;
                if (filterText === undefined) {
                    filterText = '';
                }
                if (filterText !== $scope.filterText) {
                    page = 0;
                    $scope.currentPage = 0;
                }
                $scope.filterText = filterText;

                var query = queryDisplayBuilder.createDefault();
                query.currentPage = page;
                query.itemsPerPage = perPage;
                query.sortBy = sortBy;
                query.sortDirection = sortDirection;
                query.addFilterTermParam(filterText)

                if (query.parameters.length > 0) {
                    $scope.currentFilters = query.parameters;
                }
                return query;
            };

            /**
             * @ngdoc method
             * @name buildQueryDates
             * @function
             *
             * @description
             * Perpares a new query object for passing to the ApiController
             */
             function buildQueryDates(startDate, endDate) {
                var page = $scope.currentPage;
                var perPage = $scope.limitAmount;
                var sortBy = $scope.sortInfo().sortBy;
                var sortDirection = $scope.sortInfo().sortDirection;
                if (startDate === undefined && endDate === undefined) {
                    $scope.currentFilters = [];
                } else {
                    $scope.currentFilters = [{
                        fieldName: 'invoiceDateStart',
                        value: startDate
                    }, {
                        fieldName: 'invoiceDateEnd',
                        value: endDate
                    }];
                }
                if (startDate !== $scope.filterStartDate) {
                    page = 0;
                    $scope.currentPage = 0;
                }
                $scope.filterStartDate = startDate;
                var query = buildQuery();
                query.addInvoiceDateParam(startDate, 'start');
                query.addInvoiceDateParam(endDate, 'end');

                return query;
            };

            /**
             * @ngdoc method
             * @name setupDatePicker
             * @function
             *
             * @description
             * Sets up the datepickers
             */
            function setupDatePicker(pickerId) {

                // Open the datepicker and add a changeDate eventlistener
                $element.find(pickerId).datetimepicker({
                    pickTime: false
                });

                //Ensure to remove the event handler when this instance is destroyted
                $scope.$on('$destroy', function () {
                    $element.find(pickerId).datetimepicker("destroy");
                });
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


            //// Initialize
            assetsService.loadCss('/umbraco/lib/datetimepicker/bootstrap-datetimepicker.min.css').then(function () {
                var filesToLoad = [
                    'lib/moment/moment-with-locales.js',
                    'lib/datetimepicker/bootstrap-datetimepicker.js'];
                assetsService.load(filesToLoad).then(
                    function () {
                        //The Datepicker js and css files are available and all components are ready to use.

                        setupDatePicker("#filterStartDate");
                        $element.find("#filterStartDate").datetimepicker().on("changeDate", $scope.applyDateStart);

                        setupDatePicker("#filterEndDate");
                        $element.find("#filterEndDate").datetimepicker().on("changeDate", $scope.applyDateEnd);
                    });
            });

            init();
        }]);


})();