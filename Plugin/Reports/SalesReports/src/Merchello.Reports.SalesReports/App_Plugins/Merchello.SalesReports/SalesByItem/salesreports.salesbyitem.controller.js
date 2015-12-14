angular.module('merchello.salesreports').controller('Merchello.Plugins.SalesReports.SalesByItemController',
    ['$scope', '$element', 'angularHelper', 'notificationsService', 'queryDisplayBuilder', 'queryResultDisplayBuilder', 'settingsResource',
        'saleByItemResultBuilder', 'salesByItemResource',
    function ($scope, $element, angularHelper, notificationsService, queryDisplayBuilder, queryResultDisplayBuilder, settingsResource, saleByItemResultBuilder,
              salesByItemResource) {

        $scope.loaded = false;
        $scope.preValuesLoaded = true;
        $scope.results = [];
        $scope.itemsPerPage = 0;
        $scope.totalItems = 0;
        $scope.filterStartDate = '';
        $scope.filterEndDate = '';
        $scope.currentFilters = [];
        $scope.currencySymbol = '';
        // exposed methods
        $scope.filterWithDates = filterWithDates;

        /**
         * @ngdoc method
         * @name init
         * @function
         *
         * @description
         * Initializes the controller
         */
        function init() {

            settingsResource.getCurrencySymbol().then(function(symbol) {
                $scope.currencySymbol = symbol;
                setDefaultDates(new Date());
                defaultData();
            });
        }

        /**
         * @ngdoc method
         * @name buildQueryDates
         * @function
         *
         * @description
         * Perpares a new query object for passing to the ApiController
         */
        function buildQueryDates(startDate, endDate) {

            var query = queryDisplayBuilder.createDefault();

            if (startDate === undefined && endDate === undefined) {
                $scope.currentFilters = [];
            } else {
                $scope.filterStartDate = startDate;
                $scope.filterEndDate = endDate;
                query.addInvoiceDateParam($scope.filterStartDate, 'start');
                query.addInvoiceDateParam($scope.filterEndDate, 'end');
            }

            query.currentPage = 0;
            query.itemsPerPage = 25;
            query.sortBy = 'invoiceDate';
            query.sortDirection = 'desc';

            return query;
        }

        /**
         * @ngdoc method
         * @name loadInvoicesByDates
         * @function
         *
         * @description
         * Load the invoices, either filtered or not, depending on the current page, and status of the filterStartDate/filterEndDate variables.
         */
        function renderReport(promiseResults) {
            promiseResults.then(function (response) {
                var queryResult = queryDisplayBuilder.transform(response, saleByItemResultBuilder);
                $scope.results = queryResult.items;
                $scope.itemsPerPage = queryResult.itemsPerPage;
                $scope.totalItems = queryResult.totalItems;
                $scope.loaded = true;
            }, function (reason) {
                notificationsService.error("Failed To sales by item data", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name filterWithDates
         * @function
         *
         * @description
         * Loads a sales by item report filtered by a date range
         */
        function filterWithDates(filterStartDate, filterEndDate) {
            $scope.loaded = false;
            var query = buildQueryDates(filterStartDate, filterEndDate);
            console.info(query);
            renderReport(salesByItemResource.searchByDateRange(query));
        }

        /**
         * @ngdoc method
         * @name defaultData
         * @function
         *
         * @description
         * Loads a sales by item report with default data
         */
        function defaultData() {
            renderReport(salesByItemResource.getDefaultData());
        }

        /**
         * @ngdoc method
         * @name setDefaultDates
         * @function
         *
         * @description
         * Sets the default dates
         */
        function setDefaultDates(actual) {
            var month = actual.getMonth() == 0 ? 11 : actual.getMonth() - 1;
            var start = new Date(actual.getFullYear(), month, actual.getDate());
            var end = new Date(actual.getFullYear(), actual.getMonth(), actual.getDate());
            $scope.filterStartDate = start.toLocaleDateString();
            $scope.filterEndDate = end.toLocaleDateString();
        }

        // Initializes the controller
        init();

}]);
