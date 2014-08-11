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
                shippingPromise.then(function(shipMethods) {
                    $scope.shipMethods = shipMethods;
                }, function(reason) {

                });
            }, function(reason) {
                notificationsService.error("Failed to add products to backoffice basket", reason.message);
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
            $scope.billingAddress = new merchello.Models.CustomerAddress();
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
        };

        $scope.init();
    };

                                                                                  
    angular.module("umbraco").controller("Merchello.Editors.Order.CreateController", ['$scope', '$routeParams', '$location', 'notificationsService', 'dialogService', 'merchelloOrderService', 'merchelloCustomerService', 'merchelloSettingsService', merchello.Controllers.OrderCreateController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
