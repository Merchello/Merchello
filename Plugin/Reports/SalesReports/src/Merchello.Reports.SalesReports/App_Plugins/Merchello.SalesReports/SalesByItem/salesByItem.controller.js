(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Report.SalesByItemController
     * @function
     * 
     * @description
     * The controller for the reports SalesByItem page
     */
    controllers.SalesByItemController = function ($scope, $element, assetsService, angularHelper, notificationsService, merchelloSettingsService, merchelloPluginReportSalesByItemService) {

        $scope.loaded = false;
        $scope.preValuesLoaded = true;
        $scope.results = [];
        $scope.itemsPerPage = 0;
        $scope.totalItems = 0;
        $scope.filterStartDate = '';
        $scope.filterEndDate = '';
        $scope.currentFilters = [];

        /**
         * @ngdoc method
         * @name setupDatePicker
         * @function
         * 
         * @description
         * Sets up the datepickers
        */
        $scope.setupDatePicker = function (pickerId) {

            // Open the datepicker and add a changeDate eventlistener
            $element.find(pickerId).datetimepicker();

            //Ensure to remove the event handler when this instance is destroyed
            $scope.$on('$destroy', function () {
                $element.find(pickerId).datetimepicker("destroy");
            });
        };

        assetsService.loadCss('/App_Plugins/Merchello/Common/Css/merchello.css');
        assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css').then(function () {
            var filesToLoad = ["lib/datetimepicker/bootstrap-datetimepicker.min.js"];
            assetsService.load(filesToLoad).then(
                function () {

                    //The Datepicker js and css files are available and all components are ready to use.
                    $scope.setupDatePicker("#filterStartDate");
                    $element.find("#filterStartDate").datetimepicker().on("changeDate", $scope.applyDateStart);
                   

                    $scope.setupDatePicker("#filterEndDate");
                    $element.find("#filterEndDate").datetimepicker().on("changeDate", $scope.applyDateEnd);
                });
        });


        //handles the date changing via the api
        $scope.applyDateStart = function(e) {
            angularHelper.safeApply($scope, function() {
                // when a date is changed, update the model
                if (e.localDate) {
                    $scope.filterStartDate = e.localDate.toIsoDateString();
                }
            });
        };

        //handles the date changing via the api
        $scope.applyDateEnd = function(e) {
            angularHelper.safeApply($scope, function() {
                // when a date is changed, update the model
                if (e.localDate) {
                    
                    $scope.filterEndDate = e.localDate.toIsoDateString();
                }
            });
        };


        /**
         * @ngdoc method
         * @name buildQueryDates
         * @function
         * 
         * @description
         * Perpares a new query object for passing to the ApiController
         */
        $scope.buildQueryDates = function (startDate, endDate) {

            if (startDate === undefined && endDate === undefined) {
                $scope.currentFilters = [];
            } else {

                if (Date.parse(startDate) > Date.parse(endDate)) {
                    var temp = startDate;
                    startDate = endDate;
                    endDate = temp;
                    $scope.filterStartDate = startDate;
                    $scope.filterEndDate = endDate;
                }

                $scope.currentFilters = [{
                    fieldName: 'invoiceDateStart',
                    value: startDate
                }, {
                    fieldName: 'invoiceDateEnd',
                    value: endDate
                }];
            }

            $scope.filterStartDate = startDate;
            var listQuery = new merchello.Models.ListQuery({
                currentPage: 0,
                itemsPerPage: 100,
                sortBy: 'invoiceDate',
                sortDirection: 'desc',
                parameters: $scope.currentFilters
            });

            return listQuery;
        };

        /**
        * @ngdoc method
        * @name loadInvoicesByDates
        * @function
        * 
        * @description
        * Load the invoices, either filtered or not, depending on the current page, and status of the filterStartDate/filterEndDate variables.
        */
        $scope.renderReport = function(promiseResults) {
            promiseResults.then(function (response) {
                var queryResult = new merchello.Models.QueryResult(response);
                $scope.results = _.map(queryResult.items, function (resultFromServer) {
                    return new merchello.Models.SaleByItemResult(resultFromServer, true);
                });
                $scope.itemsPerPage = queryResult.itemsPerPage;
                $scope.totalItems = queryResult.totalItems;
                $scope.loaded = true;
            }, function (reason) {
                notificationsService.error("Failed To sales by item data", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name filterWithDates
         * @function
         * 
         * @description
         * Loads a sales by item report filtered by a date range
         */
        $scope.filterWithDates = function (filterStartDate, filterEndDate) {

            $scope.loaded = false;
            var listQuery = $scope.buildQueryDates(filterStartDate, filterEndDate);
            $scope.renderReport(merchelloPluginReportSalesByItemService.searchByDateRange(listQuery));
            
        };

        ///**
        // * @ngdoc method
        // * @name exportFilterWithDates
        // * @function
        // * 
        // * @description
        // * Loads a sales by item report filtered by a date range and exports the data to a csv file
        // */
        //$scope.exportFilterWithDates = function(filterStartDate, filterEndDate) {
        //    $scope.loaded = false;
        //    var listQuery = $scope.buildQueryDates(filterStartDate, filterEndDate);
        //    $scope.renderReport(merchelloPluginReportSalesByItemService.searchByDateRange(listQuery));
        //};

        /**
         * @ngdoc method
         * @name defaultData
         * @function
         * 
         * @description
         * Loads a sales by item report with default data
         */
        $scope.defaultData = function () {
            $scope.renderReport(merchelloPluginReportSalesByItemService.getDefaultData());
        };

        /**
         * @ngdoc method
         * @name setDefaultDates
         * @function
         * 
         * @description
         * Sets the default dates
         */
        $scope.setDefaultDates = function (actual) {
            var month = actual.getMonth() == 0 ? 11 : actual.getMonth() - 1;

            var start = new Date(actual.getFullYear(), month, actual.getDate());
            var end = new Date(actual.getFullYear(), actual.getMonth(), actual.getDate());
            $scope.filterStartDate = start.toLocaleDateString();
            $scope.filterEndDate = end.toLocaleDateString();

            //$element.find("#filterStartDate").datetimepicker().setStartDate($scope.filterStartDate);
        };


        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Initializes the controller
         */
        $scope.init = function () {
            $scope.setDefaultDates(new Date());
            $scope.defaultData();
            $scope.loaded = true;
        };

        $scope.init();
    };


    angular.module("umbraco").controller("Merchello.Plugins.Reports.SalesByItemController", ['$scope', '$element', 'assetsService', 'angularHelper', 'notificationsService', 'merchelloSettingsService', 'merchelloPluginReportSalesByItemService', merchello.Controllers.SalesByItemController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
