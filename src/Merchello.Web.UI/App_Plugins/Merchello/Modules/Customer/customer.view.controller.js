(function (controllers, undefined) {
    
    /**
     * @ngdoc controller
     * @name Merchello.Editors.Customer.ViewController
     * @function
     * 
     * @description
     * The controller for the Customer view page
     */
    controllers.CustomerViewController = function($scope, $routeParams, dialogService, merchelloCustomerService, merchelloGravatarService, merchelloInvoiceService, merchelloSettingsService, notificationsService) {

        /**
         * @ngdoc method
         * @name getDefaultAddresses
         * @function
         * 
         * @description
         * Load the default addresses from the customer's list of addresses.
         */
        $scope.getDefaultAddresses = function() {
            var addresses = $scope.customer.addresses;
            $scope.billingAddresses = [];
            var haveBillingDefault = false;
            var haveShippingDefault = false;
            var i;
            var promiseTypeFields = merchelloSettingsService.getTypeFields();
            $scope.shippingAddresses = [];
            promiseTypeFields.then(function (typeFieldsResponse) {
                for (i = 0; i < typeFieldsResponse.length; i++) {
                    var typeField = typeFieldsResponse[i];
                    if (typeField.alias === "Billing" & $scope.billingKey === false) {
                        $scope.billingKey = typeField.typeKey;
                    }
                    if (typeField.alias == "Shipping" & $scope.shippingKey === false) {
                        $scope.shippingKey = typeField.typeKey;
                    }
                }
                if (addresses.length > 0) {
                    for (i = 0; i < addresses.length; i++) {
                        var newAddress = new merchello.Models.CustomerAddress(addresses[i]);
                        if (newAddress.addressTypeFieldKey == $scope.billingKey) {
                            if (newAddress.isDefault) {
                                $scope.defaultBillingAddress = newAddress;
                                haveBillingDefault = true;
                            }
                            $scope.billingAddresses.push(newAddress);
                        } else if (newAddress.addressTypeFieldKey == $scope.shippingKey) {
                            if (newAddress.isDefault) {
                                $scope.defaultShippingAddress = newAddress;
                                haveShippingDefault = true;
                            }
                            $scope.shippingAddresses.push(newAddress);
                        }
                    }
                }
                if (!haveBillingDefault && $scope.billingAddresses.length > 0) {
                    $scope.defaultBillingAddress = $scope.billingAddresses[0];
                }
                if (!haveShippingDefault && $scope.shippingAddresses.length > 0) {
                    $scope.defaultShippingAddress = $scope.shippingAddresses[0];
                }
            });
        };

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Inititalizes the scope.
         */
        $scope.init = function() {
            $scope.setVariables();
            $scope.loadCountries();
            $scope.loadCustomer();
        };

        /**
         * @ngdoc method
         * @name loadCountries
         * @function
         * 
         * @description
         * Load a list of countries and provinces from the API. 
         */
        $scope.loadCountries = function () {
            $scope.countries = [];
            var promiseCountries = merchelloSettingsService.getAllCountries();
            promiseCountries.then(function (countriesResponse) {
                for (var i = 0; i < countriesResponse.length; i++) {
                    var country = countriesResponse[i];
                    var newCountry = {
                        id: i,
                        countryCode: country.countryCode,
                        name: country.name,
                        provinces: country.provinces,
                        provinceLabel: country.provinceLabel
                    };
                    $scope.countries.push(newCountry);
                }
                $scope.countries.sort($scope.sortCountries);
                $scope.countries.unshift({ id: -1, name: 'Select Country', countryCode: '00', provinces: {}, provinceLabel: '' });
            });
        };

        /**
         * @ngdoc method
         * @name loadCustomer
         * @function
         * 
         * @description
         * Load the customer information if needed.
         */
        $scope.loadCustomer = function() {
            if ($routeParams.id !== "keygoeshere") {
                $scope.customerKey = $routeParams.id;
                var promiseLoadCustomer = merchelloCustomerService.GetCustomer($scope.customerKey);
                promiseLoadCustomer.then(function(customerResponse) {
                    $scope.customer = new merchello.Models.Customer(customerResponse);
                    $scope.avatarUrl = merchelloGravatarService.avatarUrl($scope.customer.email);
                    $scope.getDefaultAddresses();
                    $scope.loadInvoices();
                    $scope.loaded = true;
                }, function(reason) {
                    notificationsService.error("Failed to load customer", reason.message);
                });
            }
        };

        /**
         * @ngdoc method
         * @name loadCustomer
         * @function
         * 
         * @description
         * Load the invoices for the customer.
         */
        $scope.loadInvoices = function() {
            var promiseInvoices = merchelloInvoiceService.getByCustomerKey($scope.customer.key);
            promiseInvoices.then(function(invoicesResponse) {
                $scope.invoices = _.map(invoicesResponse.items, function (invoice) {
                    return new merchello.Models.Invoice(invoice);
                });
                for (var i = 0; i < invoicesResponse.length; i++) {
                    $scope.invoiceTotal += (invoicesResponse[i].total * 1);
                }

            });
        };

        /**
        * @ngdoc method
        * @name openAddressEditDialog
        * @function
        * 
        * @description
        * Opens the edit address dialog via the Umbraco dialogService.
        */
        $scope.openAddressEditDialog = function(type) {
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
        };

        /**
        * @ngdoc method
        * @name openDeleteCustomerDialog
        * @function
        * 
        * @description
        * Opens the delete customer dialog via the Umbraco dialogService.
        */
        $scope.openDeleteCustomerDialog = function () {
            var dialogData = {};
            dialogData.name = $scope.customer.firstName + ' ' + $scope.customer.lastName;
            dialogService.open({
                template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
                show: true,
                callback: $scope.processDeleteCustomerDialog,
                dialogData: dialogData
            });
        };

        /**
        * @ngdoc method
        * @name openEditInfoDialog
        * @function
        * 
        * @description
        * Opens the edit customer info dialog via the Umbraco dialogService.
        */
        $scope.openEditInfoDialog = function() {
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
        };

        /**
        * @ngdoc method
        * @name prepareAddressesForSave
        * @function
        * 
        * @description
        * Prepare a list of addresses to save with the customer
        */
        $scope.prepareAddressesForSave = function () {
            var addresses = [], addressToAdd, i;
            for (i = 0; i < $scope.billingAddresses.length; i++) {
                addressToAdd = new merchello.Models.CustomerAddress($scope.billingAddresses[i]);
                addresses.push(addressToAdd);
            };
            for (i = 0; i < $scope.shippingAddresses.length; i++) {
                addressToAdd = new merchello.Models.CustomerAddress($scope.shippingAddresses[i]);
                addresses.push(addressToAdd);
            };
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
        $scope.processEditAddressDialog = function (data) {
            var newAddress, i;
            if (data.shouldDelete) {
                if (data.addressType === 'billing') {
                    for (i = 0; i < $scope.billingAddresses.length; i++) {
                        if ($scope.billingAddresses[i].key == data.addressToReturn.key) {
                            $scope.billingAddresses.splice(i, 1);
                        }
                    }
                } else {
                    for (i = 0; i < $scope.shippingAddresses.length; i++) {
                        if ($scope.shippingAddresses[i].key == data.addressToReturn.key) {
                            $scope.shippingAddresses.splice(i, 1);
                        }
                    }
                }
            } else {
                if (data.filters.address.name != 'New Address') {
                    if (data.addressType === 'billing') {
                        $scope.billingAddresses[data.filters.address.id] = new merchello.Models.CustomerAddress(data.addressToReturn);
                    } else {
                        $scope.shippingAddresses[data.filters.address.id] = new merchello.Models.CustomerAddress(data.addressToReturn);
                    }
                    notificationsService.success("Address updated.", "");
                } else {
                    newAddress = new merchello.Models.CustomerAddress(data.addressToReturn);
                    newAddress.customerKey = $scope.customer.key;
                    newAddress.addressType = data.AddressType;
                    if (data.addressType === 'billing') {
                        newAddress.addressTypeFieldKey = $scope.billingKey;
                        $scope.billingAddresses.push(newAddress);
                    } else {
                        newAddress.addressTypeFieldKey = $scope.shippingKey;
                        $scope.shippingAddresses.push(newAddress);
                    }
                    notificationsService.success("Address added to list.", "");
                }
            }
            $scope.customer.addresses = $scope.prepareAddressesForSave();
            $scope.saveCustomer();
        };

        /**
         * @ngdoc method
         * @name processDeleteCustomerDialog
         * @function
         * 
         * @description
         * Delete a customer. 
         */
        $scope.processDeleteCustomerDialog = function (dialogData) {
            notificationsService.info("Deleting " + $scope.customer.firstName + " " + $scope.customer.lastName, "");
            var promiseDeleteCustomer = merchelloCustomerService.DeleteCustomer($scope.customer.key);
            promiseDeleteCustomer.then(function() {
                notificationsService.success("Customer deleted.", "");
                window.location.hash = "#/merchello/merchello/CustomerList/manage";
            }, function(reason) {
                notificationsService.error("Customer Deletion Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name processEditInfoDialog
         * @function
         * 
         * @description
         * Update the customer info and save. 
         */
        $scope.processEditInfoDialog = function (data) {
            $scope.customer.firstName = data.firstName;
            $scope.customer.lastName = data.lastName;
            $scope.customer.email = data.email;
            $scope.saveCustomer();
        };

        /**
         * @ngdoc method
         * @name saveCustoemr
         * @function
         * 
         * @description
         * Save the customer with the new note.
         */
        $scope.saveCustomer = function() {
            notificationsService.info("Saving...", "");
            var promiseSaveCustomer = merchelloCustomerService.SaveCustomer($scope.customer);
            promiseSaveCustomer.then(function(customerResponse) {
                $scope.customer = new merchello.Models.Customer(customerResponse);
                notificationsService.success("Customer Saved", "");
                $scope.getDefaultAddresses();
            }, function(reason) {
                notificationsService.error("Customer  Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name setVariables
         * @function
         * 
         * @description
         * Set the $scope variables.
         */
        $scope.setVariables = function () {
            $scope.avatarUrl = "";
            $scope.billingAddresses = [];
            $scope.billingKey = false;
            $scope.countries = [];
            $scope.customer = new merchello.Models.Customer();
            $scope.customerKey = false;
            $scope.currentAddress = {};
            $scope.defaultBillingAddress = false;
            $scope.defaultShippingAddress = false;
            $scope.filters = {
                province: {}
            };
            $scope.invoices = [];
            $scope.invoiceTotal = 0;
            $scope.loaded = false;
            $scope.shippingAddresses = [];
            $scope.shippingKey = false;
        };

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Editors.Customer.ViewController", ['$scope', '$routeParams', 'dialogService', 'merchelloCustomerService', 'merchelloGravatarService', 'merchelloInvoiceService', 'merchelloSettingsService', 'notificationsService', merchello.Controllers.CustomerViewController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
