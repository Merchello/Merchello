
    angular.module('merchello').controller('Merchello.Backoffice.NotificationMessageEditorController',
    ['$scope', '$routeParams', 'dialogService', 'notificationsService', 'merchelloTabsFactory',
    'notificationGatewayProviderResource', 'notificationMessageDisplayBuilder',
    function($scope, $routeParams, dialogService, notificationsService, merchelloTabsFactory,
    notificationGatewayProviderResource, notificationMessageDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.tabs = [];
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

        function init() {
            var key = $routeParams.id;
            loadNotificationMessage(key);
            loadAllNotificationMonitors();
            $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
            $scope.tabs.insertTab('messageEditor', 'Message', '#/merchello/merchello/notification.messageeditor/' + key, 2);
            $scope.tabs.setActive('messageEditor');
        }

        /**
         * @ngdoc method
         * @name loadNotificationMessage
         * @function
         *
         * @description
         * Loads all of the Notification Message
         */
        function loadNotificationMessage(key) {
            var promise = notificationGatewayProviderResource.getNotificationMessagesByKey(key);
            promise.then(function (notification) {
                $scope.notificationMessage = notificationMessageDisplayBuilder.transform(notification);
                $scope.rteProperties.value = notification.bodyText;
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            });
            return promise;
        }

        /**
         * @ngdoc method
         * @name loadAllNotificationMonitors
         * @function
         *
         * @description
         * Loads all of the Notification Monitors
         */
        function loadAllNotificationMonitors() {
            var promise = notificationGatewayProviderResource.getAllNotificationMonitors();
            promise.then(function (monitors) {
                $scope.notificationTriggers = notificationMessageDisplayBuilder.transform(monitors);
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
            $scope.notificationMessage.bodyText = $scope.rteProperties.value;
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
