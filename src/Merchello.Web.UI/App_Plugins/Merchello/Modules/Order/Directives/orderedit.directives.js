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
                customer: '=',
                shippingAddress: '=',
                billingAddress: '=',
                'create': '&createCustomerDirective'
            },
            link: function ($scope, $element) {

                $scope.createAnonymousCustomer = function () {
                    if ($scope.BillingSameAsShipping) {
                        $scope.billingAddress = $scope.shippingAddress;
                    }                                                

                    $scope.customer.addresses.push($scope.shippingAddress);
                    $scope.customer.addresses.push($scope.billingAddress);

                    $scope.create();
                }

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

                        if ($scope.BillingSameAsShipping) {
                            $scope.billingAddress = $scope.shippingAddress;
                        }

                        $scope.customer.addresses.push($scope.shippingAddress);
                        $scope.customer.addresses.push($scope.billingAddress);

                        promiseSaveCustomer = merchelloCustomerService.AddCustomer($scope.customer);

                        promiseSaveCustomer.then(function (customerResponse) {
                            $scope.customer = new merchello.Models.Customer(customerResponse);
                            notificationsService.success("Customer Saved", "");
                            $scope.create();

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
                $scope.isFormValid = function () {
                    var result = true;
                    if ($scope.customer.firstName == '' || $scope.customer.lastName == '' || $scope.customer.email == '') {
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
                    $scope.customer = new merchello.Models.Customer();
                    $scope.shippingAddress = new merchello.Models.CustomerAddress();
                    $scope.billingAddress = new merchello.Models.CustomerAddress();
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