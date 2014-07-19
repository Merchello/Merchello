(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Customer.EditController
     * @function
     * 
     * @description
     * The controller for the customers edit page
     */
    controllers.CustomerEditController = function ($scope, $routeParams, $location, merchelloCatalogShippingService, merchelloCustomerService, merchelloGravatarService, merchelloSettingsService, merchelloWarehouseService, notificationsService) {

        /**
         * @ngdoc method
         * @name buildAddressList
         * @function
         * 
         * @description
         * Add an address to the customer's list of addresses. 
         */
        $scope.addAddress = function (type) {
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
        $scope.buildAddressList = function (type, addresses) {
            // TODO: Modify this function to seperate out only billing addresses once there exists typefieldkeys to match against
            var addressList = [];
            var defaultAddress = false;
            if (addresses.length > 0) {
                for (var i = 0; i < addresses.length; i++) {
                    var newAddress = new merchello.Models.CustomerAddress(addresses[i]);
                    addressList.push(newAddress);
                    if (newAddress.isDefault) {
                        defaultAddress = newAddress;
                    }
                }
                if (!defaultAddress) {
                    defaultAddress = addresses[0];
                }
                if (type === 'billing') {
                    $scope.currentBillingAddress = defaultAddress;
                    $scope.billingAddresses = addressList;
                } else {
                    $scope.currentShippingAddress = defaultAddress;
                    $scope.shippingAddresses = addressList;
                }
            }
        };

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
        $scope.init = function () {
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
            $scope.currentShippingAddress = new merchello.Models.CustomerAddress();
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


    angular.module("umbraco").controller("Merchello.Editors.Customer.EditController", ['$scope', '$routeParams', '$location', 'merchelloCatalogShippingService', 'merchelloCustomerService', 'merchelloGravatarService', 'merchelloSettingsService', 'merchelloWarehouseService', 'notificationsService', merchello.Controllers.CustomerEditController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

