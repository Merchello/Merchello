(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name ProductInventorySection
     * @function
     * 
     * @description
     * directive to set the inventory information on a product or product variant
     */
    directives.OrderCreateCustomer = function (dialogService, merchelloCustomerService, merchelloSettingsService, notificationsService) {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Modules/Order/Directives/order-create-customer.html',
            scope: {
                'create': '&createCustomer', 
                'cancel': '&cancelCreateCustomer',
                'editCustomer': '&editCustomerInformation',
                'editAddressInfo': '&editCustomerAddressInformation',
                customer: '=',
                shippingAddress: '=',
                billingAddress: '='
            },
            link: function ($scope, $element) {

                $scope.cancelCreateCustomer = function () {
                    $scope.cancel();
                }                            
                $scope.editCustomerInformation = function() {
                    $scope.editCustomer();
                }
                $scope.createCustomerAddressFromDialog = function (type) {
                    $scope.editAddressInfo({ type: type });
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
                    notificationsService.info("Saving...", "");
                    var promiseSaveCustomer;

                    $scope.customer.loginName = $scope.customer.email;

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
                        notificationsService.error("Customer  Failed", reason.message);
                    });
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