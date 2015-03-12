/**
 * @ngdoc controller
 * @name Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingMethodController
 * @function
 * 
 * @description
 * The controller for the adding / editing shipping methods on the Shipping page
 */
angular.module('merchello.plugins.ups').controller('Merchello.Plugin.GatewayProviders.Shipping.Dialogs.UPSGatewayProviderController',
    ['$scope', 'upsProviderSettingsBuilder',
        function ($scope, upsProviderSettingsBuilder) {

            $scope.upsSettings = {};

            /**
            * @ngdoc method 
            * @name init
            * @function
            * 
            * @description
            * Method called on intial page load.  Loads in data from server and sets up scope.
            */
            function init() {

                var json = JSON.parse($scope.dialogData.provider.extendedData.getValue('upsProcessorSettings'));
                $scope.upsSettings = upsProviderSettingsBuilder.transform(json);

                // Watch with object equality to convert back to a string for the submit() call on the Save button
                $scope.$watch(function () {
                    return $scope.upsSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData.setValue('upsProcessorSettings', angular.toJson(newValue));
                }, true);
            }

            // initialize the controller
            init();

        }]);