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
            var billingAddresses = [];
            var haveBillingDefault = false;
            var haveShippingDefault = false;
            var i;
            var promiseTypeFields = merchelloSettingsService.getTypeFields();
            var shippingAddresses = [];
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
                            billingAddresses.push(newAddress);
                        } else if (newAddress.addressTypeFieldKey == $scope.shippingKey) {
                            if (newAddress.isDefault) {
                                $scope.defaultShippingAddress = newAddress;
                                haveShippingDefault = true;
                            }
                            shippingAddresses.push(newAddress);
                        }
                    }
                }
                if (!haveBillingDefault && billingAddresses.length > 0) {
                    $scope.defaultBillingAddress = billingAddresses[0];
                }
                if (!haveShippingDefault && shippingAddresses.length > 0) {
                    $scope.defaultShippingAddress = shippingAddresses[0];
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
            $scope.loadCustomer();
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
                $scope.invoices = _.map(invoicesResponse, function (invoice) {
                    return new merchello.Models.Invoice(invoice);
                });
                for (var i = 0; i < invoicesResponse.length; i++) {
                    $scope.invoiceTotal += (invoicesResponse[i].total * 1);
                }

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
         * @name processDeleteCustomerDialog
         * @function
         * 
         * @description
         * Delete an address and update the associated lists. 
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
                notificationsService.success("Customer Note Saved", "");
            }, function(reason) {
                notificationsService.error("Customer Note Save Failed", reason.message);
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
            $scope.billingKey = false;
            $scope.customer = new merchello.Models.Customer();
            $scope.customerKey = false;
            $scope.defaultBillingAddress = false;
            $scope.defaultShippingAddress = false;
            $scope.invoices = [];
            $scope.invoiceTotal = 0;
            $scope.loaded = false;
            $scope.shippingKey = false;
        };

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Editors.Customer.ViewController", ['$scope', '$routeParams', 'dialogService', 'merchelloCustomerService', 'merchelloGravatarService', 'merchelloInvoiceService', 'merchelloSettingsService', 'notificationsService', merchello.Controllers.CustomerViewController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
