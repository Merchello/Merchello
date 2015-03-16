/**
 * @ngdoc controller
 * @name Merchello.Plugin.GatewayProviders.Shipping.Dialogs.FedExGatewayProviderController
 * @function
 * 
 * @description
 * The controller for the adding / editing shipping methods on the Shipping page
 */
angular.module('merchello.plugins.fedex').controller('Merchello.Plugin.GatewayProviders.Shipping.Dialogs.FedExGatewayProviderController',
    ['$scope', 'fedexProviderSettingsBuilder',
        function ($scope, fedexProviderSettingsBuilder) {

            $scope.fedexSettings = {};

            /**
            * @ngdoc method 
            * @name init
            * @function
            * 
            * @description
            * Method called on intial page load.  Loads in data from server and sets up scope.
            */
            function init() {

                var json = JSON.parse($scope.dialogData.provider.extendedData.getValue('fedExProcessorSettings'));
                $scope.fedexSettings = fedexProviderSettingsBuilder.transform(json);

                // Watch with object equality to convert back to a string for the submit() call on the Save button
                $scope.$watch(function () {
                    return $scope.fedexSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData.setValue('fedExProcessorSettings', angular.toJson(newValue));
                }, true);
            }

            // initialize the controller
            init();

        }]);