(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Customer.ListController
     * @function
     * 
     * @description
     * The controller for the customers list page
     */
    controllers.CustomerListController = function ($scope, merchelloCustomerService, notificationsService) {

        /**
         * @ngdoc method
         * @name getAllCustomers
         * @function
         * 
         * @description
         * Load all the customers from the API.
         */
        $scope.getAllCustomers = function() {
            var promiseAllCustomers = merchelloCustomerService.GetAllCustomers();
            promiseAllCustomers.then(function (customersResponse) {
                if (customersResponse) {
                    $scope.customers = [];
                    $scope.customers = _.map(customersResponse, function (customer) {
                        return new merchello.Models.Customer(customer);
                    });
                }
            });
        };

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * initialized when the scope loads.
         */
        $scope.init = function () {
            $scope.setVariables();
            $scope.getAllCustomers();
        };

        /**
         * @ngdoc method
         * @name setVariables
         * @function
         * 
         * @description
         * Sets $scope variables.
         */
        $scope.setVariables = function () {
            $scope.customers = [];
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.visible = {
                bulkActionButton: function() {
                    var result = false;
                    return result;
                },
                bulkActionDropdown: false
            };
        };

        $scope.init();

    };


    angular.module("umbraco").controller("Merchello.Dashboards.Customer.ListController", ['$scope',  'merchelloCustomerService', 'notificationsService', merchello.Controllers.CustomerListController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
