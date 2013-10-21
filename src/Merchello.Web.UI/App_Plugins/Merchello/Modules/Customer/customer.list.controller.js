/**
 * @ngdoc controller
 * @name Merchello.Dashboards.Customer.ListController
 * @function
 * 
 * @description
 * The controller for the customers list page
 */
function CustomerListController($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

    $scope.loaded = true;
    $scope.preValuesLoaded = true;

}


angular.module("umbraco").controller("Merchello.Dashboards.Customer.ListController", CustomerListController);