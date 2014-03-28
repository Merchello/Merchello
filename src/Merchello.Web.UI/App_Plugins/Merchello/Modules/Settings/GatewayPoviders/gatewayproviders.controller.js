(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.GatewayProvidersController
     * @function
     * 
     * @description
     * The controller for the gateway providers settings page
     */

    controllers.GatewayProvidersController = function($scope, notificationsService, dialogService) {
        alert("got here");
    };
    
    angular.module("umbraco").controller("Merchello.Dashboards.Settings.GatewayProvidersController", ['$scope', 'notificationsService', 'dialogService', merchello.Controllers.GatewayProvidersController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));

