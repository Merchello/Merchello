(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Plugins.Reports.ExportOrders
     * @function
     * 
     * @description
     * The controller for the reports Export Orders page
     */
    controllers.ExportOrdersController = function ($scope) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

        $scope.exportStatus = "";

        $scope.exportOrders = function() {
            $scope.exportStatus = "Export Orders button was clicked!";
        }
    };


    angular.module("umbraco").controller("Merchello.Plugins.Reports.ExportOrders", ['$scope', merchello.Controllers.ExportOrdersController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
