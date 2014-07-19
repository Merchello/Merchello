(function (controllers, undefined) {
    
    /**
     * @ngdoc controller
     * @name Merchello.Editors.Customer.ViewController
     * @function
     * 
     * @description
     * The controller for the Customer view page
     */
    controllers.CustomerViewController = function($scope, $routeParams, merchelloCustomerService, merchelloGravatarService, notificationsService) {

        /**
         * @ngdoc method
         * @name getDefaultAddress
         * @function
         * 
         * @description
         * Load the default address from the customer's list of addresses.
         */
        $scope.getDefaultAddress = function() {
            var addresses = $scope.customer.addresses;
            for (var i = 0; i < addresses.length; i++) {
                if (addresses[i].isDefault) {
                    $scope.defaultAddress = addresses[i];
                }
            }
        }

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
                    $scope.getDefaultAddress();
                    $scope.loaded = true;
                }, function(reason) {
                    notificationsService.error("Failed to load customer", reason.message);
                });
            }
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
            $scope.customer = new merchello.Models.Customer();
            $scope.customerKey = false;
            $scope.defaultAddress = {};
            $scope.loaded = false;
        };

        $scope.init();

    };


    angular.module("umbraco").controller("Merchello.Editors.Customer.ViewController", ['$scope', '$routeParams', 'merchelloCustomerService', 'merchelloGravatarService', 'notificationsService', merchello.Controllers.CustomerViewController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
