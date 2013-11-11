(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.ShippingController
     * @function
     * 
     * @description
     * The controller for the reports list page
     */
    controllers.ShippingController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    }


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.ShippingController", merchello.Controllers.ShippingController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
