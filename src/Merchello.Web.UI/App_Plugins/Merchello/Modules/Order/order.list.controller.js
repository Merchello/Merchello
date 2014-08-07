(function (controllers, undefined) {
    
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Order.ListController
     * @function
     * 
     * @description
     * The controller for the orders list page
     */
    controllers.OrderListController = function ($scope, assetsService, notificationsService, merchelloInvoiceService, merchelloSettingsService) {

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
            $scope.loadAllInvoices();
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
        };

        /**
         * @ngdoc method
         * @name getFilteredInvoices
         * @function
         * 
         * @description
         * Calls the invoice service to search for invoices via a string search 
         * param.  This searches the Examine index in the core.
         */
        $scope.getFilteredInvoices = function (filter) {
            notificationsService.info("Filtering...", "");
            if (merchello.Helpers.Strings.isNullOrEmpty(filter)) {
                $scope.loadAllInvoices();
            } else {
                var promise = merchelloInvoiceService.getFiltered(filter);
                promise.then(function (response) {
                    var queryResult = new merchello.Models.QueryResult(response);
                    $scope.invoices = _.map(queryResult.results, function (invoice) {
                        return new merchello.Models.Invoice(invoice);
                    });
                    notificationsService.success("Filtered Invoices Loaded", "");
                }, function (reason) {
                    notificationsService.success("Filtered Invoices Load Failed:", reason.message);
                });
            }
        };

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        $scope.init = function () {
            $scope.setVariables();
        	$scope.loadAllInvoices();
	        $scope.loadSettings();
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
            $scope.page = 0;
            $scope.loadAllInvoices();
        };

        /**
        * @ngdoc method
        * @name loadAllInvoices
        * @function
        * 
        * @description
        * Load the invoices from the invoice service, then wrap the results
        * in Merchello models and add to the scope via the invoices collection.
        */
        $scope.loadAllInvoices = function(page, perPage) {
            if (page === undefined) {
                page = $scope.currentPage;
            } else {
                $scope.currentPage = page;
            }
            if (perPage === undefined) {
                perPage = $scope.limitAmount;
            } else {
                $scope.limitAmount = perPage;
            }
            var promiseAll = merchelloInvoiceService.getAll(page, perPage);
            promiseAll.then(function (response) {
                var queryResult = new merchello.Models.QueryResult(response);
                $scope.invoices = _.map(queryResult.results, function (invoice) {
                    return new merchello.Models.Invoice(invoice);
                });
                $scope.maxPages = queryResult.totalPages;
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                if ($scope.selectedOrderCount > 0) {
                    $scope.selectAllOrders = true;
                    $scope.updateBulkActionDropdownStatus(true);
                }
            }, function (reason) {
                notificationsService.error("All Invoices Load Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         * 
         * @description
         * Load the settings from the settings service to get the currency symbol
         */
        $scope.loadSettings = function () {
            var currencySymbolPromise = merchelloSettingsService.getCurrencySymbol();
            currencySymbolPromise.then(function (currencySymbol) {
                $scope.currencySymbol = currencySymbol;

            }, function (reason) {
                notificationsService.error("Settings Load Failed", reason.message);
            });
            var settingsPromise = merchelloSettingsService.getAllSettings();
            settingsPromise.then(function (settingsFromServer) {
                $scope.settings = settingsFromServer;
            });
        };

        $scope.numberOfPages = function() {
            return $scope.maxPages;
        };

        /**
         * @ngdoc method
         * @name setVariables
         * @function
         * 
         * @description
         * Sets the $scope variables.
         */
        $scope.setVariables = function () {
            $scope.currentPage = 0;
            $scope.invoices = [];
            $scope.limitAmount = '100';
            $scope.maxPages = 0;
            $scope.orderIssues = [];
            $scope.selectAllOrders = false;
            $scope.selectedOrderCount = 0;
            $scope.settings = {};
            $scope.sortOrder = "desc";
            $scope.sortProperty = "-invoiceNumber";
            $scope.visible = {};
            $scope.visible.bulkActionDropdown = false;
        };

        /**
         * @ngdoc method
         * @name setVariables
         * @function
         * 
         * @description
         * Sets the $scope variables.
         */
        $scope.updateBulkActionDropdownStatus = function (toggle, key) {
            var i, shouldShowDropdown = false;
            $scope.selectedOrderCount = 0;
            if (toggle) {
                $scope.selectAllOrders = !$scope.selectAllOrders;
            }
            for (i = 0; i < $scope.invoices.length; i++) {
                if (toggle) {
                    $scope.invoices[i].selected = $scope.selectAllOrders;
                } else {
                    if ($scope.invoices[i].key === key) {
                        $scope.invoices[i].selected = !$scope.invoices[i].selected;
                    }
                }
                if ($scope.invoices[i].selected) {
                    shouldShowDropdown = true;
                    $scope.selectedOrderCount += 1;
                }
            }
            $scope.visible.bulkActionDropdown = shouldShowDropdown;
        };

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Order.ListController", ['$scope', 'assetsService', 'notificationsService', 'merchelloInvoiceService', 'merchelloSettingsService', merchello.Controllers.OrderListController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
