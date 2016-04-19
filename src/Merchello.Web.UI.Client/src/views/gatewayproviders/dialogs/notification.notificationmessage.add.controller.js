    /**
     * @ngdoc controller
     * @name Merchello.GatewayProviders.Dialogs.NotificationsMessageAddController
     * @function
     *
     * @description
     * The controller for the adding / editing Notification messages on the Notifications page
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.NotificationsMessageAddController',
        ['$scope',
        function($scope) {

            // exposed methods
            $scope.save = save;
            $scope.setMonitorDefaults = setMonitorDefaults;

            function init() {
                setMonitorDefaults();
            }

            function setMonitorDefaults() {
               var subject = $scope.dialogData.selectedMonitor.name.replace(' (Legacy)', '').replace(' (Razor)', '');
                $scope.dialogData.notificationMessage.name = subject;
                $scope.dialogData.notificationMessage.bodyTextIsFilePath = $scope.dialogData.selectedMonitor.useCodeEditor;
            }

            function save() {
                $scope.dialogData.notificationMessage.monitorKey = $scope.dialogData.selectedMonitor.monitorKey;
                $scope.submit($scope.dialogData);
            }

            init();
    }]);
