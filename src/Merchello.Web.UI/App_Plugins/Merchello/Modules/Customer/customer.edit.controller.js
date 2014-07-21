﻿(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Customer.EditController
     * @function
     * 
     * @description
     * The controller for the customers edit page
     */
    controllers.CustomerEditController = function($scope, $routeParams, $location, angularHelper, dialogService, merchelloCatalogShippingService, merchelloCustomerService, merchelloGravatarService, merchelloSettingsService, merchelloWarehouseService, notificationsService) {

        /**
         * @ngdoc method
         * @name buildAddressList
         * @function
         * 
         * @description
         * Add an address to the customer's list of addresses. 
         */
        $scope.addAddress = function(type) {
            // TODO: Add functionality to tag address with applicable billing or shipping flags.
            var newAddress;
            if (type === 'billing') {
                newAddress = $scope.currentBillingAddress;
            } else {
                newAddress = $scope.currentShippingAddress;
            }
            newAddress.customerKey = $scope.customer.key;
            newAddress.addressType = type;
            $scope.customer.addresses.push(newAddress);
            notificationsService.success("Address added to list. Customer must be saved to keep change", "");
        };

        /**
         * @ngdoc method
         * @name buildAddressList
         * @function
         * 
         * @description
         * Build a list of addresses and assign to either billing or shipping address lists depending on passed parameters. 
         */
        $scope.buildAddressList = function(type, addresses) {
            // TODO: Modify this function to seperate out only billing addresses once there exists typefieldkeys to match against
            var addressList = [];
            var defaultAddress = false, defaultAddressId = -1;
            if (addresses.length > 0) {
                for (var i = 0; i < addresses.length; i++) {
                    var newAddress = new merchello.Models.CustomerAddress(addresses[i]);
                    if (newAddress.isDefault) {
                        defaultAddress = newAddress;
                        defaultAddressId = addressList.length;
                    }
                    addressList.push(newAddress);
                }
                if (!defaultAddress) {
                    defaultAddress = addresses[0];
                }
                if (type === 'billing') {
                    $scope.currentBillingAddress = defaultAddress;
                    $scope.currentBillingAddressId = defaultAddressId;
                    $scope.billingAddresses = addressList;
                } else {
                    $scope.currentShippingAddress = defaultAddress;
                    $scope.shippingAddresses = addressList;
                    $scope.currentShippingAddressId = defaultAddressId;
                }
            }
        };

        /**
         * @ngdoc method
         * @name deleteAddress
         * @function
         * 
         * @description
         * Delete an address and updated the associated lists. 
         */
        $scope.deleteAddress = function (type) {
            // TODO: This won't work properly until the address typekeys, and keys are existing (at which point it will need alteration).
            if (type === 'billing') {
                if ($scope.currentBillingAddressId == -1) {
                    $scope.newAddress('billing');
                } else {
                    $scope.billingAddresses.splice($scope.currentBillingAddressId, 1);
                }
            } else {
                if ($scope.currentShippingAddressId == -1) {
                    $scope.newAddress('shipping');
                } else {
                    $scope.shippingAddresses.splice($scope.currentShippingAddressId, 1);
                }
            }
            notificationsService.info("Removing address.", "");
            notificationsService.info("Updating address lists. Customer must be saved to preserve address removal.", "");
            var updatedAddressList = $scope.prepareAddressesForSave();
            $scope.buildAddressList('billing', updatedAddressList);
            $scope.buildAddressList('shipping', updatedAddressList);

        }

        /**
         * @ngdoc method
         * @name doesCountryHaveProvinces
         * @function
         * 
         * @description
         * Returns true if the country provided has provinces. Otherwise returns false.
         */
        $scope.doesCountryHaveProvinces = function(country) {
            var result = false;
            if (country.provinces) {
                if (country.provinces.length > 0) {
                    result = true;
                }
            }
            return result;
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
            $scope.loadCustomer();
            $scope.loadCountries();
        };

        /**
         * @ngdoc method
         * @name isAvatarAvailable
         * @function
         * 
         * @description
         * Return true if Gravatar is returning a valid icon. Otherwise return false.
         */
        $scope.isAvatarAvailable = function() {
            var result = true;
            if ($scope.customer.email == '' || $scope.customerForm.email.$invalid) {
                result = false;
            }
            return result;
        };

        /**
         * @ngdoc method
         * @name isFormValid
         * @function
         * 
         * @description
         * Return true if the customer form is valid. Otherwise return false.
         */
        $scope.isFormValid = function() {
            var result = true;
            if ($scope.customerForm.firstName.$invalid || $scope.customerForm.lastName.$invalid || $scope.customerForm.email.$invalid) {
                result = false;
            }
            return result;
        }

        /**
         * @ngdoc method
         * @name loadCountries
         * @function
         * 
         * @description
         * Load a list of countries and provinces from the API. 
         */
        $scope.loadCountries = function() {
            $scope.countries = [];
            var promiseCountries = merchelloSettingsService.getAllCountries();
            promiseCountries.then(function(countriesResponse) {
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
                $scope.filters.billingCountry = $scope.countries[0];
                $scope.filters.ShippingCountry = $scope.countries[0];
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
            if ($routeParams.id === "new") {
                $scope.loaded = true;
            } else {
                var customerKey = $routeParams.id;
                var promiseLoadCustomer = merchelloCustomerService.GetCustomer(customerKey);
                promiseLoadCustomer.then(function(customerResponse) {
                    $scope.customer = new merchello.Models.Customer(customerResponse);
                    $scope.updateAvatarUrl();
                    $scope.buildAddressList('billing', customerResponse.addresses);
                    $scope.buildAddressList('shipping', customerResponse.addresses);
                    $scope.loaded = true;
                }, function(reason) {
                    notificationsService.error("Failed to load customer", reason.message);
                });
            }
        };

        /**
         * @ngdoc method
         * @name newAddress
         * @function
         * 
         * @description
         * Resets the edit panel for the selected address type for a new, blank address.
         */
        $scope.newAddress = function(type) {
            if (type === 'billing') {
                $scope.filters.billingCountry = $scope.countries[0];
                $scope.currentBillingAddress = new merchello.Models.CustomerAddress();
                $scope.currentBillingAddressId = -1;
            } else {
                $scope.filters.shippingProvince = $scope.countries[0];
                $scope.currentShippingAddress = new merchello.Models.CustomerAddress();
                $scope.currentBillingAddressId = -1;
            }
        };

        /**
        * @ngdoc method
        * @name openSelectAddressDialog
        * @function
        * 
        * @description
        * Opens the address selection dialog via the Umbraco dialogService.
        */
        $scope.openSelectAddressDialog = function (type) {
            var addresses, availableAddresses = [];
            if (type === 'billing') {
                addresses = $scope.billingAddresses;
            } else {
                addresses = $scope.shippingAddresses;
            }
            for (var i = 0; i < addresses.length; i++) {
                var addressToAdd = {
                    id: i,
                    name: addresses[i].address1 + " " + addresses[i].locality + ", " + addresses[i].region
                };
                availableAddresses.push(addressToAdd);
            }
            var myDialogData = {
                addressType: type,
                availableAddresses: availableAddresses,
                filter: availableAddresses[0]
            };
            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Customer/Dialogs/customer.selectaddress.html',
                show: true,
                callback: $scope.processSelectAddressDialog,
                dialogData: myDialogData
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
        $scope.prepareAddressesForSave = function() {
            // TODO: Currently all addresses have the same key, requiring much more thorough comparison between addresses to confirm uniqueness.
            var addresses = [], addressToAdd, addressToCompare, isDuplicate, i, j;
            for (i = 0; i < $scope.billingAddresses.length; i++) {
                addressToAdd = new merchello.Models.CustomerAddress($scope.billingAddresses[i]);
                addresses.push(addressToAdd);
            }
            // Because the customer keys are all the same, and there are no different type keys between shipping and billing addresses, logic must exist to double check that duplicates of each address are not being created.
            for (i = 0; i < $scope.shippingAddresses.length; i++) {
                addressToAdd = new merchello.Models.CustomerAddress($scope.shippingAddresses[i]);
                isDuplicate = false;
                // TODO: Get rid of this when the above to-do is accomplished. This is ridiculous. <-- Kyle Weems
                // AKA: I wrote it, but it's tacky and I hate it.
                for (j = 0; j < $scope.billingAddresses.length; j++) {
                    addressToCompare = $scope.billingAddresses[j];
                    if (addressToAdd.address1 == addressToCompare.address1) {
                        if (addressToAdd.address2 == addressToCompare.address2) {
                            if (addressToAdd.addressType == addressToCompare.addressType) {
                                if (addressToAdd.addressTypeFieldKey == addressToCompare.addressTypeFieldKey) {
                                    if (addressToAdd.company == addressToCompare.company) {
                                        if (addressToAdd.countryCode == addressToCompare.countryCode) {
                                            if (addressToAdd.customerKey == addressToCompare.customerKey) {
                                                if (addressToAdd.fullName == addressToCompare.fullName) {
                                                    if (addressToAdd.isDefault == addressToCompare.isDefault) {
                                                        if (addressToAdd.key == addressToCompare.key) {
                                                            if (addressToAdd.label == addressToCompare.label) {
                                                                if (addressToAdd.locality == addressToCompare.locality) {
                                                                    if (addressToAdd.phone == addressToCompare.phone) {
                                                                        if (addressToAdd.postalCode == addressToCompare.postalCode) {
                                                                            if (addressToAdd.region == addressToCompare.region) {
                                                                                isDuplicate = true;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (!isDuplicate) {
                    addresses.push(addressToAdd);
                }
            }
            return addresses;
        };

        /**
        * @ngdoc method
        * @name processSelectAddressDialog
        * @function
        * 
        * @description
        * Process the dialogData returned back from the address selection dialog via the Umbraco dialogService.
        */
        $scope.processSelectAddressDialog = function (dialogData) {
            var selected = dialogData.filter;
            if (dialogData.addressType === 'billing') {
                $scope.currentBillingAddress = $scope.billingAddresses[dialogData.filter.id];
                $scope.currentBillingAddressId = dialogData.filter.id;
            } else {
                $scope.currentShippingAddress = $scope.shippingAddresses[dialogData.filter.id];
                $scope.currentShippingAddressId = dialogData.filter.id;
            }

        };

        /**
         * @ngdoc method
         * @name saveCustomer
         * @function
         * 
         * @description
         * Save the customer.
         */
        $scope.saveCustomer = function () {
            $scope.wasFormSubmitted = true;
            if ($scope.isFormValid()) {
                notificationsService.info("Saving...", "");
                var promiseSaveCustomer;
                $scope.customer.loginName = $scope.customer.email;
                $scope.customer.addresses = $scope.prepareAddressesForSave();
                if ($routeParams.id === "new") {
                    promiseSaveCustomer = merchelloCustomerService.AddCustomer($scope.customer);
                } else {
                    promiseSaveCustomer = merchelloCustomerService.SaveCustomer($scope.customer);
                }
                promiseSaveCustomer.then(function(customerResponse) {
                    $scope.customer = new merchello.Models.Customer(customerResponse);
                    notificationsService.success("Customer Saved", "");
                    if ($routeParams.id === "") {
                        window.location.hash = "#/merchello/merchello/CustomerList/manage";
                    } else {
                        window.location.hash = "#/merchello/merchello/CustomerView/" + $routeParams.id;
                    }
                }, function(reason) {
                    notificationsService.error("Customer Save Failed", reason.message);
                });
            } else {
                notificationsService.error("Cannot save customer. Required fields are missing or improperly formatted", "");
            }
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
            $scope.currentBillingAddress = new merchello.Models.CustomerAddress();
            $scope.currentBillingAddressId = -1;
            $scope.currentShippingAddress = new merchello.Models.CustomerAddress();
            $scope.currentShippingAddressId = -1;
            $scope.customer = new merchello.Models.Customer();
            $scope.filters = {
                billingCountry: {},
                billingProvnce: {},
                shippingCountry: {},
                shippingProvince: {}
            };
            $scope.loaded = false;
            $scope.provinces = [];
            $scope.provinceLabel = "State/Province";
            $scope.shippingAddresses = [];
            $scope.wasFormSubmitted = false;
        };

        /**
         * @ngdoc method
         * @name sortCountries
         * @function
         * 
         * @description
         * Helper function that can be called by $scope.countries.sort($scope.sortCountries) to alphabetically sort countries.
         */
        $scope.sortCountries = function(a, b) {
            if (a.name < b.name) {
                return -1;
            }
            if (a.name > b.name) {
                return 1;
            }
            return 0;
        };

        /**
         * @ngdoc method
         * @name updateAddress
         * @function
         * 
         * @description
         * Update an existing address.
         */
        $scope.updateAddress = function (type) {
            if (type === 'billing') {
                $scope.billingAddresses[$scope.currentBillingAddressId] = $scope.currentBillingAddress;
            } else {
                $scope.shippingAddresses[$scope.currentShippingAddressId] = $scope.currentShippingAddress;
            }
            notificationsService.success("Address updated. Customer must be saved to keep change", "");
        };

        /**
         * @ngdoc method
         * @name updateAvatarUrl
         * @function
         * 
         * @description
         * Update the avatar URL.
         */
        $scope.updateAvatarUrl = function () {
            $scope.avatarUrl = merchelloGravatarService.avatarUrl($scope.customer.email);
        };

        /**
         * @ngdoc method
         * @name updateCountry
         * @function
         * 
         * @description
         * Update the selected country for the applicable address type, and prepare the provinces for selection.
         */
        $scope.updateCountry = function(type) {
            var selectedCountry;
            if (type === 'billing') {
                selectedCountry = $scope.filters.billingCountry;
                if (selectedCountry.id > -1) {
                    $scope.currentBillingAddress.countryCode = selectedCountry.countryCode;
                    if (selectedCountry.provinces.length > 0) {
                        $scope.currentBillingAddress.region = '';
                        if (selectedCountry.provinces[0].code !== '00') {
                            $scope.filters.billingCountry.provinces.unshift({ code: '00', name: 'Select State/Province' });
                        }
                        $scope.filters.billingProvince = $scope.filters.billingCountry.provinces[0];
                    }
                }
            } else {
                selectedCountry = $scope.filters.shippingCountry;
                if (selectedCountry.id > -1) {
                    $scope.currentShippingAddress.countryCode = selectedCountry.countryCode;
                    if (selectedCountry.provinces.length > 0) {
                        $scope.currentShippingAddress.region = '';
                        $scope.filters.shippingCountry.provinces.unshift({ code: '00', name: 'Select State/Province' });
                        $scope.filters.shippingProvince = $scope.filters.shippingCountry.provinces[0];
                    }
                }
            }
        }

        /**
         * @ngdoc method
         * @name updateProvince
         * @function
         * 
         * @description
         * Update the selected province for the applicable address type.
         */
        $scope.updateProvince = function (type) {
            var selectedProvince;
            if (type === 'billing') {
                selectedProvince = $scope.filters.billingProvince;
                if (selectedProvince.code !== '00') {
                    $scope.currentBillingAddress.region = selectedProvince.name;
                }
            } else {
                selectedProvince = $scope.filters.shippingProvince;
                if (selectedProvince.code !== '00') {
                    $scope.currentShippingAddress.region = selectedProvince.name;
                }
            }
        }

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Editors.Customer.EditController", ['$scope', '$routeParams', '$location', 'angularHelper', 'dialogService', 'merchelloCatalogShippingService', 'merchelloCustomerService', 'merchelloGravatarService', 'merchelloSettingsService', 'merchelloWarehouseService', 'notificationsService', merchello.Controllers.CustomerEditController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

