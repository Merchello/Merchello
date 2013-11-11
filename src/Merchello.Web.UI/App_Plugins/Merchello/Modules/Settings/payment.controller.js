(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.PaymentController
     * @function
     * 
     * @description
     * The controller for the reports list page
     */
    controllers.PaymentController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    }


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.PaymentController", merchello.Controllers.PaymentController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
