(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Report.SalesByItemController
     * @function
     * 
     * @description
     * The controller for the reports SalesByItem page
     */
    controllers.SalesByItemController = function($scope) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    };


    angular.module("umbraco").controller("Merchello.Dashboards.Report.SalesByItemController", ['$scope', merchello.Controllers.SalesByItemController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
