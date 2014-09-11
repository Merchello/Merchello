(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Customer.ListController
     * @function
     * 
     * @description
     * The controller for the customers list page
     */
    controllers.CustomerListController = function ($scope, dialogService, merchelloCustomerService, merchelloInvoiceService, notificationsService) {

        /**
         * @ngdoc method
         * @name changePage
         * @function
         * 
         * @description
         * Changes the current page.
         */
        $scope.changePage = function (page) {
            $scope.currentPage = page;
            $scope.loadCustomers($scope.filterText);
        };

        /**
         * @ngdoc method
         * @name changeSortOrder
         * @function
         * 
         * @description
         * Helper function to set the current sort on the table and switch the 
         * direction if the property is already the current sort column.
         */
        $scope.changeSortOrder = function (propertyToSort) {
            if ($scope.sortProperty == propertyToSort) {
                if ($scope.sortOrder == "asc") {
                    $scope.sortProperty = "-" + propertyToSort;
                    $scope.sortOrder = "desc";
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "asc";
                }
            } else {
                $scope.sortProperty = propertyToSort;
                $scope.sortOrder = "asc";
            }
            $scope.loadCustomers($scope.filterText);
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
            $scope.loadCustomers();
        };

        /**
         * @ngdoc method
         * @name limitChanged
         * @function
         * 
         * @description
         * Helper function to set the amount of items to show per page for the paging filters and calculations
         */
        $scope.limitChanged = function (newVal) {
            $scope.limitAmount = newVal;
            $scope.currentPage = 0;
            $scope.loadCustomers($scope.filterText);
        };

        /**
         * @ngdoc method
         * @name loadCustomers
         * @function
         * 
         * @description
         * Load the customers from the API using the provided filter (if any).
         */
        $scope.loadCustomers = function (filterText) {
            var page = $scope.currentPage;
            var perPage = $scope.limitAmount;
            var sortBy = $scope.sortInfo().sortBy;
            var sortDirection = $scope.sortInfo().sortDirection;
            if (filterText !== $scope.filterText) {
                page = 0;
                $scope.currentPage = 0;
            }
            $scope.filterText = filterText;
            var listQuery = new merchello.Models.ListQuery({
                currentPage: page,
                itemsPerPage: perPage,
                sortBy: sortBy,
                sortDirection: sortDirection,
                parameters: [
                {
                    fieldName: 'term',
                    value: filterText
                }]
            });
            var promiseAllCustomers = merchelloCustomerService.searchCustomers(listQuery);
            promiseAllCustomers.then(function (customersResponse) {
                if (customersResponse) {
                    $scope.customers = [];
                    var queryResult = new merchello.Models.QueryResult(customersResponse);
                    $scope.customers = _.map(queryResult.items, function (customer) {
                        return new merchello.Models.Customer(customer);
                    });
                    $scope.maxPages = queryResult.totalPages;
                    // TODO: comment out line below once the merchelloInvoiceService.getByCustomerKey API endpoint returns valid results.
                    // $scope.loadMostRecentOrders();
                }
            });
        };

        /**
        * @ngdoc method
        * @name loadMostRecentOrders
        * @function
        * 
        * @description
        * Iterate through all the customers in the list, and acquire their most recent order.
        */
        $scope.loadMostRecentOrders = function () {
            _.each($scope.customers, function (customer) {
                var promiseOrder = merchelloInvoiceService.getByCustomerKey(customer.key);
                promiseOrder.then(function (response) {
                    // TODO: Finish function acquiring the most recent order total for each customer once the merchelloInvoiceService.getByCustomerKey API endpoint returns valid results.
                });
            });
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
            $scope.currentPage = 0;
            $scope.customers = [];
            $scope.limitAmount = 100;
            $scope.loaded = true;
            $scope.maxPages = 0;
            $scope.preValuesLoaded = true;
            $scope.sortProperty = 'loginName';
            $scope.visible = {
                bulkActionButton: function() {
                    var result = false;
                    return result;
                },
                bulkActionDropdown: false
            };
        };

        /**
         * @ngdoc method
         * @name setVariables
         * @function
         * 
         * @description
         * Returns sort information based off the current $scope.sortProperty.
         */
        $scope.sortInfo = function () {
            var sortDirection, sortBy;
            // If the sortProperty starts with '-', it's representing a descending value.
            if ($scope.sortProperty.indexOf('-') > -1) {
                // Get the text after the '-' for sortBy
                sortBy = $scope.sortProperty.split('-')[1];
                sortDirection = 'Descending';
                // Otherwise it is ascending.
            } else {
                sortBy = $scope.sortProperty;
                sortDirection = 'Ascending';
            }
            return {
                sortBy: sortBy.toLowerCase(), // We'll want the sortBy all lower case for API purposes.
                sortDirection: sortDirection
            }
        };

        $scope.init();

    };


    angular.module("umbraco").controller("Merchello.Dashboards.Customer.ListController", ['$scope',  'dialogService', 'merchelloCustomerService', 'merchelloInvoiceService', 'notificationsService', merchello.Controllers.CustomerListController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
