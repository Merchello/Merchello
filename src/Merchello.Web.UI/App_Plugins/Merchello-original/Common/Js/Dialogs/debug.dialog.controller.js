(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Common.Dialogs.DebugDialog
     * @function
     * 
     * @description
     * The controller for adding a country
     */
    controllers.DebugDialog = function ($scope, assetsService) {

	    assetsService.loadCss("/App_Plugins/Merchello/lib/ngJsonExplorer/gd-ui-jsonexplorer.css");

    };

    angular.module("umbraco").controller("Merchello.Common.Dialogs.DebugDialog", ['$scope', 'assetsService', merchello.Controllers.DebugDialog]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
