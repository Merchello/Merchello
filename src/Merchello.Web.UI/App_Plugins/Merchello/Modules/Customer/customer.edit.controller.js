(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Customer.EditController
     * @function
     * 
     * @description
     * The controller for the customers edit page
     */
    controllers.CustomerEditController = function($scope, $routeParams, $location, merchelloCustomerService, merchelloSettingsService, notificationsService) {

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Inititalizes the scope.
         */
        $scope.init = function() {
            var promise = merchelloSettingsService.getTypeFields();
            promise.then(function (response) {
                console.info(response);
            }, function (reason){});
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
            if ($routeParams.id === "new") {
                $scope.loaded = true;
            } else {
                var customerKey = $routeParams.id;
                var promiseLoadCustomer = merchelloCustomerService.GetCustomer(customerKey);
                promiseLoadCustomer.then(function(customerResponse) {
                    $scope.customer = new merchello.Models.Customer(customerResponse);
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
        $scope.saveCustomer = function() {
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
            }, function(reason) {
                notificationsService.error("Customer Save Failed", reason.message);
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
        $scope.setVariables = function() {
            $scope.customer = new merchello.Models.Customer();
            $scope.loaded = false;
        };

        $scope.init();

    };


    angular.module("umbraco").controller("Merchello.Editors.Customer.EditController", ['$scope', '$routeParams', '$location', 'merchelloCustomerService', 'merchelloSettingsService', 'notificationsService', merchello.Controllers.CustomerEditController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

