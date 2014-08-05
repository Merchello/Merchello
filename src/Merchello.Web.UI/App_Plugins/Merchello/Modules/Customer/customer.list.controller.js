(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Customer.ListController
     * @function
     * 
     * @description
     * The controller for the customers list page
     */
    controllers.CustomerListController = function ($scope, dialogService, merchelloCustomerService, notificationsService) {

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
         * @name filterCustomers
         * @function
         * 
         * @description
         * Filter the customer list based on the provided filter.
         */
        $scope.filterCustomers = function () {

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
        * @name openNewCustomerDialog
        * @function
        * 
        * @description
        * Opens the new customer dialog via the Umbraco dialogService.
        */
        $scope.openNewCustomerDialog = function () {
            var dialogData = {
                firstName: '',
                lastName: '',
                email: ''
            };
            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Customer/Dialogs/customer.editinfo.html',
                show: true,
                callback: $scope.processNewCustomerDialog,
                dialogData: dialogData
            });
        };

        /**
         * @ngdoc method
         * @name processEditInfoDialog
         * @function
         * 
         * @description
         * Update the customer info and save. 
         */
        $scope.processNewCustomerDialog = function (data) {
            var newCustomer = new merchello.Models.Customer();
            newCustomer.firstName = data.firstName;
            newCustomer.lastName = data.lastName;
            newCustomer.email = data.email;
            newCustomer.loginName = data.email;
            var promiseSaveCustomer = merchelloCustomerService.AddCustomer(newCustomer);
            promiseSaveCustomer.then(function (customerResponse) {
                notificationsService.success("Customer Saved", "");
                $scope.init();
            }, function (reason) {
                notificationsService.error("Customer Save Failed", reason.message);
            });


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


    angular.module("umbraco").controller("Merchello.Dashboards.Customer.ListController", ['$scope',  'dialogService', 'merchelloCustomerService', 'notificationsService', merchello.Controllers.CustomerListController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
