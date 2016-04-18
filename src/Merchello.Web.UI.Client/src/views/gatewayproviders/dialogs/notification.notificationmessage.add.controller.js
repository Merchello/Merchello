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
            $scope.setSubject = setSubject;

            function init() {
                setSubject();
            }

            function setSubject() {
               var subject = $scope.dialogData.selectedMonitor.name.replace(' (Legacy)', '');

                $scope.dialogData.notificationMessage.name = subject;
            }

            function save() {
                $scope.dialogData.notificationMessage.monitorKey = $scope.dialogData.selectedMonitor.monitorKey;
                $scope.submit($scope.dialogData);
            }

            init();
    }]);
