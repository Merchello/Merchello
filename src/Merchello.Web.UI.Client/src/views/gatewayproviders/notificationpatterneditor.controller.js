
    angular.module('merchello').controller('Merchello.Backoffice.NotificationPatternEditorController',
    ['$scope', '$q', '$routeParams', '$location', 'assetsService', 'dialogService', 'notificationsService', 'merchelloTabsFactory', 'dialogDataFactory',
    'notificationGatewayProviderResource', 'notificationMessageDisplayBuilder', 'notificationMonitorDisplayBuilder',
    function($scope, $q, $routeParams, $location, assetsService, dialogService, notificationsService, merchelloTabsFactory, dialogDataFactory,
    notificationGatewayProviderResource, notificationMessageDisplayBuilder, notificationMonitorDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.tabs = [];
        $scope.notificationMessage = {};
        $scope.notificationMonitors = [];
        $scope.currentMonitor = {};

        $scope.editorOptions = {
            lineNumbers: true
        }

        $scope.rteProperties = {
            label: 'bodyText',
            view: 'rte',
            config: {
                editor: {
                    toolbar: ["code", "undo", "redo", "cut", "styleselect", "bold", "italic", "alignleft", "aligncenter", "alignright", "bullist", "numlist", "link", "umbmediapicker", "umbmacro", "table", "umbembeddialog"],
                    stylesheets: [],
                    dimensions: { height: 350 }
                }
            },
            value: ""
        };

        // exposed methods
        $scope.save = save;
        $scope.deleteMessage = deleteMessage;

        function init() {
            var key = $routeParams.id;
            $q.all([
                notificationGatewayProviderResource.getNotificationMessagesByKey(key),
                notificationGatewayProviderResource.getAllNotificationMonitors(),
                assetsService.loadCss('/App_Plugins/Merchello/lib/codemirror/codemirror.css')
            ]).then(function(data) {

                /*
                 assetsService.loadJs('/App_Plugins/Merchello/lib/codemirror/codemirror.js'),
                 assetsService.loadJs('/App_Plugins/Merchello/lib/codemirror/ui-codemirror.js'),
                 
                 
                var isInstalled = _.find(angular.module('merchello.plugins').requires, function(mod) {
                   return mod === 'ui.codemirror';
                });

                if (isInstalled === undefined) {
                    angular.module('merchello.plugins').requires.push('ui.codemirror');
                }
                */

                $scope.notificationMessage = notificationMessageDisplayBuilder.transform(data[0]);
                $scope.notificationMonitors = notificationMonitorDisplayBuilder.transform(data[1])

                $scope.currentMonitor = _.find($scope.notificationMonitors, function(m) {
                   return m.monitorKey === $scope.notificationMessage.monitorKey;
                });

                $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
                $scope.tabs.insertTab('messageEditor', 'merchelloTabs_message', '#/merchello/merchello/notification.messageeditor/' + key, 2);
                $scope.tabs.setActive('messageEditor');

                setupEditor();
            });
        }

        function setupEditor() {
            if($scope.currentMonitor.useCodeEditor === false) {

            }
            
            
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
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
                init();
            }, function (reason) {
                notificationsService.error("Notification Message Save Failed", reason.message);
            });
        }

        // Initialize the controller
        init();

    }]);
