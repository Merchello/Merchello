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
            $scope.shippingAddresses = [];
            if (addresses.length > 0) {
                _.each(addresses, function(address) {
                    if (address.addressType.toLowerCase() === 'billing') {
                        if (address.isDefault) {
                            $scope.defaultBillingAddress = new merchello.Models.CustomerAddress(address);
                            haveBillingDefault = true;
                        }
                        $scope.billingAddresses.push(new merchello.Models.CustomerAddress(address));
                    } else {
                        if (address.isDefault) {
                            $scope.defaultShippingAddress = new merchello.Models.CustomerAddress(address);
                            haveShippingDefault = true;
                        }
                        $scope.shippingAddresses.push(new merchello.Models.CustomerAddress(address));
                    }
                });
            }
            if (!haveBillingDefault && $scope.billingAddresses.length > 0) {
                $scope.defaultBillingAddress = $scope.billingAddresses[0];
            }
            if (!haveShippingDefault && $scope.shippingAddresses.length > 0) {
                $scope.defaultShippingAddress = $scope.shippingAddresses[0];
            }
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
        $scope.processEditAddressDialog = function (data) {
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
        };

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Editors.Customer.ViewController", ['$scope', '$routeParams', 'dialogService', 'merchelloCustomerService', 'merchelloGravatarService', 'merchelloInvoiceService', 'merchelloSettingsService', 'notificationsService', merchello.Controllers.CustomerViewController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
