/**
 * @ngdoc controller
 * @name Merchello.Dashboards.Report.ListController
 * @function
 * 
 * @description
 * The controller for the reports list page
 */
function ReportListController($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

    $scope.loaded = true;
    $scope.preValuesLoaded = true;

}


angular.module("umbraco").controller("Merchello.Dashboards.Report.ListController", ReportListController);