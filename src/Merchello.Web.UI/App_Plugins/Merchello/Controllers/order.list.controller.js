/**
 * @ngdoc controller
 * @name Merchello.Dashboards.Order.ListController
 * @function
 * 
 * @description
 * The controller for the customers list page
 */
function OrderListController($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

    $scope.loaded = true;
    $scope.preValuesLoaded = true;

}


angular.module("umbraco").controller("Merchello.Dashboards.Order.ListController", OrderListController);