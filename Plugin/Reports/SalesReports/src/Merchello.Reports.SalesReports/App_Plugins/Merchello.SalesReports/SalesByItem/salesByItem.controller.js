(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Report.SalesByItemController
     * @function
     * 
     * @description
     * The controller for the reports SalesByItem page
     */
    controllers.SalesByItemController = function ($scope, merchelloPluginReportSalesByItemService) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;
        $scope.invoices = [];

        $scope.defaultData = function () {
            
            var promise = merchelloPluginReportSalesByItemService.getDefaultData();
            promise.then(function (data) {
                $scope.loaded = true;
                $scope.invoices = data;
            });

        };

        $scope.init = function () {
            $scope.defaultData();
        };

        $scope.init();
    };


    angular.module("umbraco").controller("Merchello.Plugins.Reports.SalesByItemController", ['$scope', merchello.Controllers.SalesByItemController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
