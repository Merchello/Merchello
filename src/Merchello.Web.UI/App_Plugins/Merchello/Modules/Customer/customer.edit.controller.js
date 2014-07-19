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
         * @name buildBillingAddressList
         * @function
         * 
         * @description
         * Build a list of billing addresses. 
         */
        $scope.buildBillingAddressList = function (addresses) {
            // TODO: Modify this function to seperate out only billing addresses once there exists typefieldkeys to match against
            $scope.billingAddresses = [];
            var defaultAddress = false;
            if (addresses.length > 0) {
                for (var i = 0; i < addresses.length; i++) {
                    var newAddress = new merchello.Models.CustomerAddress(addresses[i]);
                    $scope.billingAddresses.push(newAddress);
                    if (newAddress.isDefault) {
                        defaultAddress = newAddress;
                    }
                }
                if (!defaultAddress) {
                    defaultAddress = addresses[0];
                }
                $scope.currentBillingAddress = defaultAddress;
            }
        };

        /**
         * @ngdoc method
         * @name buildCountryList
         * @function
         * 
         * @description
         * Build a list of countries. 
         */
        $scope.buildCountryList = function () {
            // TODO: Modify this someday to require less calls to do the same thing.
            $scope.countries = [{id: -1, countryCode: 00, name: 'Select Country'}];
            var promiseWarehouse = merchelloWarehouseService.getDefaultWarehouse();
            promiseWarehouse.then(function (warehouseResponse) {
                var catalogKey = warehouseResponse.warehouseCatalogs[0].key;
                if (catalogKey) {
                    var promiseCatalog = merchelloCatalogShippingService.getWarehouseCatalogShippingCountries(catalogKey);
                    promiseCatalog.then(function (promiseCountries) {
                        for (var i = 0; i < promiseCountries.length; i++) {
                            var newCountry = {
                                id: i,
                                countryCode: promiseCountries[i].countryCode,
                                name: promiseCountries[i].name
                            };
                            $scope.countries.push(newCountry);
                        }
                        $scope.filters.country = $scope.countries[0];
                    });
                }
            });
        }

        /**
         * @ngdoc method
         * @name buildBillingAddressList
         * @function
         * 
         * @description
         * Build a list of shipping addresses. 
         */
        $scope.buildShippingAddressList = function (addresses) {
            // TODO: Modify this function to seperate out only shipping addresses once there exists typefieldkeys to match against
            $scope.shippingAddresses = [];
            var defaultAddress = false;
            if (addresses.length > 0) {
                for (var i = 0; i < $scope.addresses.length; i++) {
                    var newAddress = new merchello.Models.CustomerAddress(addresses[i]);
                    $scope.shippingAddresses.push(newAddress);
                    if (newAddress.isDefault) {
                        defaultAddress = newAddress;
                    }
                }
                if (!defaultAddress) {
                    defaultAddress = addresses[0];
                }
                $scope.currentShippingAddress = defaultAddress;
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
            $scope.loadCustomer();
            $scope.buildCountryList();
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
                    $scope.buildBillingAddressList(customerResponse.addresses);
                    $scope.buildShippingAddressList(customerResponse.addresses);
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
                country: {}
            };
            $scope.loaded = false;
            $scope.provinces = [];
            $scope.shippingAddresses = [];
            $scope.wasFormSubmitted = false;
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

        $scope.init();

    };


    angular.module("umbraco").controller("Merchello.Editors.Customer.EditController", ['$scope', '$routeParams', '$location', 'merchelloCatalogShippingService', 'merchelloCustomerService', 'merchelloGravatarService', 'merchelloSettingsService', 'merchelloWarehouseService', 'notificationsService', merchello.Controllers.CustomerEditController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

