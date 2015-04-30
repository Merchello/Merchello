(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Plugins.Reports.ExportOrders
     * @function
     * 
     * @description
     * The controller for the reports Export Orders page
     */
    controllers.ExportOrdersController = function ($scope, merchelloPluginReportOrderExportService, queryDisplayBuilder) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

        $scope.itemsPerPage = 0;
        $scope.totalItems = 0;
        $scope.filterStartDate = '';
        $scope.filterEndDate = '';
        $scope.currentFilters = [];

        // exposed methods
        $scope.exportOrders = exportOrders;

        /**
         * @ngdoc method
         * @name init
         * @function
         *
         * @description
         * Initializes the controller
         */
        function init() {
            setDefaultDates(new Date());
        }

        /**
         * @ngdoc method
         * @name exportOrders
         * @function
         *
         * @description
         * Requests order data download
         */
        function exportOrders(filterStartDate, filterEndDate) {
            // prevent exporting more orders until current order is complete
            if ($scope.loaded == false) {
                return;
            }

            $scope.filterStartDate = filterStartDate;
            $scope.filterEndDate = filterEndDate;
            $scope.loaded = false;

            var query = buildQueryDates($scope.filterStartDate, $scope.filterEndDate);
            console.info(query);
            var promise = merchelloPluginReportOrderExportService.getOrdersByDateRange(query);
            promise.then(function (data) {
                // IE fix. Must download csv via msSaveBlob
                if (window.navigator.msSaveOrOpenBlob) {
                    var blob = new Blob([decodeURIComponent(encodeURI(data))], {
                        type: 'text/csv;charset=utf-8'
                    });
                    navigator.msSaveBlob(blob, 'orders.csv');
                }
                else {
                    var hiddenElement = document.createElement('a');
                    hiddenElement.href = 'data:attachment/csv,' + encodeURI(data);
                    hiddenElement.target = '_blank';
                    hiddenElement.download = 'orders.csv';
                    // Firefox fix. Element must be on DOM for download click to work
                    document.body.appendChild(hiddenElement);
                    hiddenElement.click();
                }

                $scope.loaded = true;
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
                if (Date.parse(startDate) > Date.parse(endDate)) {
                    var temp = startDate;
                    startDate = endDate;
                    endDate = temp;
                }
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

        init();
    };


    angular.module("umbraco").controller("Merchello.Plugins.Reports.ExportOrders", ['$scope', 'merchelloPluginReportOrderExportService', 'queryDisplayBuilder', merchello.Controllers.ExportOrdersController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
