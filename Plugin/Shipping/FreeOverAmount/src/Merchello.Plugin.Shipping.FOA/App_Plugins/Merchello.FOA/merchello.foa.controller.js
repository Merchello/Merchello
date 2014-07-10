(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.Shipping.Dialogs.FoaGatewayProviderController
     * @function
     * 
     * @description
     * The controller for the adding / editing Shipping Gateway Provider for Free Over Amount
     */
    controllers.FoaGatewayProviderController = function ($scope) {

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
            $scope.providerSettings = JSON.parse(settingsString);


            // Watch with object equality to convert back to a string for the submit() call on the Save button
            $scope.$watch(function () {
                return $scope.providerSettings;
            }, function (newValue, oldValue) {
                $scope.dialogData.provider.extendedData[0].value = angular.toJson(newValue);
            }, true);
        };
        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Plugin.GatewayProviders.Shipping.Dialogs.FoaGatewayProviderController", ['$scope', merchello.Controllers.FoaGatewayProviderController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
