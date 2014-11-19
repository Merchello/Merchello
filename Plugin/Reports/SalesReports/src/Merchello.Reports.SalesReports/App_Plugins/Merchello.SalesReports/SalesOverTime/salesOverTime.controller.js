(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Report.SalesOverTimeController
     * @function
     * 
     * @description
     * The controller for the reports SalesOverTime  page
     */
    controllers.SalesOverTimeController = function ($scope, assetsService) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

        assetsService.loadCss('/App_Plugins/Merchello/Common/Css/merchello.css');
    }


    angular.module("umbraco").controller("Merchello.Plugins.Reports.SalesOverTimeController", ['$scope', 'assetsService', merchello.Controllers.SalesOverTimeController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
