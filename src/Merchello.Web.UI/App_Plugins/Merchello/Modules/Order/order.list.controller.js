(function (controllers, undefined) {
    
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Order.ListController
     * @function
     * 
     * @description
     * The controller for the customers list page
     */
    controllers.OrderListController = function($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    }


    angular.module("umbraco").controller("Merchello.Dashboards.Order.ListController", merchello.Controllers.OrderListController);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
