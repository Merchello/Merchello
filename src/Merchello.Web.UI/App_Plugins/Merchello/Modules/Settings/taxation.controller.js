(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.TaxationController
     * @function
     * 
     * @description
     * The controller for the reports list page
     */
    controllers.TaxationController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    }


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.TaxationController", merchello.Controllers.TaxationController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
