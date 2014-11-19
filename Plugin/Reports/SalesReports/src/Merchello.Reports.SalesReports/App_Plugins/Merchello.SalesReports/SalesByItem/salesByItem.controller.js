(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Report.SalesByItemController
     * @function
     * 
     * @description
     * The controller for the reports SalesByItem page
     */
    controllers.SalesByItemController = function ($scope, assetsService, angularHelper, merchelloPluginReportSalesByItemService) {

        $scope.loaded = false;
        $scope.preValuesLoaded = true;
        $scope.results = [];
        $scope.itemsPerPage = 0;
        $scope.totalItems = 0;

        assetsService.loadCss('/App_Plugins/Merchello/Common/Css/merchello.css');

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
            $scope.defaultData();
            $scope.loaded = true;
        };

        $scope.init();
    };


    angular.module("umbraco").controller("Merchello.Plugins.Reports.SalesByItemController", ['$scope', 'assetsService', 'angularHelper', 'merchelloPluginReportSalesByItemService', merchello.Controllers.SalesByItemController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
