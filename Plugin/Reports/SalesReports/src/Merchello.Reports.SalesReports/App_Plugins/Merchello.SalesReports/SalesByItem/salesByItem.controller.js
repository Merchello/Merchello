(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Report.SalesByItemController
     * @function
     * 
     * @description
     * The controller for the reports SalesByItem page
     */
    controllers.SalesByItemController = function ($scope, assetsService, merchelloPluginReportSalesByItemService) {

        $scope.loaded = false;
        $scope.preValuesLoaded = true;
        $scope.invoices = [];

        assetsService.loadCss('/App_Plugins/Merchello/Common/Css/merchello.css');

        $scope.defaultData = function () {
            
            var promise = merchelloPluginReportSalesByItemService.getDefaultData();
            promise.then(function (data) {
                $scope.loaded = true;
                $scope.invoices = data;
            });

        };

        $scope.init = function () {
            $scope.defaultData();
            $scope.loaded = true;
        };

        $scope.init();
    };


    angular.module("umbraco").controller("Merchello.Plugins.Reports.SalesByItemController", ['$scope', 'assetsService', 'merchelloPluginReportSalesByItemService', merchello.Controllers.SalesByItemController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
