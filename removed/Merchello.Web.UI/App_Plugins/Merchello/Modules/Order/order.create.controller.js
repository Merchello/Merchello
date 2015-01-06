(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Order.CreateController
     * @function
     * 
     * @description
     * The controller for the customers list page
     */                                                                                      
    controllers.OrderCreateController = function ($scope, $routeParams, $location, notificationsService, dialogService, merchelloOrderService, merchelloCustomerService, merchelloSettingsService) {

        $scope.selectedShippingMethod = {};
        $scope.chosenPaymentMethod = {};

        if ($routeParams.create) {
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.order = {};
            $(".content-column-body").css('background-image', 'none');
        }
        else {
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.order = {};
            $(".content-column-body").css('background-image', 'none');

            //we are editing so get the product from the server
            //var promise = merchelloProductService.getByKey($routeParams.id);

            //promise.then(function (product) {

            //    $scope.product = product;
            //    $scope.loaded = true;
            //    $scope.preValuesLoaded = true;
            //    $(".content-column-body").css('background-image', 'none');

            //}, function (reason) {

            //    alert('Failed: ' + reason.message);

            //});
        }
        
        /**
        * @ngdoc method
        * @name editCustomerInformation
        * @function
        * 
        * @description
        * Opens the add country dialog via the Umbraco dialogService.
        */
        $scope.editCustomerInformation = function () {
            var dialogData = {};
            $scope.customer = new merchello.Models.Customer();
            $scope.shippingAddress = new merchello.Models.CustomerAddress();
            $scope.billingAddress = new merchello.Models.CustomerAddress();

            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Order/Dialogs/selectcustomer.html',
                show: true,
                callback: $scope.editCustomerInformationConfirm,
                dialogData: dialogData
            });
        };

        /**
         * @ngdoc method
         * @name editCustomerInformationConfirm
         * @function
         * 
         * @description
         * Handles the save after recieving the country to add from the dialog view/controller
         */
        $scope.editCustomerInformationConfirm = function (selectedCustomer) {
            $scope.customer = selectedCustomer;
            $scope.shippingAddress = new merchello.Models.CustomerAddress();
            $scope.billingAddress = new merchello.Models.CustomerAddress();
            $scope.isShippingAddressSelected = false;
            $scope.isBillingAddressSelected = false;
            $scope.existingCustomer = true;
            $scope.customerSelected = true;
            notificationsService.info("Saved!", "");
        };


        /**
       * @ngdoc method
       * @name selectedProductFromDialog
       * @function
       * 
       * @description
       * Handles the model update after recieving the product to add from the dialog view/controller
       */
        $scope.selectedProductFromDialog = function (selectedProduct) {
            $scope.products.push(selectedProduct.key);
            selectedProduct.quantity = 1;
            $scope.invoice.items.push(new merchello.Models.InvoiceLineItem(selectedProduct));    
        };

        /**
         * @ngdoc method
         * @name selectProduct
         * @function
         * 
         * @description
         * Opens the product select dialog via the Umbraco dialogService.
         */
        $scope.selectProduct = function () {

            dialogService.open({
                template: '/App_Plugins/Merchello/PropertyEditors/ProductPicker/Views/merchelloproductdialog.html',
                show: true,
                callback: $scope.selectedProductFromDialog,
                dialogData: $scope.product
            });

        };

        /**
        * @ngdoc method
        * @name processesProductsToBackofficeOrder
        * @function
        * 
        * @description
        * Will add products to the sales preparation and will return the order summary.
        */
        $scope.processesProductsToBackofficeOrder = function (shipping, billing) {
            var promise = merchelloOrderService.processesProductsToBackofficeOrder($scope.customer.key, $scope.products, shipping, billing);
            promise.then(function (orderSummary) { 
                orderSummary.orderPrepComplete = true;
                $scope.orderSummary = new merchello.Models.OrderSummary(orderSummary);
                notificationsService.success("The order has been finalized.");
                                                                                       
                var shippingPromise = merchelloOrderService.getShippingMethods($scope.customer.key, $scope.products, shipping, billing);
                shippingPromise.then(function (shipMethods) {
                    $scope.shipMethods = shipMethods;
                }, function(reason) {

                });
                var paymentPromise = merchelloOrderService.getPaymentMethods();
                paymentPromise.then(function (paymentMethods) {
                    $scope.paymentMethods = paymentMethods;
                }, function (reason) {

                });
            }, function(reason) {
                notificationsService.error("Failed to add products to backoffice basket", reason.message);
            });
        };

        $scope.finalizeBackofficeOrder = function() {
            var promise = merchelloOrderService.finalizeBackofficeOrder($scope.customer.key, null, null, null, $scope.selectedPaymentMethod.paymentMethod.key, $scope.selectedPaymentMethod.paymentMethod.providerKey, $scope.selectedShippingMethod);
            promise.then(function(backofficeOrder) {
                
            }, function(reason) {

            });
        };

        /**
        * @ngdoc method
        * @name createCustomerDirective
        * @function
        * 
        * @description
        * Assigns values gathered from the ordercreate.directives.js on creating new customers.
        */
        $scope.createCustomerDirective = function (customer, shippingAddress, billingAddress) {
            $scope.init();

            customer.addresses = [];
            customer.addresses.push(shippingAddress);
            customer.addresses.push(billingAddress);

            $scope.customer = customer;                    
            $scope.shippingAddress = shippingAddress;
            $scope.billingAddress = billingAddress;

            $scope.createCustomer = false;
            $scope.customerSelected = true;
            $scope.isShippingAddressSelected = true;
            $scope.isBillingAddressSelected = true;
            $scope.existingCustomer = false;
        }

        $scope.paymentMethodChanged = function (method) {
            $scope.selectedPaymentMethod = method;
        }
        
        $scope.shipMethodChanged = function (method) {
            $scope.selectedShippingMethod = method;
        }
        /**
        * @ngdoc method
        * @name toggleCreateCustomer
        * @function
        * 
        * @description
        * Toggles the customer created flag.
        */
        $scope.toggleCreateCustomer = function() {   
            $scope.createCustomer = !$scope.createCustomer;
        }

        /**
        * @ngdoc method
        * @name shippingAddressSelected
        * @function
        * 
        * @description
        * Toggles the shipping address selected flag.
        */
        $scope.shippingAddressSelected = function () {
            $scope.isShippingAddressSelected = true;
        }

        /**
        * @ngdoc method
        * @name billingAddressSelected
        * @function
        * 
        * @description
        * Toggles the billing address selected flag.
        */
        $scope.billingAddressSelected = function () {
            $scope.isBillingAddressSelected = true;
        }

        /**
        * @ngdoc method
        * @name openAddressEditDialog
        * @function
        * 
        * @description
        * Opens the edit address dialog via the Umbraco dialogService.
        */
        $scope.createCustomerAddressFromDialog = function (type) {
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
                callback: $scope.createCustomerAddressFromDialogConfirm,
                dialogData: dialogData
            });
        };
        /**
        * @ngdoc method
        * @name processEditAddressDialog
        * @function
        * 
        * @description
        * Edit an address and update the associated lists. 
        */
        $scope.createCustomerAddressFromDialogConfirm = function (data) {
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
                
                    newAddress = new merchello.Models.CustomerAddress(data.addressToReturn);
                    newAddress.customerKey = $scope.customer.key;
                    newAddress.addressType = data.AddressType;
                    if (data.addressType === 'billing') {
                        newAddress.addressTypeFieldKey = $scope.billingKey;
                        $scope.billingAddresses.push(newAddress);
                        $scope.billingAddress = newAddress;
                    } else {
                        newAddress.addressTypeFieldKey = $scope.shippingKey;
                        $scope.shippingAddresses.push(newAddress);
                        $scope.shippingAddress = newAddress;
                    }
                    notificationsService.success("Address added to list.", "");     
            }
            $scope.customer.addresses = $scope.prepareAddressesForSave();
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
        * @name selectedProductFromDialog
        * @function
        * 
        * @description
        * Handles the model update after recieving the product to add from the dialog view/controller
        */
        $scope.createCustomerInformationFromDialogConfirm = function (dialogData) {
            $scope.customer.firstName = dialogData.firstName;
            $scope.customer.lastName = dialogData.lastName;
            $scope.customer.email = dialogData.email;
        };

        /**
         * @ngdoc method
         * @name selectProduct
         * @function
         * 
         * @description
         * Opens the product select dialog via the Umbraco dialogService.
         */
        $scope.createCustomerInformationFromDialog = function () {

            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Order/Dialogs/order.editcustomerinfo.html',
                show: true,
                callback: $scope.createCustomerInformationFromDialogConfirm,
                dialogData: $scope.customer
            });

        };

        /**
         * @ngdoc method
         * @name submitIfValid
         * @function
         * 
         * @description
         * Submit form if valid.
         */
        $scope.submitIfValid = function () {
            $scope.wasFormSubmitted = true;
            if ($scope.editInfoForm.firstName.$valid && $scope.editInfoForm.lastName.$valid && $scope.editInfoForm.email.$valid) {
                $scope.submit($scope.dialogData);
            }
        };

        /**
        * @ngdoc method
        * @name saveAddress
        * @function
        * 
        * @description
        * Acquire the address and close teh dialog, submitting collected data.
        */
        $scope.submitAddressIfValid = function () {
            if ($scope.editAddressInfo.fullName.$valid && $scope.editAddressForm.address1.$valid && $scope.editAddressForm.locality.$valid && $scope.editAddressForm.postalCode && $scope.dialogData.filters.country.id != -1) {
                $scope.dialogData.addressToReturn = $scope.currentAddress;
                $scope.submit($scope.dialogData);
            } else {
                $scope.wasFormSubmitted = true;
            }
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
        * @name save
        * @function
        * 
        * @description
        * Saves the customer.
        */
        $scope.save = function () {

            notificationsService.info("Saving...", "");

            //we are editing so get the product from the server
            //var promise = merchelloProductService.save($scope.product);

            //promise.then(function (product) {

            //    notificationsService.success("Order Saved", "");

            //}, function (reason) {

            //    notificationsService.error("Order Save Failed", reason.message);

            //});
        };

        $scope.init = function() {
            $scope.customer = new merchello.Models.Customer();
            $scope.shippingAddress = new merchello.Models.CustomerAddress();
            $scope.shippingAddresses = [];
            $scope.billingAddress = new merchello.Models.CustomerAddress();
            $scope.billingAddresses = [];
            $scope.customerSelected = false;
            $scope.createCustomer = false;
            $scope.product = new merchello.Models.Product();
            $scope.products = [];
            $scope.invoice = new merchello.Models.Invoice();
            $scope.existingCustomer = false;
            $scope.orderSummary = new merchello.Models.OrderSummary();
            $scope.isShippingAddressSelected = false;
            $scope.isBillingAddressSelected = false;
            $scope.createCustomer = false;
            $scope.shipMethods = null;
            $scope.paymentMethods = null;
            $scope.selectedShippingMethod = '00000000-0000-0000-0000-000000000000';
            $scope.selectedPaymentMethod = '';

            $scope.avatarUrl = "";
            $scope.countries = [];
            $scope.filters = {
                province: {}
            };

            $scope.loadCountries();
        };

        $scope.init();
    };

                                                                                  
    angular.module("umbraco").controller("Merchello.Editors.Order.CreateController", ['$scope', '$routeParams', '$location', 'notificationsService', 'dialogService', 'merchelloOrderService', 'merchelloCustomerService', 'merchelloSettingsService', merchello.Controllers.OrderCreateController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
