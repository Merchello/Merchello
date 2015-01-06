(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Catalog.Dialogs.SmtpNotificationsProviderSettingsController
     * @function
     * 
     * @description
     * The controller for updating the Smtp provider settings (relay info) - host, username, passord, ssl
     */
    controllers.SmtpNotificationsProviderSettingsController = function ($scope) {

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
                $scope.notificationProviderSettings = JSON.parse(settingsString);
                console.info($scope.dialogData);
                console.info($scope.notificationProviderSettings);

                // Watch with object equality to convert back to a string for the submit() call on the Save button
                $scope.$watch(function () {
                    return $scope.notificationProviderSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData[0].value = angular.toJson(newValue);
                }, true);
            }
        };
        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.Notifications.Dialogs.SmtpNotificationsProviderSettingsController", ['$scope', merchello.Controllers.SmtpNotificationsProviderSettingsController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));