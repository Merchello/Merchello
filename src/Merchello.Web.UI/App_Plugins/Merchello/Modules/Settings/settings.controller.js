(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.PageController
     * @function
     * 
     * @description
     * The controller for the reports list page
     */
    controllers.SettingsPageController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    }


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.PageController", merchello.Controllers.SettingsPageController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
