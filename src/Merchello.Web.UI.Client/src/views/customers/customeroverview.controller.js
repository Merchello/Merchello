    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.CustomerOverviewController
     * @function
     *
     * @description
     * The controller for customer overview view
     */
    angular.module('merchello').controller('Merchello.Backoffice.CustomerOverviewController',
        ['$scope', '$routeParams', 'dialogService', 'notificationsService', 'gravatarService', 'settingsResource', 'invoiceHelper', 'merchelloTabsFactory', 'dialogDataFactory',
            'customerResource', 'customerDisplayBuilder', 'countryDisplayBuilder', 'currencyDisplayBuilder', 'settingDisplayBuilder',
        function($scope, $routeParams, dialogService, notificationsService, gravatarService, settingsResource, invoiceHelper, merchelloTabsFactory, dialogDataFactory,
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
            $scope.openEditInfoDialog = openEditInfoDialog;
            $scope.openDeleteCustomerDialog = openDeleteCustomerDialog;

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
                    $scope.avatarUrl = gravatarService.getAvatarUrl($scope.customer.email);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
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
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/customer.info.edit.html',
                    show: true,
                    callback: processEditInfoDialog,
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
                    notificationsService.success("Customer Saved", "");
                    init();

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
