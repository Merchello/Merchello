(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Report.BootstrapController
     * @function
     * 
     * @description
     * The controller for the ViewReport page
     * 
     * This is a bootstrapper to allow reports that are plugins to be loaded using the merchello application route.
     */
    controllers.ViewReportBootstrapController = function ($scope, $routeParams) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

        // Property to control the report to show
        $scope.reportParam = $routeParams.id;

        var re = /(\|)+/;
        var subst = '/';

        var result = $scope.reportParam.replace(re, subst);

        //$scope.reportPath = "/App_Plugins/Merchello.ExportOrders/ExportOrders.html";
        $scope.reportPath = "/App_Plugins/" + result + ".html";

    };


    angular.module("umbraco").controller("Merchello.Dashboards.Report.BootstrapController", ['$scope', '$routeParams', merchello.Controllers.ViewReportBootstrapController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
