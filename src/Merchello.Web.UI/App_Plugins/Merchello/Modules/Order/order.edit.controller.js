(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Order.EditController
     * @function
     * 
     * @description
     * The controller for the customers list page
     */
    controllers.OrderEditController = function ($scope, $routeParams, $location, notificationsService, dialogService, merchelloCustomerService, merchelloSettingsService) {
        $scope.customer = {};
        $scope.shippingAddress = {};
        $scope.billingAddress = {};
        $scope.customerSelected = false;
        $scope.createCustomer = false;

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
        * @name addCountry
        * @function
        * 
        * @description
        * Opens the add country dialog via the Umbraco dialogService.
        */
        $scope.editCustomerInformation = function () {
            var dialogData = {};

            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Order/Dialogs/selectcustomer.html',
                show: true,
                callback: $scope.editCustomerInformationConfirm,
                dialogData: dialogData
            });
        };

        /**
         * @ngdoc method
         * @name addCountryDialogConfirm
         * @function
         * 
         * @description
         * Handles the save after recieving the country to add from the dialog view/controller
         */
        $scope.editCustomerInformationConfirm = function (customer) {
            $scope.customer = new merchello.Models.Customer(customer);
            $scope.customerSelected = true;
            notificationsService.info("Saved!", "");
        };

        $scope.createCustomerDirective = function (newCustomer, newBillingAddress, newShippingAddress) {
            $scope.toggleCreateCustomer();
            $scope.customerSelected = true;
            $scope.shippingAddressSelected();
            $scope.billingAddressSelected();
        }

        $scope.toggleCreateCustomer = function() {   
            $scope.createCustomer = !$scope.createCustomer;
        }
        $scope.shippingAddressSelected = function () {
            $scope.shippingAddressSelected = true;
        }

        $scope.billingAddressSelected = function () {
            $scope.billingAddressSelected = true;
        }

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
    };

                                                                                  
    angular.module("umbraco").controller("Merchello.Editors.Order.EditController", ['$scope', '$routeParams', '$location', 'notificationsService', 'dialogService', 'merchelloCustomerService', 'merchelloSettingsService', merchello.Controllers.OrderEditController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
