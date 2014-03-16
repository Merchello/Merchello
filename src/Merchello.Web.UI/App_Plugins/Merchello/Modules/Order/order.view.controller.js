(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Editors.Order.ViewController
     * @function
     * 
     * @description
     * The controller for the order view page
     */
    controllers.OrderViewController = function($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.invoice = {};

        $scope.loaded = true;
        $scope.preValuesLoaded = true;


    };


    angular.module("umbraco").controller("Merchello.Editors.Order.ViewController", merchello.Controllers.OrderViewController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));

