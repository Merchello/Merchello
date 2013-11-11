(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.RegionsController
     * @function
     * 
     * @description
     * The controller for the reports list page
     */
    controllers.RegionsController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    }


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.RegionsController", merchello.Controllers.RegionsController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
