(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingMethodController
     * @function
     * 
     * @description
     * The controller for the adding / editing shipping methods on the Shipping page
     */
    controllers.UPSGatewayProviderController = function ($scope) {

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
            var settingsString = $scope.dialogData.provider.extendedData[0].value;
            $scope.upsSettings = JSON.parse(settingsString);


            // Watch with object equality to convert back to a string for the submit() call on the Save button
            $scope.$watch(function () {
                return $scope.upsSettings;
            }, function (newValue, oldValue) {
                $scope.dialogData.provider.extendedData[0].value = angular.toJson(newValue);
            }, true);
        };
        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Plugin.GatewayProviders.Shipping.Dialogs.UPSGatewayProviderController", ['$scope', merchello.Controllers.UPSGatewayProviderController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
