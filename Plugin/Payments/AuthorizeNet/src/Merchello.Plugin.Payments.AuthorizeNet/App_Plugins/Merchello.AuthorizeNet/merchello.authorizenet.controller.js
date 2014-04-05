(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingMethodController
     * @function
     * 
     * @description
     * The controller for the adding / editing shipping methods on the Shipping page
     */
    controllers.AuthorizeNetGatewayProviderController = function ($scope) {

        /**
        * @ngdoc method
        * @name init
        * @function
        * 
        * @description
        * Method called on intial page load.  Loads in data from server and sets up scope.
        */
        $scope.init = function () {
            
            //$scope.dialogData.provider.extendedData

            // on initial load extendedData will be empty but we need to populate with key values
            // 
            alert($scope.dialogData.provider.extendedData);
        };
        $scope.init();


    };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.GatewayProviders.Dialogs.AuthorizeNetGatewayProviderController", ['$scope', merchello.Controllers.AuthorizeNetGatewayProviderController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
