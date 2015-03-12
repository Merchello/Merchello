/**
 * @ngdoc controller
 * @name Merchello.Plugin.GatewayProviders.Shipping.Dialogs.USPSGatewayProviderController
 * @function
 * 
 * @description
 * The controller for editing the Shipping Gateway Provider for USPS
 */
angular.module('merchello.plugins.usps').controller('Merchello.Plugin.GatewayProviders.Shipping.Dialogs.USPSGatewayProviderController',
    ['$scope', 'uspsProviderSettingsBuilder',
        function ($scope, uspsProviderSettingsBuilder) {

            $scope.uspsSettings = {};

            /**
            * @ngdoc method 
            * @name init
            * @function
            * 
            * @description
            * Method called on intial page load.  Loads in data from server and sets up scope.
            */
            function init() {

                var json = JSON.parse($scope.dialogData.provider.extendedData.getValue('uspsProcessorSettings'));
                $scope.uspsSettings = uspsProviderSettingsBuilder.transform(json);

                // Watch with object equality to convert back to a string for the submit() call on the Save button
                $scope.$watch(function () {
                    return $scope.uspsSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData.setValue('uspsProcessorSettings', angular.toJson(newValue));
                }, true);
            }

            // initialize the controller
            init();

        }]);