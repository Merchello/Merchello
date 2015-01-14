    /**
     * @ngdoc controller
     * @name Merchello.GatewayProvider.Dialogs.NotificationsProviderSettingsSmtpController
     * @function
     *
     * @description
     * The controller for configuring thee SMTP provider
     */
    angular.module('merchello').controller('Merchello.GatewayProvider.Dialogs.NotificationsProviderSettingsSmtpController',
        ['$scope', function($scope) {

            $scope.notificationProviderSettings = {};

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {

                //$scope.dialogData.provider.extendedData

                // on initial load extendedData will be empty but we need to populate with key values
                //
                if ($scope.dialogData.provider.extendedData.items.length > 0) {
                    var extendedDataKey = 'merchSmtpProviderSettings';
                    var settingsString = $scope.dialogData.provider.extendedData.getValue(extendedDataKey);
                    $scope.notificationProviderSettings = angular.fromJson(settingsString);
                    console.info($scope.dialogData);
                    console.info($scope.notificationProviderSettings);

                    // Watch with object equality to convert back to a string for the submit() call on the Save button
                    $scope.$watch(function () {
                        return $scope.notificationProviderSettings;
                    }, function (newValue, oldValue) {
                        $scope.dialogData.provider.extendedData.setValue(extendedDataKey, angular.toJson(newValue));
                    }, true);
                }
            }

            // Initialize
            init();

        }]);
