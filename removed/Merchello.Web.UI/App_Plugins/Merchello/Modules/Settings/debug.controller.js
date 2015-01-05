(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.DebugController
     * @function
     * 
     * @description
     * The controller for the reports list page
     */
    controllers.DebugController = function($scope) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    };


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.DebugController", ['$scope', merchello.Controllers.DebugController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
