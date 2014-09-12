(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingMethodController
     * @function
     * 
     * @description
     * The controller for the adding / editing shipping methods on the Shipping page
     */
    controllers.PurchaseOrderGatewayProviderController = function ($scope) {

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
                $scope.purchaseOrderSettings = JSON.parse(settingsString);

                // Watch with object equality to convert back to a string for the submit() call on the Save button
                $scope.$watch(function () {
                    return $scope.purchaseOrderSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData[0].value = angular.toJson(newValue);
                },true);
            }
        };
        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Plugin.GatewayProviders.Payments.Dialogs.PurchaseOrderGatewayProviderController", ['$scope', merchello.Controllers.PurchaseOrderGatewayProviderController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
