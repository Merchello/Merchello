(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Plugins.Reports.ExportOrders
     * @function
     * 
     * @description
     * The controller for the reports Export Orders page
     */
    controllers.ExportOrdersController = function ($scope, merchelloPluginReportOrderExportService) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

        $scope.exportStatus = "";

        $scope.exportOrders = function () {
            $scope.exportStatus = "Exporting Orders!";
            var promise = merchelloPluginReportOrderExportService.getAllOrders();
            promise.then(function (data) {
                $scope.loaded = true;
                $scope.invoices = data;
                $scope.exportStatus = "Download";
            });
        }

        $scope.finishDownload = function () {
            $scope.exportStatus = "";
        }
    };


    angular.module("umbraco").controller("Merchello.Plugins.Reports.ExportOrders", ['$scope', 'merchelloPluginReportOrderExportService', merchello.Controllers.ExportOrdersController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
