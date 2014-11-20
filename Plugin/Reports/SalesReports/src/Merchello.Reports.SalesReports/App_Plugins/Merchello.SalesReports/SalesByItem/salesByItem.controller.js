(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Report.SalesByItemController
     * @function
     * 
     * @description
     * The controller for the reports SalesByItem page
     */
    controllers.SalesByItemController = function ($scope, $element, assetsService, angularHelper, merchelloPluginReportSalesByItemService) {

        $scope.loaded = false;
        $scope.preValuesLoaded = true;
        $scope.results = [];
        $scope.itemsPerPage = 0;
        $scope.totalItems = 0;
        $scope.filterStartDate = '';
        $scope.filterEndDate = '';

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

            //Ensure to remove the event handler when this instance is destroyted
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
        $scope.applyDateStart = function (e) {
            angularHelper.safeApply($scope, function () {
                // when a date is changed, update the model
                if (e.localDate) {
                    $scope.filterStartDate = e.localDate.toIsoDateString();
                }
            });
        }

        //handles the date changing via the api
        $scope.applyDateEnd = function (e) {
            angularHelper.safeApply($scope, function () {
                // when a date is changed, update the model
                if (e.localDate) {
                    $scope.filterEndDate = e.localDate.toIsoDateString();
                }
            });
        }

        $scope.setDefaultDates = function(actual) {
            var month = actual.getMonth() == 0 ? 11 : actual.getMonth() - 1;

            var start = new Date(actual.getFullYear(), month, actual.getDay());
            $scope.applyDateStart(start);
            $scope.applyDateEnd(actual);
        }

        
        $scope.defaultData = function () {
            
            var promise = merchelloPluginReportSalesByItemService.getDefaultData();
            promise.then(function (data) {
                $scope.loaded = true;
                $scope.results = _.map(data.items, function(resultFromServer) {
                    return new merchello.Models.SaleByItemResult(resultFromServer, true);
                });
                $scope.itemsPerPage = data.itemsPerPage;
                $scope.totalItems = data.totalItems;
            });

        };

        $scope.init = function () {
            $scope.setDefaultDates(new Date());
            $scope.defaultData();
            $scope.loaded = true;
        };

        $scope.init();
    };


    angular.module("umbraco").controller("Merchello.Plugins.Reports.SalesByItemController", ['$scope', '$element', 'assetsService', 'angularHelper', 'merchelloPluginReportSalesByItemService', merchello.Controllers.SalesByItemController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
