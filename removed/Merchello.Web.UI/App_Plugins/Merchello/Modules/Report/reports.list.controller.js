(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Report.ListController
     * @function
     * 
     * @description
     * The controller for the reports list page
     */
    controllers.ReportListController = function($scope) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    };


    angular.module("umbraco").controller("Merchello.Dashboards.Report.ListController", ['$scope', merchello.Controllers.ReportListController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
