(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name ProductInventorySection
     * @function
     * 
     * @description
     * directive to set the inventory information on a product or product variant
     */
    directives.OrderCreateCustomer = function (merchelloCustomerService, merchelloSettingsService, notificationsService) {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Modules/Order/Directives/order-create-customer.html',
            scope: {
                'create': '&createCustomer', 
                'cancel': '&cancelCreateCustomer',
                customer: '=',
                shippingAddress: '=',
                billingAddress: '='
            },
            link: function ($scope, $element) {

                $scope.cancelCreateCustomer = function () {
                    $scope.cancel();
                }

                /**
                * @ngdoc method
                * @name saveCustomer
                * @function
                * 
                * @description
                * Save the customer.
                */
                $scope.saveCustomer = function (isAnonymous) {
                    $scope.shippingAddress = $scope.shippingAddressCreate;
                    $scope.billingAddress = $scope.billingAddressCreate;
                    $scope.customer = $scope.customerCreate;

                    $scope.wasFormSubmitted = true;
                    if ($scope.isFormValid(isAnonymous)) {
                        notificationsService.info("Saving...", "");
                        var promiseSaveCustomer;
                        $scope.customer.loginName = $scope.customer.email;

                        if ($scope.BillingSameAsShipping) {
                            $scope.billingAddress = $scope.shippingAddress;
                        }

                        $scope.customer.addresses.push($scope.shippingAddress);
                        $scope.customer.addresses.push($scope.billingAddress);

                        if (isAnonymous) {
                            promiseSaveCustomer = merchelloCustomerService.AddAnonymousCustomer($scope.customer);
                        }
                        else {
                            promiseSaveCustomer = merchelloCustomerService.AddCustomer($scope.customer);
                        }                                                                               

                        promiseSaveCustomer.then(function (customerResponse) {
                            if (isAnonymous) {
                                $scope.customer = customerResponse;
                            }
                            else {
                                $scope.customer = new merchello.Models.Customer(customerResponse);
                            }
                            notificationsService.success("Customer Saved", "");

                            $scope.create({ customer: $scope.customer, shipping: $scope.shippingAddress, billing: $scope.billingAddress });

                        }, function (reason) {
                            notificationsService.error("Customer Save Failed", reason.message);
                        });
                    } else {
                        notificationsService.error("Cannot save customer. Required fields are missing or improperly formatted", "");
                    }
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
                * @name doesCountryHaveProvinces
                * @function
                * 
                * @description
                * Returns true if the country provided has provinces. Otherwise returns false.
                */
                $scope.doesCountryHaveProvinces = function (country) {
                    var result = false;
                    
                    if (country != undefined && country.provinces) {
                        if (country.provinces.length > 0) {
                            result = true;
                        }
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
                $scope.isFormValid = function (isAnonymous) {
                    var result = true;
                    if (!isAnonymous && ($scope.customer.firstName == '' || $scope.customer.lastName == '' || $scope.customer.email == '')) {
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
                 * @name setVariables
                 * @function
                 * 
                 * @description
                 * Set the $scope variables.
                 */
                $scope.setVariables = function () {
                    $scope.customerCreate = new merchello.Models.Customer();
                    $scope.shippingAddressCreate = new merchello.Models.CustomerAddress();
                    $scope.billingAddressCreate = new merchello.Models.CustomerAddress();
                    $scope.BillingSameAsShipping = true;  
                    $scope.provinceLabel = "State/Province";
                    $scope.countries = [];
                    $scope.provinces = [];
                    $scope.filters = {
                        billingCountry: {},
                        billingProvnce: {},
                        shippingCountry: {},
                        shippingProvince: {}
                    };
   
                };

                /**
                 * @ngdoc method
                 * @name updateCountry
                 * @function
                 * 
                 * @description
                 * Update the selected country for the applicable address type, and prepare the provinces for selection.
                 */
                $scope.updateShippingCountry = function (selectedCountry) {
                    if (selectedCountry.id > -1) {
                        $scope.shippingAddress.countryCode = selectedCountry.countryCode;
                        if (selectedCountry.provinces.length > 0) {
                            $scope.provinces = _.map(selectedCountry.provinces, function (province) {
                                return province;
                            });
                            $scope.provinces.unshift({ code: '00', name: 'Select State/Province' });
                            $scope.filters.province = $scope.provinces[0];
                        }
                    } else {
                        $scope.provinces = [];
                    }
                };

                /**
                 * @ngdoc method
                 * @name updateCountry
                 * @function
                 * 
                 * @description
                 * Update the selected country for the applicable address type, and prepare the provinces for selection.
                 */
                $scope.updateBillingCountry = function (selectedCountry) {
                    if (selectedCountry.id > -1) {
                        $scope.billingAddress.countryCode = selectedCountry.countryCode;
                        if (selectedCountry.provinces.length > 0) {
                            $scope.provinces = _.map(selectedCountry.provinces, function (province) {
                                return province;
                            });
                            $scope.provinces.unshift({ code: '00', name: 'Select State/Province' });
                            $scope.filters.province = $scope.provinces[0];
                        }
                    } else {
                        $scope.provinces = [];
                    }
                };

                /**
                 * @ngdoc method
                 * @name updateProvince
                 * @function
                 * 
                 * @description
                 * Update the selected province for the applicable address type.
                 */
                $scope.updateShippingProvince = function (selectedProvince) {
                    if (selectedProvince.code !== '00') {
                        $scope.shippingAddress.region = selectedProvince.name;
                    }
                };

                /**
                 * @ngdoc method
                 * @name updateProvince
                 * @function
                 * 
                 * @description
                 * Update the selected province for the applicable address type.
                 */
                $scope.updateBillingProvince = function (selectedProvince) {
                    if (selectedProvince.code !== '00') {
                        $scope.billingAddress.region = selectedProvince.name;
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
                $scope.init = function () {
                    $scope.setVariables();
                    $scope.loadCountries();
                };

                $scope.init();
            }

        };
    };

    angular.module("umbraco").directive('orderCreateCustomer', merchello.Directives.OrderCreateCustomer);

}(window.merchello.Directives = window.merchello.Directives || {}));