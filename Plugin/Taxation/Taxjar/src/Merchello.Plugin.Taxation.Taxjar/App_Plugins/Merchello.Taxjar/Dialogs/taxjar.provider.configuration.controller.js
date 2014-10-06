(function (controllers, undefined) {

    /**
    * @ngdoc controller
    * @name Merchello.Plugin.GatewayProviders.Taxation.Dialogs.TaxJarGatewayProviderController
    * @function
    * 
    * @description
    * The controller for the editing of TaxJar tax provider info on the Gateway Providers page.
    */
    controllers.TaxJarGatewayProviderController = function ($scope, $q, merchelloSettingsService) {

        /**
        * @ngdoc method 
        * @name buildTaxJarTaxSettingsFromString
        * @function
        * 
        * @description
        * On initial load extendedData will be empty but we need to populate with key values.
        */
        $scope.buildTaxJarTaxSettingsFromString = function () {
            if (!$scope.dialogData.provider.extendedData[0].value) {
                $scope.dialogData.provider.extendedData[0].value = "{\"ApiToken\":\"sk_test_4IiLJ0h3rtv4i6sJgW7dYHEW\"}";
            }
            var settingsString = $scope.dialogData.provider.extendedData[0].value;
            $scope.taxjartaxSettings = JSON.parse(settingsString);
        };


        /**
        * @ngdoc method 
        * @name init
        * @function
        * 
        * @description
        * Method called on intial page load.  Loads in data from server and sets up scope.
        */
        $scope.init = function () {
            $scope.setVariables();
            $scope.buildTaxJarTaxSettingsFromString();
        };

        /**
        * @ngdoc method
        * @name saveSettings
        * @function
        * 
        * @description
        * If the form is valid, acquire the settings and close the dialog, submitting the collected data.
        */
        $scope.saveSettings = function () {
            $scope.dialogData.provider.extendedData[0].value = angular.toJson($scope.taxjartaxSettings);
            $scope.submit($scope.dialogData);
        };

        /**
        * @ngdoc method
        * @name setVariables
        * @function
        * 
        * @description
        * Set the $scope variables.
        */
        $scope.setVariables = function () {
            $scope.wasFormSubmitted = false;
        };

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Plugin.GatewayProviders.Taxation.Dialogs.TaxJarGatewayProviderController", ['$scope', '$q', 'merchelloSettingsService', merchello.Controllers.TaxJarGatewayProviderController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
