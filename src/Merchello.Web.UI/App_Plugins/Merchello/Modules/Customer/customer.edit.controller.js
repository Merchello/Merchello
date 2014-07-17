(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Customer.EditController
     * @function
     * 
     * @description
     * The controller for the customers edit page
     */
    controllers.CustomerEditController = function ($scope, $routeParams, $location, merchelloCustomerService, notificationsService) {

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
                var loadCustomerMethod = merchelloCustomerService.GetCustomer(customerKey);
                loadCustomerMethod.then(function(customerResponse) {
                    $scope.customer = new merchello.Models.Customer(customerResponse);
                    $scope.loaded = true;
                }, function(reason) {
                    notificationsService.error("Failed to load customer", reason.message);
                });
            }
        };


        $scope.loadRouteParams = function() {
            console.info($routeParams.create);
            if ($routeParams.create) {
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                $scope.customer = {};
                $(".content-column-body").css('background-image', 'none');
            } else {
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                $scope.customer = {};
                $(".content-column-body").css('background-image', 'none');
            }
        };


        $scope.saveCustomer = function () {

            notificationsService.info("Saving...", "");

            //we are editing so get the product from the server
            //var promise = merchelloProductService.save($scope.product);

            //promise.then(function (product) {

            //    notificationsService.success("Order Saved", "");

            //}, function (reason) {

            //    notificationsService.error("Order Save Failed", reason.message);

            //});
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

    }


    angular.module("umbraco").controller("Merchello.Editors.Customer.EditController", ['$scope', '$routeParams', '$location', 'merchelloCustomerService', 'notificationsService', merchello.Controllers.CustomerEditController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

