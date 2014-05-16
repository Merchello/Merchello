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

        $scope.orderIssues = [];
        $scope.invoices = [];
        $scope.sortProperty = "name";
        $scope.sortOrder = "asc";
        $scope.limitAmount = 10;
        $scope.currentPage = 0;

        assetsService.loadCss("/App_Plugins/Merchello/Common/Css/merchello.css");

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        $scope.loadAllInvoices = function () {

            var promiseAll = merchelloInvoiceService.getAll();
            promiseAll.then(function (allInvoices) {

                $scope.invoices = _.map(allInvoices, function (invoice) {
                    return new merchello.Models.Invoice(invoice);
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {

                notificationsService.error("All Invoices Load Failed", reason.message);

            });

        };

        $scope.loadSettings = function () {


        	var currencySymbolPromise = merchelloSettingsService.getCurrencySymbol();
        	currencySymbolPromise.then(function (currencySymbol) {
        		$scope.currencySymbol = currencySymbol;

        	}, function (reason) {
        		alert('Failed: ' + reason.message);
        	});
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

        	$scope.loadAllInvoices();
	        $scope.loadSettings();

        };

        $scope.init();


        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------

        $scope.limitChanged = function (newVal) {
            $scope.limitAmount = newVal;
        };

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

        $scope.getFilteredInvoices = function (filter) {
            notificationsService.info("Filtering...", "");

            if (merchello.Helpers.Strings.isNullOrEmpty(filter)) {
                $scope.loadAllInvoices();
                notificationsService.success("Filtered Invoices Loaded", "");
            } else {
                var promise = merchelloInvoiceService.getFiltered(filter);

                promise.then(function (invoices) {

                    $scope.invoices = _.map(invoices, function (invoice) {
                        return new merchello.Models.Invoice(invoice);
                    });

                    notificationsService.success("Filtered Invoices Loaded", "");

                }, function (reason) {

                    notificationsService.success("Filtered Invoices Load Failed:", reason.message);

                });
            }
        };

        //--------------------------------------------------------------------------------------
        // Calculations
        //--------------------------------------------------------------------------------------

        $scope.numberOfPages = function () {
            return Math.ceil($scope.invoices.length / $scope.limitAmount);
        };



    };


    angular.module("umbraco").controller("Merchello.Dashboards.Order.ListController", ['$scope', 'assetsService', 'notificationsService', 'merchelloInvoiceService', 'merchelloSettingsService', merchello.Controllers.OrderListController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
