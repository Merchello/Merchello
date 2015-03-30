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
     * The controller for customer addresses view
     */
    angular.module('merchello').controller('Merchello.Backoffice.CustomerAddressesController',
        ['$scope', '$routeParams', '$timeout', 'dialogService', 'notificationsService', 'settingsResource', 'merchelloTabsFactory', 'customerResource',
            'countryDisplayBuilder', 'customerDisplayBuilder',
        function($scope, $routeParams, $timeout, dialogService, notificationsService, settingsResource, merchelloTabsFactory, customerResource,
                 countryDisplayBuilder, customerDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.customer = {};
            $scope.billingAddresses = [];
            $scope.shippingAddresses = [];
            $scope.countries = [];

            // exposed methods
            $scope.reload = init;
            $scope.save = save;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Initialize the controller.
             */
            function init() {
                var key = $routeParams.id;
                loadCustomer(key);
                loadCountries();
            }

            /**
             * @ngdoc method
             * @name loadCustomer
             * @function
             *
             * @description
             * Loads the customer.
             */
            function loadCustomer(key) {
                var promiseLoadCustomer = customerResource.GetCustomer(key);
                promiseLoadCustomer.then(function(customerResponse) {
                    $scope.customer = customerDisplayBuilder.transform(customerResponse);
                    $scope.tabs = merchelloTabsFactory.createCustomerOverviewTabs(key, $scope.customer.hasAddresses());
                    $scope.tabs.setActive('addresses');
                    $scope.shippingAddresses = _.sortBy($scope.customer.getShippingAddresses(), function(adr) { return adr.label; });
                    $scope.billingAddresses = _.sortBy($scope.customer.getBillingAddresses(), function(adr) { return adr.label; });
                    loadCountries();
                }, function(reason) {
                    notificationsService.error("Failed to load customer", reason.message);
                });
            }

            function loadCountries() {
                var promise = settingsResource.getAllCountries();
                promise.then(function(countries) {
                    $scope.countries = countryDisplayBuilder.transform(countries);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function(reason) {
                    notificationsService.error('Failed to load add countries', reason);
                });
            }

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Save the customer.
             */
            function save(customer) {
                $scope.preValuesLoaded = false;
                notificationsService.info("Saving...", "");
                var promiseSaveCustomer = customerResource.SaveCustomer($scope.customer);
                promiseSaveCustomer.then(function(customer) {
                    $timeout(function() {
                        notificationsService.success("Customer Saved", "");
                        init();
                    }, 400);

                }, function(reason) {
                    notificationsService.error("Customer  Failed", reason.message);
                });
            }

            // initialize the controller
            init();
    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.CustomerListController
     * @function
     *
     * @description
     * The controller for customer list view
     */
    angular.module('merchello').controller('Merchello.Backoffice.CustomerListController',
        ['$scope', 'dialogService', 'notificationsService', 'merchelloTabsFactory', 'dialogDataFactory', 'customerResource', 'queryDisplayBuilder',
            'queryResultDisplayBuilder', 'customerDisplayBuilder',
        function($scope, dialogService, notificationsService, merchelloTabsFactory, dialogDataFactory, customerResource,
                 queryDisplayBuilder, queryResultDisplayBuilder, customerDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;

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


            // exposed methods
            $scope.loadCustomers = loadCustomers;
            $scope.resetFilters = resetFilters;
            $scope.openNewCustomerDialog = openNewCustomerDialog;
            $scope.numberOfPages = numberOfPages;
            $scope.limitChanged = limitChanged;
            $scope.changePage = changePage;
            $scope.changeSortOrder = changeSortOrder;

            var maxPages = 0;

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
                $scope.loaded = true;
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

                var promiseAllCustomers = customerResource.searchCustomers(query);
                promiseAllCustomers.then(function (customersResponse) {
                    $scope.customers = [];
                    var queryResult = queryResultDisplayBuilder.transform(customersResponse, customerDisplayBuilder);
                    $scope.customers = queryResult.items;
                    maxPages = queryResult.totalPages;
                    if(query.parameters.length >= 0) {
                        $scope.currentFilters = query.parameters;
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
                    init();
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
        ['$scope', '$routeParams', '$timeout', 'dialogService', 'notificationsService', 'gravatarService', 'settingsResource', 'invoiceHelper', 'merchelloTabsFactory', 'dialogDataFactory',
            'customerResource', 'customerDisplayBuilder', 'countryDisplayBuilder', 'currencyDisplayBuilder', 'settingDisplayBuilder',
        function($scope, $routeParams, $timeout, dialogService, notificationsService, gravatarService, settingsResource, invoiceHelper, merchelloTabsFactory, dialogDataFactory,
                 customerResource, customerDisplayBuilder, countryDisplayBuilder, currencyDisplayBuilder, settingDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.avatarUrl = "";
            $scope.defaultShippingAddress = {};
            $scope.defaultBillingAddress = {};
            $scope.customer = {};
            $scope.invoiceTotals = [];
            $scope.settings = {}

            // exposed methods
            $scope.getCurrency = getCurrency;
            $scope.openEditInfoDialog = openEditInfoDialog;
            $scope.openDeleteCustomerDialog = openDeleteCustomerDialog;
            $scope.openAddressAddEditDialog = openAddressAddEditDialog;
            $scope.saveCustomer = saveCustomer;

            // private properties
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
                    $scope.avatarUrl = gravatarService.getAvatarUrl($scope.customer.email);
                    $scope.defaultBillingAddress = $scope.customer.getDefaultBillingAddress();
                    $scope.defaultShippingAddress = $scope.customer.getDefaultShippingAddress();
                    $scope.tabs = merchelloTabsFactory.createCustomerOverviewTabs(key, $scope.customer.hasAddresses());
                    $scope.tabs.setActive('overview');
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                    console.info($scope.customer);
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
                    $scope.settings = settingDisplayBuilder.transform(settingsResponse);

                    // we need all of the currencies since invoices may be billed in various currencies
                    var promiseCurrencies = settingsResource.getAllCurrencies();
                    promiseCurrencies.then(function(currenciesResponse) {
                        currencies = currencyDisplayBuilder.transform(currenciesResponse);

                        // get the default currency from the settings in case we cannot determine
                        // the currency used in an invoice
                        defaultCurrency = _.find(currencies, function(c) {
                            return c.currencyCode === $scope.settings.currencyCode;
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
            function openAddressAddEditDialog(address) {
                var dialogData = dialogDataFactory.createAddEditCustomerAddressDialogData();
                // if the address is not defined we need to create a default (empty) CustomerAddressDisplay
                if(address === null || address === undefined) {
                    dialogData.customerAddress = customerAddressDisplayBuilder.createDefault();
                    dialogData.selectedCountry = countries[0];
                } else {
                    dialogData.customerAddress = address;
                    dialogData.selectedCountry = address.countryCode === '' ? countries[0] :
                        _.find(countries, function(country) {
                        return country.countryCode === address.countryCode;
                    });
                }
                dialogData.countries = countries;
                dialogData.customerAddress.customerKey = $scope.customer.key;
                if (dialogData.selectedCountry.hasProvinces()) {
                    if(dialogData.customerAddress.region !== '') {
                        dialogData.selectedProvince = _.find(dialogData.selectedCountry.provinces, function(province) {
                            return province.code === address.region;
                        });
                    }
                    if(dialogData.selectedProvince === null || dialogData.selectedProvince === undefined) {
                        dialogData.selectedProvince = dialogData.selectedCountry.provinces[0];
                    }
                }
                // if the customer has not addresses of the given type we are going to force an added
                // address to be the primary address
                if(!$scope.customer.hasDefaultAddressOfType(dialogData.customerAddress.addressType) || address.isDefault) {
                    dialogData.customerAddress.isDefault = true;
                    dialogData.setDefault = true;
                }
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/customer.customeraddress.addedit.html',
                    show: true,
                    callback: processAddEditAddressDialog,
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
                var dialogData = dialogDataFactory.createDeleteCustomerDialogData();
                dialogData.customer = $scope.customer;
                dialogData.name = $scope.customer.firstName + ' ' + $scope.customer.lastName;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteCustomerDialog,
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

                var dialogData = dialogDataFactory.createAddEditCustomerDialogData();
                dialogData.firstName = $scope.customer.firstName;
                dialogData.lastName = $scope.customer.lastName;
                dialogData.email = $scope.customer.email;

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/customer.info.addedit.html',
                    show: true,
                    callback: processEditInfoDialog,
                    dialogData: dialogData
                });
            }


            /**
             * @ngdoc method
             * @name processEditAddressDialog
             * @function
             *
             * @description
             * Edit an address and update the associated lists.
             */
            function processAddEditAddressDialog(dialogData) {
                var defaultAddressOfType = $scope.customer.getDefaultAddress(dialogData.customerAddress.addressType);
                if(dialogData.customerAddress.key !== '') {
                    $scope.customer.addresses =_.reject($scope.customer.addresses, function(address) {
                      return address.key == dialogData.customerAddress.key;
                    });
                }
                if (dialogData.customerAddress.isDefault && defaultAddressOfType !== undefined) {
                    if(dialogData.customerAddress.key !== defaultAddressOfType.key) {
                        defaultAddressOfType.isDefault = false;
                    }
                }
                $scope.customer.addresses.push(dialogData.customerAddress);
                saveCustomer();
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
                notificationsService.info("Deleting " + dialogData.customer.firstName + " " + dialogData.customer.lastName, "");
                var promiseDeleteCustomer = customerResource.DeleteCustomer(dialogData.customer.key);
                promiseDeleteCustomer.then(function() {
                    notificationsService.success("Customer deleted.", "");
                    window.location.hash = "#/merchello/merchello/customerList/manage";
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
            function processEditInfoDialog(dialogData) {
                $scope.customer.firstName = dialogData.firstName;
                $scope.customer.lastName = dialogData.lastName;
                $scope.customer.email = dialogData.email;
                saveCustomer();
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
                $scope.preValuesLoaded = false;
                notificationsService.info("Saving...", "");
                var promiseSaveCustomer = customerResource.SaveCustomer($scope.customer);
                promiseSaveCustomer.then(function(customerResponse) {
                    $timeout(function() {
                    notificationsService.success("Customer Saved", "");
                        loadCustomer($scope.customer.key);
                    }, 400);

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
     * @name Merchello.Customer.Dialogs.CustomerAddressAddEditController
     * @function
     *
     * @description
     * The controller for adding and editing customer addresses
     */
    angular.module('merchello').controller('Merchello.Customer.Dialogs.CustomerAddressAddEditController',
        ['$scope',
        function($scope) {

            // exposed methods
            $scope.updateProvinceList = updateProvinceList;
            $scope.toTitleCase = toTitleCase;
            $scope.save = save;

            function updateProvinceList() {
                // try to find the province matching the province code of the customer address
                var provinceCode = $scope.dialogData.customerAddress.region;
                if($scope.dialogData.selectedCountry.provinces.length > 0) {
                    var province = _.find($scope.dialogData.selectedCountry.provinces, function(p) {
                        return p.code === provinceCode;
                    });
                    if (province === null || province === undefined) {
                        $scope.dialogData.selectedProvince = $scope.dialogData.selectedCountry.provinces[0];
                    } else {
                        $scope.dialogData.selectedProvince = province;
                    }
                }
            }

            function save() {
                if($scope.editAddressForm.address1.$valid && $scope.editAddressForm.locality.$valid && $scope.editAddressForm.postalCode.$valid) {
                    if($scope.dialogData.selectedCountry.hasProvinces()) {
                        $scope.dialogData.customerAddress.region = $scope.dialogData.selectedProvince.code;
                    }
                    $scope.dialogData.customerAddress.countryCode = $scope.dialogData.selectedCountry.countryCode;
                    $scope.submit($scope.dialogData);
                }
            }

            function toTitleCase(str) {
                return str.charAt(0).toUpperCase() + str.slice(1);
            }

    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Customer.Dialogs.CustomerInfoEditController
     * @function
     *
     * @description
     * The controller for editing customer information
     */
    angular.module('merchello').controller('Merchello.Customer.Dialogs.CustomerInfoEditController',
        ['$scope',
        function($scope) {

            $scope.wasFormSubmitted = false;

            // exposed methods
            $scope.save = save;

            /**
             * @ngdoc method
             * @name submitIfValid
             * @function
             *
             * @description
             * Submit form if valid.
             */
            function save() {
                $scope.wasFormSubmitted = true;
                console.info($scope.dialogData);
                if ($scope.editInfoForm.email.$valid) {
                    $scope.submit($scope.dialogData);
                }
            }

        }]);

angular.module('merchello').controller('Merchello.Backoffice.MerchelloDashboardController',
    ['$scope', 'settingsResource',
    function($scope, settingsResource) {

        $scope.loaded = false;
        $scope.merchelloVersion = '';

        function init() {
            var promise = settingsResource.getMerchelloVersion();
            promise.then(function(version) {
                console.info(version);
              $scope.merchelloVersion = version.replace(/['"]+/g, '');
                $scope.loaded = true;
            });
        }

        // initialize the controller
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
    .controller('Merchello.GatewayProviders.Dialogs.CashPaymentMethodAuthorizeCapturePaymentController',
    ['$scope', function($scope) {

        function round(num, places) {
            return +(Math.round(num + "e+" + places) + "e-" + places);
        }

        $scope.dialogData.amount = round($scope.dialogData.invoiceBalance, 2)

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
            $scope.removeRateTier = removeRateTier;
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
    ['$scope', 'notificationsService', 'dialogService', 'settingsResource',
        'shippingGatewayProviderResource', 'shippingGatewayProviderDisplayBuilder', 'shipMethodDisplayBuilder',
        'shippingGatewayMethodDisplayBuilder', 'gatewayResourceDisplayBuilder', 'dialogDataFactory',
        function($scope, notificationsService, dialogService, settingsResource,
                 shippingGatewayProviderResource, shippingGatewayProviderDisplayBuilder, shipMethodDisplayBuilder,
                 shippingGatewayMethodDisplayBuilder, gatewayResourceDisplayBuilder, dialogDataFactory) {

            $scope.providersLoaded = false;
            $scope.allProviders = [];
            $scope.assignedProviders = [];
            $scope.availableProviders = [];
            $scope.currencySymbol = '';

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
                loadSettings();
                loadCountryProviders();
            }

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Loads the currency settings
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function(currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
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
                dialogData.currencySymbol = $scope.currencySymbol;
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
                dialogData.currencySymbol = $scope.currencySymbol;
                var editor = gatewayMethod.dialogEditorView.editorView;
                dialogService.open({
                    template: editor,
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
                    notificationsService.success(provider.name + " Method Activated");
                }, function (reason) {
                    notificationsService.error(provider.name + " Activate Failed", reason.message);
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
                    notificationsService.success(provider.name + " Deactivated");
                }, function (reason) {
                    notificationsService.error(provider.name + " Deactivate Failed", reason.message);
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
                $scope.preValuesLoaded = false;
                var promise = gatewayProviderResource.saveGatewayProvider(data.provider);
                promise.then(function (provider) {
                        notificationsService.success("Gateway Provider Saved", "");
                        init();
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
                notificationsService.success("Notification Message Saved");
                init();
            }, function (reason) {
                notificationsService.error("Notification Message Save Failed", reason.message);
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
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
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
 * @name Merchello.Backoffice.CampaignListController
 * @function
 *
 * @description
 * The controller for the marketing campaign list
 */
angular.module('merchello').controller('Merchello.Backoffice.CampaignListController',
    ['$scope', 'assetsService', 'notificationsService', 'dialogService',
    function($scope, assetsService, notificationsService, dialogService) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

}]);

    /**
     * @ngdoc controller
     * @name Merchello.Product.Dialogs.ProductVariantBulkChangePricesController
     * @function
     *
     * @description
     * The controller for the adding / editing Notification messages on the Notifications page
     */
    angular.module('merchello').controller('Merchello.Product.Dialogs.ProductVariantBulkChangePricesController',
        ['$scope',
        function($scope) {

    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Product.Dialogs.ProductVariantBulkChangePricesController
     * @function
     *
     * @description
     * The controller for the adding / editing Notification messages on the Notifications page
     */
    angular.module('merchello').controller('Merchello.Product.Dialogs.ProductVariantBulkUpdateInventoryController',
        ['$scope',
        function($scope) {

            function init() {
                console.info($scope.dialogData);
            }

            // Initialize the controller
            init();

        }]);

/**
 * @ngdoc controller
 * @name Merchello.Directives.ProductVariantsViewTableDirectiveController
 * @function
 *
 * @description
 * The controller for the product variant view table view directive
 */
angular.module('merchello').controller('Merchello.Directives.ProductVariantsViewTableDirectiveController',
    ['$scope', '$timeout', 'notificationsService', 'dialogService', 'dialogDataFactory', 'productResource', 'productDisplayBuilder', 'productVariantDisplayBuilder',
    function($scope, $timeout, notificationsService, dialogService, dialogDataFactory, productResource, productDisplayBuilder, productVariantDisplayBuilder) {

        $scope.sortProperty = "sku";
        $scope.sortOrder = "asc";
        $scope.bulkAction = true;
        $scope.allVariants = false;

        // exposed methods
        $scope.assertActiveShippingCatalog = assertActiveShippingCatalog;
        $scope.selectedVariants = selectedVariants;
        $scope.selectVariants = selectVariants;
        $scope.checkBulkVariantsSelected = checkBulkVariantsSelected;
        $scope.changeSortOrder = changeSortOrder;
        $scope.toggleAVariant = toggleAVariant;
        $scope.toggleAllVariants = toggleAllVariants;
        $scope.changePrices = changePrices;
        $scope.updateInventory = updateInventory;
        $scope.toggleOnSale = toggleOnSale;
        $scope.toggleAvailable = toggleAvailable;

        function init() {
            angular.forEach($scope.product.productVariants, function(pv) {
                pv.selected = false;
            });
        }

        /**
         * @ngdoc method
         * @name selectVariants
         * @function
         *
         * @description
         * Called by the ProductOptionAttributesSelection directive when an attribute is selected.
         * It will select the variants that have that option attribute and mark their selected property to true.
         * All other variants will have selected set to false.
         *
         */
        function selectVariants(attributeToSelect) {
            // Reset the selected state to false for all variants
            for (var i = 0; i < $scope.product.productVariants.length; i++) {
                $scope.product.productVariants[i].selected = false;
            }

            // Build a list of variants to select
            var filteredVariants = [];

            if (attributeToSelect == "All") {
                filteredVariants = $scope.product.productVariants;
            } else if (attributeToSelect == "None") {
            } else {
                filteredVariants = _.filter($scope.product.productVariants,
                    function (variant) {
                        return _.where(variant.attributes, { name: attributeToSelect }).length > 0;
                    });
            }

            // Set the selected state to true for all variants
            for (var v = 0; v < filteredVariants.length; v++) {
                filteredVariants[v].selected = true;
            }

            // Set the property to toggle in the bulk menu in the table header
            checkBulkVariantsSelected();
        }

        /**
         * @ngdoc method
         * @name selectedVariants
         * @function
         *
         * @description
         * This is a helper method to get a collection of variants that are selected.
         *
         */
        function selectedVariants() {
            if ($scope.product !== undefined) {
                return _.filter($scope.product.productVariants, function(v) {
                    return v.selected;
                });
            } else {
                return [];
            }
        }

        /**
         * @ngdoc method
         * @name checkBulkVariantsSelected
         * @function
         *
         * @description
         * This is a helper method to set the allVariants flag when one or more variants on selected
         * in the Variant Information table on the Variant tab.
         *
         */
        function checkBulkVariantsSelected() {
            var v = selectedVariants();
            if (v.length >= 1) {
                $scope.allVariants = true;
            } else {
                $scope.allVariants = false;
            }
        }

        /**
         * @ngdoc method
         * @name changeSortOrder
         * @function
         *
         * @description
         * Event handler for changing the rows sort by column when a header column item is clicked
         *
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
        }

        /**
         * @ngdoc method
         * @name toggleAVariant
         * @function
         *
         * @description
         * Event handler toggling the variant's selected state
         *
         */
        function toggleAVariant(variant) {
            variant.selected = !variant.selected;
            $scope.checkBulkVariantsSelected();
        }

        /**
         * @ngdoc method
         * @name toggleAllVariants
         * @function
         *
         * @description
         * Event handler toggling the all of the product variant's selected state
         *
         */
        function toggleAllVariants(newstate) {
            for (var i = 0; i < $scope.product.productVariants.length; i++) {
                $scope.product.productVariants[i].selected = newstate;
            }
            $scope.allVariants = newstate;
        }

        /**
         * @ngdoc method
         * @name toggleAvailable
         * @function
         *
         * @description
         * Toggles the variant available setting
         */
        function toggleAvailable() {
            var success = true;
            var selected = $scope.selectedVariants();
            for (var i = 0; i < selected.length; i++) {
                selected[i].available = !selected[i].available;
                var savepromise = productResource.saveVariant(selected[i]);
                savepromise.then(function () {
                    //notificationsService.success("Product Variant Saved", "");
                }, function (reason) {
                    success = false;
                    //notificationsService.error("Product Variant Save Failed", reason.message);
                });
            }
            if (success) {
                notificationsService.success("Confirmed available update", "");
                $timeout(function() {
                    reload();
                }, 400);
            } else {
                notificationsService.error("Failed to update available", "");
            }
        }

        //--------------------------------------------------------------------------------------
        // Dialog Event Handlers
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name changePrices
         * @function
         *
         * @description
         * Opens the dialog for setting the new price
         */
        function changePrices() {
            var dialogData = dialogDataFactory.createBulkVariantChangePricesDialogData();
            dialogData.productVariants = $scope.selectedVariants();
            dialogData.price = _.min(dialogData.productVariants, function(v) { return v.price;}).price;
            dialogData.salePrice = _.min(dialogData.productVariants, function(v) { return v.salePrice; }).price;
            dialogData.currencySymbol = $scope.currencySymbol;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/productvariant.bulk.changeprice.html',
                show: true,
                callback: changePricesDialogConfirm,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name updateInventory
         * @function
         *
         * @description
         * Opens the dialog for setting the new inventory
         */
        function updateInventory() {
            var dialogData = dialogDataFactory.createBulkEditInventoryCountsDialogData();
            dialogData.warning = 'Note: This will update the inventory for all warehouses on all selected variants';
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/productvariant.bulk.updateinventory.html',
                show: true,
                callback: updateInventoryDialogConfirm,
                dialogData: dialogData
            });
        }


        function toggleOnSale() {
            var success = true;
            var selected = $scope.selectedVariants();
            for (var i = 0; i < selected.length; i++) {
                selected[i].onSale = !selected[i].onSale;
                var savepromise = productResource.saveVariant(selected[i]);
                savepromise.then(function () {
                    //notificationsService.success("Product Variant Saved", "");
                }, function (reason) {
                    success = false;
                    //notificationsService.error("Product Variant Save Failed", reason.message);
                });
            }
            if (success) {
                notificationsService.success("Confirmed on sale update", "");
                $timeout(function() {
                    reload();
                }, 400);
            } else {
                notificationsService.error("Failed to update on sale setting", "");
            }
        }

        /**
         * @ngdoc method
         * @name updateInventoryDialogConfirm
         * @param {dialogData} contains the new inventory that is the price to adjust the variants price to.
         * @function
         *
         * @description
         * Handles the new inventory passed back from the dialog and sets the variants inventory and saves them.
         */
        function updateInventoryDialogConfirm(dialogData) {
            var success = true;
            var selected = $scope.selectedVariants();
            for (var i = 0; i < selected.length; i++) {
                selected[i].setAllInventoryCount(dialogData.count);
                if(dialogData.includeLowCount) {
                    selected[i].setAllInventoryLowCount(dialogData.lowCount);
                }
                var savepromise = productResource.saveVariant(selected[i]);
                savepromise.then(function () {
                    // don't reset success here
                }, function (reason) {
                    success = false;
                });
            }
            if (success) {
                notificationsService.success("Confirmed inventory update", "");
                $timeout(function() {
                    reload();
                }, 400);

            } else {
                notificationsService.error("Failed to update inventory", "");
            }
        }

        /**
         * @ngdoc method
         * @name changePricesDialogConfirm
         * @param {dialogData} contains the newPrice that is the price to adjust the variants price to.
         * @function
         *
         * @description
         * Handles the new price passed back from the dialog and sets the variants price and saves them.
         */
        function changePricesDialogConfirm(dialogData) {
            angular.forEach(dialogData.productVariants, function(pv) {
                pv.price = dialogData.price;
                if(dialogData.includeSalePrice) {
                    pv.salePrice = dialogData.salePrice;
                }
                productResource.saveVariant(pv);
            })
            notificationsService.success("Updated prices");
        }

        function assertActiveShippingCatalog() {
            $scope.assertCatalog();
        }

        function reload() {
            $scope.reload();
        }

        // initialize the controller
        init();
}]);


angular.module('merchello').controller('Merchello.Directives.ProductVariantShippingDirectiveController',
    ['$scope', 'notificationsService', 'dialogService', 'warehouseResource', 'warehouseDisplayBuilder', 'catalogInventoryDisplayBuilder',
        function($scope, notificationsService, dialogService, warehouseResource, warehouseDisplayBuilder, catalogInventoryDisplayBuilder) {

            $scope.warehouses = [];
            $scope.defaultWarehouse = {};
            $scope.defaultWarehouseCatalog = {};

            // exposed methods
            $scope.getUnits = getUnits;
            $scope.mapToCatalog = mapToCatalog;
            $scope.toggleCatalog = toggleCatalog;

            function init() {
                loadAllWarehouses();
            }

            /**
             * @ngdoc method
             * @name loadAllWarehouses
             * @function
             *
             * @description
             * Loads in default warehouse and all other warehouses from server into the scope.  Called in init().
             */
            function loadAllWarehouses() {
                var promiseWarehouse = warehouseResource.getDefaultWarehouse();
                promiseWarehouse.then(function (warehouse) {
                    $scope.defaultWarehouse = warehouseDisplayBuilder.transform(warehouse);
                    $scope.warehouses.push($scope.defaultWarehouse);
                    $scope.defaultWarehouseCatalog = _.find($scope.defaultWarehouse.warehouseCatalogs, function (dc) { return dc.isDefault; });
                }, function (reason) {
                    notificationsService.error("Default Warehouse Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name mapToCatalog
             * @function
             *
             * @description
             * Maps a catalog to a product variant catalog inventory
             */
            function mapToCatalog(catalog) {
                var mapped = _.find($scope.productVariant.catalogInventories, function(ci) { return ci.catalogKey === catalog.key;});
                if(mapped === undefined) {
                    var catalogInventory = catalogInventoryDisplayBuilder.createDefault();
                    catalogInventory.productVariantKey = $scope.productVariant.key;
                    catalogInventory.catalogKey = catalog.key;
                    catalogInventory.active = false;
                    $scope.productVariant.catalogInventories.push(catalogInventory);
                    mapped = catalogInventory;
                }
                return mapped;
            }

            function toggleCatalog() {
                $scope.productVariant.ensureCatalogInventory($scope.defaultWarehouseCatalog.key);
            }

            function getUnits(settings, type) {
                if(settings.unitSystem === 'Imperial') {
                    return type === 'weight' ? '(pounds)' : '(inches)';
                } else {
                    return type === 'weight' ? '(kg)' : '(cm)';
                }
            }

            // Initializes the controller
            init();
}]);

    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ProductEditController
     * @function
     *
     * @description
     * The controller for product edit view
     */
    angular.module('merchello').controller('Merchello.Backoffice.ProductEditController',
        ['$scope', '$routeParams', '$location', '$timeout', 'assetsService', 'notificationsService', 'dialogService', 'merchelloTabsFactory', 'dialogDataFactory',
            'serverValidationManager', 'productResource', 'warehouseResource', 'settingsResource',
            'productDisplayBuilder', 'productVariantDisplayBuilder', 'warehouseDisplayBuilder', 'settingDisplayBuilder', 'catalogInventoryDisplayBuilder',
        function($scope, $routeParams, $location, $timeout, assetsService, notificationsService, dialogService, merchelloTabsFactory, dialogDataFactory,
            serverValidationManager, productResource, warehouseResource, settingsResource,
            productDisplayBuilder, productVariantDisplayBuilder, warehouseDisplayBuilder, settingDisplayBuilder, catalogInventoryDisplayBuilder) {

            //--------------------------------------------------------------------------------------
            // Declare and initialize key scope properties
            //--------------------------------------------------------------------------------------
            //

            // To help umbraco directives show our page
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];

            // settings - contains defaults for the checkboxes
            $scope.settings = {};

            // this is for the slide panel directive to get rid of the close button since we'll
            // be handling it in a different way in this case
            $scope.hideClose = true;

            $scope.product = productDisplayBuilder.createDefault();
            $scope.productVariant = productVariantDisplayBuilder.createDefault();
            $scope.warehouses = [];
            $scope.defaultWarehouse = {};
            $scope.context = 'createproduct';

            // Exposed methods
            $scope.save = save;
            $scope.loadAllWarehouses = loadAllWarehouses;
            $scope.deleteProductDialog = deleteProductDialog;


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
                var key = $routeParams.id;
                var productVariantKey = $routeParams.variantid;
                loadSettings();
                loadAllWarehouses(key, productVariantKey);
            }

            /**
             * @ngdoc method
             * @name loadAllWarehouses
             * @function
             *
             * @description
             * Loads in default warehouse and all other warehouses from server into the scope.  Called in init().
             */
            function loadAllWarehouses(key, productVariantKey) {
                var promiseWarehouse = warehouseResource.getDefaultWarehouse();
                promiseWarehouse.then(function (warehouse) {
                    $scope.defaultWarehouse = warehouseDisplayBuilder.transform(warehouse);
                    $scope.warehouses.push($scope.defaultWarehouse);
                    loadProduct(key, productVariantKey);
                }, function (reason) {
                    notificationsService.error("Default Warehouse Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadProduct
             * @function
             *
             * @description
             * Load a product by the product key.
             */
            function loadProduct(key, productVariantKey) {
                if (key === 'create' || key === '' || key === undefined) {
                    $scope.context = 'createproduct';
                    $scope.productVariant = $scope.product.getMasterVariant();
                    $scope.tabs = merchelloTabsFactory.createNewProductEditorTabs();
                    $scope.tabs.setActive($scope.context);
                    $scope.preValuesLoaded = true;
                    return;
                }
                var promiseProduct = productResource.getByKey(key);
                promiseProduct.then(function (product) {
                    $scope.product = productDisplayBuilder.transform(product);
                    if(productVariantKey === '' || productVariantKey === undefined) {
                        // this is a product edit.
                        // we use the master variant context so that we can use directives associated with variants
                        $scope.productVariant = $scope.product.getMasterVariant();
                        $scope.context = 'productedit';
                        $scope.tabs = merchelloTabsFactory.createProductEditorTabs(key);
                    } else {
                        // this is a product variant edit
                        // in this case we need the specific variant
                        $scope.productVariant = $scope.product.getProductVariant(productVariantKey);
                        $scope.context = 'varianteditor';
                        $scope.tabs = merchelloTabsFactory.createProductVariantEditorTabs(key, productVariantKey);
                    }
                    $scope.preValuesLoaded = true;
                    $scope.tabs.setActive($scope.context);
                }, function (reason) {
                    notificationsService.error("Product Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Loads in store settings from server into the scope.  Called in init().
             */
            function loadSettings() {
                var promiseSettings = settingsResource.getAllSettings();
                promiseSettings.then(function(settings) {
                    $scope.settings = settingDisplayBuilder.transform(settings);
                    $scope.loaded = true;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            //--------------------------------------------------------------------------------------
            // Event Handlers
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Called when the Save button is pressed.  See comments below.
             */
            function save(thisForm) {
                // TODO we should unbind the return click event
                // so that we can quickly add the options and remove the following
                if(thisForm === undefined) {
                    return;
                }
                if (thisForm.$valid) {
                    $scope.preValuesLoaded = false;
                    switch ($scope.context) {
                        case "productedit":
                            // Copy from master variant
                            var productOptions = $scope.product.productOptions;
                            $scope.productVariant.ensureCatalogInventory();
                            $scope.product = $scope.productVariant.getProductForMasterVariant();
                            $scope.product.productOptions = productOptions;
                            saveProduct();
                            break;
                        case "varianteditor":
                            saveProductVariant();
                            break;
                        default:
                            var productOptions = $scope.product.productOptions;
                            $scope.product = $scope.productVariant.getProductForMasterVariant();
                            $scope.product.productOptions = productOptions;
                            createProduct();
                            break;
                    }
                }
            }

            /**
             * @ngdoc method
             * @name createProduct
             * @function
             *
             * @description
             * Creates a product.  See comments below.
             */
            function createProduct() {
                var promise = productResource.add($scope.product);
                promise.then(function (product) {
                    notificationsService.success("Product Saved", "");
                    $scope.product = productDisplayBuilder.transform(product);
                    // short pause to make sure examine index has a chance to update
                    $timeout(function() {
                        if ($scope.product.hasVariants()) {
                            $location.url("/merchello/merchello/producteditwithoptions/" + $scope.product.key, true);
                        } else {
                            $location.url("/merchello/merchello/productedit/" + $scope.product.key, true);
                        }
                    }, 400);
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Product Save Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name saveProduct
             * @function
             *
             * @description
             * Saves a product.  See comments below.
             */
            function saveProduct() {
                var promise = productResource.save($scope.product);
                promise.then(function (product) {
                    notificationsService.success("Product Saved", "");
                    $scope.product = productDisplayBuilder.transform(product);
                    $scope.productVariant = $scope.product.getMasterVariant();

                    if ($scope.product.hasVariants()) {
                        // short pause to make sure examine index has a chance to update
                        $timeout(function() {
                            $location.url("/merchello/merchello/producteditwithoptions/" + $scope.product.key, true);
                        }, 400);
                    }
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Product Save Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name saveProductVariant
             * @function
             *
             * @description
             * Saves a product variant.  See comments below.
             */
            function saveProductVariant() {
                //var variant = $scope.productVariant.deepClone();
                $scope.productVariant.removeInActiveInventories();
                var variantPromise = productResource.saveVariant($scope.productVariant);
                variantPromise.then(function(resultVariant) {
                    notificationsService.success("Product Variant Saved");
                    $scope.productVariant = productVariantDisplayBuilder.transform(resultVariant);
                    $scope.preValuesLoaded = true;
                }, function(reason) {
                    notificationsService.error("Product Variant Save Failed", reason.message);
                });
            }


            /**
             * @ngdoc method
             * @name deleteProductDialogConfirmation
             * @function
             *
             * @description
             * Called when the Delete Product button is pressed.
             */
            function deleteProductDialogConfirmation() {
                var promiseDel = productResource.deleteProduct($scope.product);
                promiseDel.then(function () {
                    notificationsService.success("Product Deleted", "");
                    $location.url("/merchello/merchello/productlist/manage", true);
                }, function (reason) {
                    notificationsService.error("Product Deletion Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deleteProductDialog
             * @function
             *
             * @description
             * Opens the delete confirmation dialog via the Umbraco dialogService.
             */
            function deleteProductDialog() {
                var dialogData = dialogDataFactory.createDeleteProductDialogData();
                dialogData.product = $scope.product;
                dialogData.name = $scope.product.name + ' (' + $scope.product.sku + ')';
                dialogData.warning = 'This action cannot be reversed.';

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: deleteProductDialogConfirmation,
                    dialogData: dialogData
                });
            }

            // Initialize the controller
            init();
    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ProductEditWithOptionsController
     * @function
     *
     * @description
     * The controller for product edit with options view
     */
    angular.module('merchello').controller('Merchello.Backoffice.ProductEditWithOptionsController',
        ['$scope', '$routeParams', '$location', '$q', 'assetsService', 'notificationsService', 'dialogService', 'serverValidationManager',
            'merchelloTabsFactory', 'dialogDataFactory', 'productResource', 'settingsResource', 'productDisplayBuilder',
        function($scope, $routeParams, $location, $q, assetsService, notificationsService, dialogService, serverValidationManager,
            merchelloTabsFactory, dialogDataFactory, productResource, settingsResource, productDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.product = productDisplayBuilder.createDefault();
            $scope.currencySymbol = '';
            $scope.reorderVariants = false;
            $scope.hideClose = true;

            // exposed methods
            $scope.save = save;
            $scope.deleteProductDialog = deleteProductDialog;
            $scope.init = init;

            function init() {
                var key = $routeParams.id;
                $scope.tabs = merchelloTabsFactory.createProductEditorWithOptionsTabs(key);
                $scope.tabs.setActive('variantlist');
                loadSettings();
                loadProduct(key);
            }

            /**
             * @ngdoc method
             * @name loadProduct
             * @function
             *
             * @description
             * Load a product by the product key.
             */
            function loadProduct(key) {
                var promise = productResource.getByKey(key);
                promise.then(function (product) {
                    $scope.product = productDisplayBuilder.transform(product);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Product Load Failed", reason.message);
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
            // Event Handlers
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Saves the product - used for changing the master variant name
             */
            function save(thisForm) {
                if (thisForm) {
                    if (thisForm.$valid) {
                        notificationsService.info("Saving Product...", "");
                        var promise = productResource.save($scope.product);
                        promise.then(function (product) {
                            notificationsService.success("Product Saved", "");
                            $scope.product = productDisplayBuilder.transform(product);
                        }, function (reason) {
                            notificationsService.error("Product Save Failed", reason.message);
                        });
                    }
                }
            }

            /**
             * @ngdoc method
             * @name deleteProductDialog
             * @function
             *
             * @description
             * Opens the delete confirmation dialog via the Umbraco dialogService.
             */
            function deleteProductDialog() {
                var dialogData = dialogDataFactory.createDeleteProductDialogData();
                dialogData.product = $scope.product;
                dialogData.name = $scope.product.name + ' (' + $scope.product.sku + ')';
                dialogData.warning = 'This action cannot be reversed.';

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: deleteProductDialogConfirmation,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name deleteProductDialogConfirmation
             * @function
             *
             * @description
             * Called when the Delete Product button is pressed.
             */
            function deleteProductDialogConfirmation() {
                var promiseDel = productResource.deleteProduct($scope.product);
                promiseDel.then(function () {
                    notificationsService.success("Product Deleted", "");
                    $location.url("/merchello/merchello/productlist/manage", true);
                }, function (reason) {
                    notificationsService.error("Product Deletion Failed", reason.message);
                });
            }

            // Initialize the controller
            init();
    }]);
    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ProductListController
     * @function
     *
     * @description
     * The controller for product list view controller
     */
    angular.module('merchello').controller('Merchello.Backoffice.ProductListController',
        ['$scope', '$routeParams', '$location', 'assetsService', 'notificationsService', 'settingsResource', 'merchelloTabsFactory', 'dialogDataFactory', 'productResource', 'productDisplayBuilder',
            'queryDisplayBuilder', 'queryResultDisplayBuilder',
        function($scope, $routeParams, $location, assetsService, notificationsService, settingsResource, merchelloTabsFactory, dialogDataFactory, productResource, productDisplayBuilder,
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
                $scope.currentFilters = query.parameters;

                var promise = productResource.searchProducts(query);
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
                //return Math.ceil($scope.products.length / $scope.limitAmount);
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
                return product.hasVariants() ? "#/merchello/merchello/producteditwithoptions/" + product.key :
                    "#/merchello/merchello/productedit/" + product.key;
            }

            // Initialize the controller
            init();

        }]);


    angular.module('merchello').controller('Merchello.Backoffice.ProductOptionsEditorController',
        ['$scope', '$routeParams', '$location', '$timeout', 'notificationsService', 'merchelloTabsFactory', 'productResource', 'settingsResource', 'productDisplayBuilder',
        function($scope, $routeParams, $location, $timeout, notificationsService, merchelloTabsFactory, productResource, settingsResource, productDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.product = productDisplayBuilder.createDefault();
            $scope.currencySymbol = '';

            // Exposed methods
            $scope.save = save;
            $scope.deleteProductDialog = deleteProductDialog;

            function init() {
                var key = $routeParams.id;
                $scope.tabs = merchelloTabsFactory.createProductEditorWithOptionsTabs(key);
                $scope.tabs.setActive('optionslist');
                loadSettings();
                loadProduct(key);
            }

            /**
             * @ngdoc method
             * @name loadProduct
             * @function
             *
             * @description
             * Load a product by the product key.
             */
            function loadProduct(key) {
                var promise = productResource.getByKey(key);
                promise.then(function (product) {
                    $scope.product = productDisplayBuilder.transform(product);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

                }, function (reason) {
                    notificationsService.error("Product Load Failed", reason.message);
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

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Saves the product - used for changing the master variant name
             */
            function save(thisForm) {
                // TODO we should unbind the return click event
                // so that we can quickly add the options and remove the following
                if(thisForm === undefined) {
                    return;
                }
                if (thisForm.$valid) {
                    notificationsService.info("Saving Product...", "");
                    console.info($scope.product);
                    var promise = productResource.save($scope.product);
                    promise.then(function (product) {
                        notificationsService.success("Product Saved", "");
                        $scope.product = productDisplayBuilder.transform(product);
                        if (!$scope.product.hasVariants()) {
                            // short pause to make sure examine index has a chance to update
                            $timeout(function() {
                                $location.url("/merchello/merchello/productedit/" + $scope.product.key, true);
                            }, 400);
                        }
                    }, function (reason) {
                        notificationsService.error("Product Save Failed", reason.message);
                    });
                }
            }

            /**
             * @ngdoc method
             * @name deleteProductDialog
             * @function
             *
             * @description
             * Opens the delete confirmation dialog via the Umbraco dialogService.
             */
            function deleteProductDialog() {
                var dialogData = dialogDataFactory.createDeleteProductDialogData();
                dialogData.product = $scope.product;
                dialogData.name = $scope.product.name + ' (' + $scope.product.sku + ')';
                dialogData.warning = 'This action cannot be reversed.';

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: deleteProductDialogConfirmation,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name deleteProductDialogConfirmation
             * @function
             *
             * @description
             * Called when the Delete Product button is pressed.
             */
            function deleteProductDialogConfirmation() {
                var promiseDel = productResource.deleteProduct($scope.product);
                promiseDel.then(function () {
                    notificationsService.success("Product Deleted", "");
                    $location.url("/merchello/merchello/productlist/manage", true);
                }, function (reason) {
                    notificationsService.error("Product Deletion Failed", reason.message);
                });
            }


            // Initializes the controller
            init();
    }]);

    /**
     * @ngdoc controller
     * @name Merchello.PropertyEditors.MerchelloProductSelectorController
     * @function
     *
     * @description
     * The controller for product product selector property editor view
     */
    angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloProductSelectorController',
        ['$scope', 'notificationsService', 'dialogService', 'assetsService', 'dialogDataFactory', 'productResource', 'settingsResource', 'productDisplayBuilder',
        function($scope, notificationsService, dialogService, assetsService, dialogDataFactory, productResource, settingsResource, productDisplayBuilder) {

            $scope.product = {};
            $scope.currencySymbol = '';
            $scope.loaded = false;

            // exposed methods
            $scope.selectProduct = selectProduct;
            $scope.disableProduct = disableProduct;

            //--------------------------------------------------------------------------------------
            // Initialization methods
            //--------------------------------------------------------------------------------------
            // Load the product from the Guid key stored in the model.value
            if (_.isString($scope.model.value)) {
                loadSettings();
                if ($scope.model.value.length > 0) {
                    loadProduct($scope.model.value);
                }
            }

            /**
             * @ngdoc method
             * @name loadProduct
             * @function
             *
             * @description
             * Load the product from the product service.
             */
            function loadProduct(key) {
                var promise = productResource.getByKey(key);
                promise.then(function (product) {
                    $scope.product = productDisplayBuilder.transform(product);
                    $scope.loaded = true;
                }, function (reason) {
                    notificationsService.error("Product Load Failed", reason.message);
                });
            }

            function loadSettings() {
                var promise = settingsResource.getCurrencySymbol();
                promise.then(function(symbol) {
                    $scope.currencySymbol = symbol;
                }, function (reason) {
                    notificationsService.error('Could not retrieve currency symbol', reason.message);
                });
            }

            //--------------------------------------------------------------------------------------
            // Event Handlers
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name selectedProductFromDialog
             * @function
             *
             * @description
             * Handles the model update after recieving the product to add from the dialog view/controller
             */
            function selectedProductFromDialog(dialogData) {
                $scope.model.value = dialogData.product.key;
                $scope.product = dialogData.product;
            }

            /**
             * @ngdoc method
             * @name selectProduct
             * @function
             *
             * @description
             * Opens the product select dialog via the Umbraco dialogService.
             */
            function selectProduct() {
                var dialogData = dialogDataFactory.createProductSelectorDialogData();
                dialogData.product = $scope.product;
                dialogService.open({
                    template: '/App_Plugins/Merchello/propertyeditors/productpicker/merchello.productselector.dialog.html',
                    show: true,
                    callback: selectedProductFromDialog,
                    dialogData: dialogData
                });
            }

            function disableProduct() {
                $scope.model.value = '';
                $scope.product = {};
            }

    }]);

    /**
     * @ngdoc controller
     * @name Merchello.PropertyEditors.MerchelloProductSelectorDialogController
     * @function
     *
     * @description
     * The controller for product selector property editor dialog view
     */
    angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloProductSelectorDialogController',
        ['$scope', 'notificationsService', 'productResource', 'settingsResource', 'productDisplayBuilder', 'queryDisplayBuilder', 'queryResultDisplayBuilder',
        function($scope, notificationsService, productResource, settingsResource, productDisplayBuilder, queryDisplayBuilder, queryResultDisplayBuilder) {

            $scope.filterText = "";
            $scope.products = [];
            $scope.filteredproducts = [];
            $scope.watchCount = 0;
            $scope.sortProperty = "name";
            $scope.sortOrder = "Ascending";
            $scope.limitAmount = 10;
            $scope.currentPage = 0;
            $scope.maxPages = 0;

            // exposed methods
            $scope.changePage = changePage;
            $scope.limitChanged = limitChanged;
            $scope.changeSortOrder = changeSortOrder;
            $scope.getFilteredProducts = getFilteredProducts;
            $scope.numberOfPages = numberOfPages;
            $scope.setProduct = setProduct;

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
                loadProducts();
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

            //--------------------------------------------------------------------------------------
            // Helper methods
            //--------------------------------------------------------------------------------------


            /**
             * @ngdoc method
             * @name selectProduct
             * @function
             *
             * @description
             * Helper to set the selected bool on a product if it is the currently selected product
             */
            function selectProduct(products) {
                for (var i = 0; i < products.length; i++) {
                    if (products[i].key == $scope.dialogData.product.key) {
                        products[i].selected = true;
                    } else {
                        products[i].selected = false;
                    }
                }
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

            function setProduct(product) {
                $scope.dialogData.product = product;
                selectProduct($scope.products);;
                $scope.submit($scope.dialogData);
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
                if (currency !== undefined) {
                    $scope.currencySymbol = currency.symbol;
                } else {
                    // this handles a legacy error where in some cases the invoice may not have saved the ISO currency code
                    // default currency
                    var defaultCurrencyPromise = settingsResource.getCurrencySymbol();
                    defaultCurrencyPromise.then(function (currencySymbol) {
                        $scope.currencySymbol = currencySymbol;
                    }, function (reason) {
                        notificationService.error('Failed to load the default currency symbol', reason.message);
                    });
                }
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
        ['$scope', '$routeParams', '$timeout', '$log', '$location', 'assetsService', 'dialogService', 'localizationService', 'notificationsService',
            'auditLogResource', 'invoiceResource', 'settingsResource', 'paymentResource', 'shipmentResource',
            'orderResource', 'dialogDataFactory', 'merchelloTabsFactory', 'addressDisplayBuilder', 'salesHistoryDisplayBuilder',
            'invoiceDisplayBuilder', 'paymentDisplayBuilder', 'paymentMethodDisplayBuilder', 'shipMethodsQueryDisplayBuilder',
        function($scope, $routeParams, $timeout, $log, $location, assetsService, dialogService, localizationService, notificationsService,
                 auditLogResource, invoiceResource, settingsResource, paymentResource, shipmentResource, orderResource, dialogDataFactory,
                 merchelloTabsFactory, addressDisplayBuilder, salesHistoryDisplayBuilder, invoiceDisplayBuilder, paymentDisplayBuilder, paymentMethodDisplayBuilder, shipMethodsQueryDisplayBuilder) {

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
            $scope.paymentMethods = {};
            $scope.payments = [];
            $scope.billingAddress = {};
            $scope.hasShippingAddress = false;
            $scope.authorizedCapturedLabel = '';
            $scope.shipmentLineItems = [];
            $scope.customLineItems = [];
            $scope.discountLineItems = [];

            // exposed methods
            //  dialogs
            $scope.capturePayment = capturePayment;
            $scope.showFulfill = true;
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
                // assert the collections are reset before populating
                $scope.shipmentLineItems = [];
                $scope.customLineItems = [];
                $scope.discountLineItems = [];

                var promise = invoiceResource.getByKey(id);
                promise.then(function (invoice) {
                    $scope.invoice = invoiceDisplayBuilder.transform(invoice);
                    $scope.billingAddress = $scope.invoice.getBillToAddress();
                    var taxLineItem = $scope.invoice.getTaxLineItem();
                    $scope.taxTotal = taxLineItem !== undefined ? taxLineItem.price : 0;
                    $scope.shippingTotal = $scope.invoice.shippingTotal();
                    loadSettings();
                    loadPayments(id);
                    loadAuditLog(id);
                    loadShippingAddress(id);

                    aggregateScopeLineItemCollection($scope.invoice.getCustomLineItems(), $scope.customLineItems);
                    aggregateScopeLineItemCollection($scope.invoice.getDiscountLineItems(), $scope.discountLineItems);

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
                    if (currency !== undefined) {
                    $scope.currencySymbol = currency.symbol;
                    } else {
                        // this handles a legacy error where in some cases the invoice may not have saved the ISO currency code
                        // default currency
                        var defaultCurrencyPromise = settingsResource.getCurrencySymbol();
                        defaultCurrencyPromise.then(function (currencySymbol) {
                            $scope.currencySymbol = currencySymbol;
                        }, function (reason) {
                            notificationService.error('Failed to load the default currency symbol', reason.message);
                        });
                    }
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
                var dialogData = dialogDataFactory.createCapturePaymentDialogData();
                dialogData.setPaymentData($scope.payments[0]);
                dialogData.setInvoiceData($scope.payments, $scope.invoice, $scope.currencySymbol);
                if (!dialogData.isValid()) {
                    return false;
                }
                var promise = paymentResource.getPaymentMethod(dialogData.paymentMethodKey);
                promise.then(function(paymentMethod) {
                    var pm = paymentMethodDisplayBuilder.transform(paymentMethod);
                    if (pm.authorizeCapturePaymentDialogEditorView !== '') {
                        dialogData.authorizeCapturePaymentEditorView = pm.authorizeCapturePaymentEditorView.editorView;
                    } else {
                        dialogData.authorizeCapturePaymentEditorView = '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.cashpaymentmethod.authorizecapturepayment.html';
                    }
                    dialogService.open({
                        template: dialogData.authorizeCapturePaymentEditorView,
                        show: true,
                        callback: capturePaymentDialogConfirm,
                        dialogData: dialogData
                    });
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
                console.info(paymentRequest);
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
                    $location.url("/merchello/merchello/saleslist/manage", true);
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
                      return (item.shipmentKey === '' || item.shipmentKey === null) && item.extendedData.getValue('merchShippable').toLowerCase() === 'true';
                    });
                    if(item !== null && item !== undefined) {
                        found = true;
                    } else {
                        i++;
                    }
                }

                return found;
            }

            // utility method to assist in building scope line item collections
            function aggregateScopeLineItemCollection(lineItems, collection) {
                if(angular.isArray(lineItems)) {
                    angular.forEach(lineItems, function(item) {
                        collection.push(item);
                    });
                } else {
                    collection.push(lineItems);
                }
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
        'invoiceResource', 'queryDisplayBuilder', 'queryResultDisplayBuilder', 'invoiceDisplayBuilder', 'settingDisplayBuilder',
        function($scope, $element, $log, angularHelper, assetsService, notificationService, merchelloTabsFactory, settingsResource, invoiceResource,
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