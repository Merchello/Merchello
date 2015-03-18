/**
 * @ngdoc controller
 * @name Merchello.Plugin.GatewayProviders.Payments.Dialogs.ChaseGatewayProviderController
 * @function
 * 
 * @description
 * The controller for the provider settings dialog flyout
 */
angular.module('merchello.plugins.chase').controller('Merchello.Plugin.GatewayProviders.Payments.Dialogs.ChaseGatewayProviderController',
    ['$scope', 'chaseProviderSettingsBuilder',
        function ($scope, chaseProviderSettingsBuilder) {

            $scope.chaseSettings = {};

            /**
            * @ngdoc method 
            * @name init
            * @function
            * 
            * @description
            * Method called on intial page load.  Loads in data from server and sets up scope.
            */
            function init() {

                var json = JSON.parse($scope.dialogData.provider.extendedData.getValue('chaseProcessorSettings'));
                $scope.chaseSettings = chaseProviderSettingsBuilder.transform(json);

                // Watch with object equality to convert back to a string for the submit() call on the Save button
                $scope.$watch(function () {
                    return $scope.chaseSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData.setValue('chaseProcessorSettings', angular.toJson(newValue));
                }, true);
            }

            // initialize the controller
            init();

        }]);