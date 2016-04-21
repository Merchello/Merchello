
    angular.module('merchello').controller('Merchello.Backoffice.NotificationMessageEditorController',
    ['$scope', '$q', '$routeParams', '$location', 'assetsService', 'dialogService', 'eventsService', 'notificationsService', 'merchelloTabsFactory', 'dialogDataFactory',
    'notificationGatewayProviderResource', 'notificationMessageDisplayBuilder', 'notificationMonitorDisplayBuilder',
    function($scope, $q, $routeParams, $location, assetsService, dialogService, eventsService, notificationsService, merchelloTabsFactory, dialogDataFactory,
    notificationGatewayProviderResource, notificationMessageDisplayBuilder, notificationMonitorDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.tabs = [];
        $scope.notificationMessage = {};
        $scope.notificationMonitors = [];
        $scope.currentMonitor = {};
        
        $scope.reload = init;


        // exposed methods
        $scope.save = save;
        $scope.deleteMessage = deleteMessage;

        var saveEventName = 'notification.message.saved';
        
        function init() {
            $scope.loaded = false;
            var key = $routeParams.id;

            $q.all([
                notificationGatewayProviderResource.getNotificationMessagesByKey(key),
                notificationGatewayProviderResource.getAllNotificationMonitors()
            ]).then(function(data) {

                $scope.notificationMessage = notificationMessageDisplayBuilder.transform(data[0]);
                $scope.notificationMonitors = notificationMonitorDisplayBuilder.transform(data[1]);
                
                $scope.currentMonitor = _.find($scope.notificationMonitors, function(m) {
                   return m.monitorKey === $scope.notificationMessage.monitorKey;
                });

                $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
                $scope.tabs.insertTab('messageEditor', 'merchelloTabs_message', '#/merchello/merchello/notification.messageeditor/' + key, 2);
                $scope.tabs.setActive('messageEditor');

                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            });
        }
        

        function deleteMessage() {
            var dialogData = dialogDataFactory.createDeleteNotificationMessageDialogData();
            dialogData.notificationMessage = $scope.notificationMessage;
            dialogData.name = $scope.notificationMessage.name;

            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                show: true,
                callback: notificationsMessageDeleteDialogConfirm,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name notificationsMessageDeleteDialogConfirm
         * @function
         *
         * @description
         * Handles the delete after recieving the deleted command from the dialog view/controller
         */
        function notificationsMessageDeleteDialogConfirm(dialogData) {
            var promiseNotificationMethod = notificationGatewayProviderResource.deleteNotificationMessage(dialogData.notificationMessage.key);
            promiseNotificationMethod.then(function () {
                $location.url('merchello/merchello/notificationproviders/manage', true);
            }, function (reason) {
                notificationsService.error("Notification Method Deletion Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name save
         * @function
         *
         * @description
         * Saves the notification message
         */
        function save() {
            $scope.preValuesLoaded = false;

            var promiseSave = notificationGatewayProviderResource.updateNotificationMessage($scope.notificationMessage);
            promiseSave.then(function () {
                notificationsService.success("Notification Message Saved");
                eventsService.emit(saveEventName, $scope.notificationMessage);
                
                init();
            }, function (reason) {
                notificationsService.error("Notification Message Save Failed", reason.message);
            });
        }

        // Initialize the controller
        init();

    }]);
