(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Report.SalesOverTimeController
     * @function
     * 
     * @description
     * The controller for the reports SalesOverTime  page
     */
    controllers.SalesOverTimeController = function ($scope) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    }


    angular.module("umbraco").controller("Merchello.Dashboards.Report.SalesOverTimeController", ['$scope', merchello.Controllers.SalesOverTimeController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
