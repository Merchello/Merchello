(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Plugin.GatewayProviders.Payments.Dialogs.StripeGatewayProviderController
     * @function
     * 
     * @description
     * The controller todo
     */
    controllers.StripeGatewayProviderController = function ($scope) {

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
            if ($scope.dialogData.provider.extendedData.length > 0) {
                var settingsString = $scope.dialogData.provider.extendedData[0].value;
                $scope.stripeSettings = JSON.parse(settingsString);

                // Watch with object equality to convert back to a string for the submit() call on the Save button
                $scope.$watch(function () {
                    return $scope.stripeSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData[0].value = angular.toJson(newValue);
                }, true);
            }
        };
        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Plugin.GatewayProviders.Payments.Dialogs.StripeGatewayProviderController", ['$scope', merchello.Controllers.StripeGatewayProviderController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
