/**
 * @ngdoc controller
 * @name Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingMethodController
 * @function
 * 
 * @description
 * The controller for the adding / editing shipping methods on the Shipping page
 */
angular.module('merchello.plugins.authorizenet').controller('Merchello.Plugin.GatewayProviders.Payments.Dialogs.AuthorizeNetGatewayProviderController',
    ['$scope', 'authorizenetProviderSettingsBuilder',
        function ($scope, authorizenetProviderSettingsBuilder) {

            $scope.authorizeNetSettings = {};

            /**
            * @ngdoc method 
            * @name init
            * @function
            * 
            * @description
            * Method called on intial page load.  Loads in data from server and sets up scope.
            */
            function init() {

                var json = JSON.parse($scope.dialogData.provider.extendedData.getValue('authorizeNetProcessorSettings'));
                $scope.authorizeNetSettings = authorizenetProviderSettingsBuilder.transform(json);

                // Watch with object equality to convert back to a string for the submit() call on the Save button
                $scope.$watch(function () {
                    return $scope.authorizeNetSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData.setValue('authorizeNetProcessorSettings', angular.toJson(newValue));
                }, true);
            }

            // initialize the controller
            init();

        }]);