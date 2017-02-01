    angular.module('merchello').controller('Merchello.Backoffice.NotificationProvidersController',
        ['$scope', '$location', 'notificationsService', 'dialogService', 'merchelloTabsFactory', 'dialogDataFactory', 'gatewayResourceDisplayBuilder',
        'notificationGatewayProviderResource', 'notificationGatewayProviderDisplayBuilder', 'notificationMethodDisplayBuilder',
        'notificationMonitorDisplayBuilder', 'notificationMessageDisplayBuilder',
        function($scope, $location, notificationsService, dialogService, merchelloTabsFactory, dialogDataFactory, gatewayResourceDisplayBuilder,
        notificationGatewayProviderResource, notificationGatewayProviderDisplayBuilder, notificationMethodDisplayBuilder,
        notificationMonitorDisplayBuilder, notificationMessageDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.notificationMonitors = [];
            $scope.tabs = [];

            $scope.notificationGatewayProviders = [];

            // exposed methods
            $scope.addNotificationMethod = addNotificationMethod;
            $scope.editNotificationMethod = editNotificationMethod;
            $scope.deleteNotificationMethod = deleteNotificationMethod;
            $scope.addNotificationMessage = addNotificationMessage;
            $scope.deleteNotificationMessage = deleteNotificationMessage;
            
            $scope.goToEditor = goToEditor;

            function init() {
                loadAllNotificationGatewayProviders();
                loadAllNotificationMonitors();
                $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
                $scope.tabs.setActive('notification');
            }

            /**
             * @ngdoc method
             * @name getProviderByKey
             * @function
             *
             * @description
             * Helper method to get a provider from the notificationGatewayProviders array using the provider key passed in.
             */
            function getProviderByKey(providerkey) {
                return _.find($scope.notificationGatewayProviders, function (gatewayprovider) { return gatewayprovider.key == providerkey; });
            }

            /**
             * @ngdoc method
             * @name loadAllNotificationGatewayProviders
             * @function
             *
             * @description
             * Load the notification gateway providers from the notification gateway service, then wrap the results
             * in Merchello models and add to the scope via the notificationGatewayProviders collection.
             */
            function loadAllNotificationGatewayProviders() {
                var promiseAllProviders = notificationGatewayProviderResource.getAllGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.notificationGatewayProviders = notificationGatewayProviderDisplayBuilder.transform(allProviders);
                    angular.forEach($scope.notificationGatewayProviders, function(provider) {
                        loadNotificationGatewayResources(provider.key);
                        loadNotificationMethods(provider.key);
                    });
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Notification Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadNotificationGatewayResources
             * @function
             *
             * @description
             * Load the notification gateway resources from the notification gateway service, then wrap the results
             * in Merchello models and add to the provider in the resources collection.  This will only
             * return resources that haven't already been added via other methods on the provider.
             */
            function loadNotificationGatewayResources(providerKey) {
                var provider = getProviderByKey(providerKey);
                var promiseAllResources = notificationGatewayProviderResource.getGatewayResources(provider.key);
                promiseAllResources.then(function (allResources) {
                    provider.gatewayResources = gatewayResourceDisplayBuilder.transform(allResources);
                    if (provider.gatewayResources.length > 0) {
                        provider.selectedGatewayResource = provider.gatewayResources[0];
                    }
                }, function (reason) {
                    notificationsService.error("Available Notification Provider Resources Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllNotificationTriggers
             * @function
             *
             * @description
             * Loads the triggers for the notification messages.
             */
            function loadAllNotificationMonitors() {
                var promise = notificationGatewayProviderResource.getAllNotificationMonitors();
                promise.then(function (notificationMonitors) {
                    $scope.notificationMonitors = notificationMonitorDisplayBuilder.transform(notificationMonitors);
                });
            }

            /**
             * @ngdoc method
             * @name loadNotificationMethods
             * @function
             *
             * @description
             * Load the notification gateway methods from the notification gateway service, then wrap the results
             * in Merchello models and add to the provider in the methods collection.
             */
            function loadNotificationMethods(providerKey) {

                var provider = getProviderByKey(providerKey);
                var promiseAllResources = notificationGatewayProviderResource.getNotificationProviderNotificationMethods(providerKey);
                promiseAllResources.then(function (allMethods) {
                    provider.notificationMethods = notificationMethodDisplayBuilder.transform(allMethods);
                }, function (reason) {
                    notificationsService.error("Notification Methods Load Failed", reason.message);
                });
            }

            //--------------------------------------------------------------------------------------
            // Dialog methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name addNotificationsDialogConfirm
             * @function
             *
             * @description
             * Handles the save after recieving the notification to add from the dialog view/controller
             */
            function addNotificationsDialogConfirm(dialogData) {
                $scope.preValuesLoaded = false;
                var promiseNotificationMethod = notificationGatewayProviderResource.addNotificationMethod(dialogData.notificationMethod);
                promiseNotificationMethod.then(function(notificationFromServer) {
                    notificationsService.success("Notification Method saved.", "");
                    init();
                }, function(reason) {
                    notificationsService.error("Notification Method save Failed", reason.message);
                });
            }

            function saveNotificationDialogConfirm(dialogData) {
                $scope.preValuesLoaded = false;
                var promiseNotificationMethod = notificationGatewayProviderResource.saveNotificationMethod(dialogData.notificationMethod);
                promiseNotificationMethod.then(function(notificationFromServer) {
                    notificationsService.success("Notification Method saved.", "");
                    init();
                }, function(reason) {
                    notificationsService.error("Notification Method save Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name addNotificationMethod
             * @function
             *
             * @description
             * Opens the add notification method dialog via the Umbraco dialogService.
             */
            function addNotificationMethod(provider, resource) {
                var dialogData = dialogDataFactory.createAddEditNotificationMethodDialogData();
                var method = notificationMethodDisplayBuilder.createDefault();
                method.name = resource.name;
                method.serviceCode = resource.serviceCode;
                method.providerKey = provider.key;
                dialogData.notificationMethod = method;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/notification.notificationmethod.addedit.html',
                    show: true,
                    callback: addNotificationsDialogConfirm,
                    dialogData: dialogData
                });
            }

            function editNotificationMethod(method) {
                var dialogData = {
                    notificationMethod: angular.extend(notificationMethodDisplayBuilder.createDefault(), method)
                };

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/notification.notificationmethod.addedit.html',
                    show: true,
                    callback: saveNotificationDialogConfirm,
                    dialogData: dialogData
                });
            }



            /**
             * @ngdoc method
             * @name notificationsMethodDeleteDialogConfirm
             * @function
             *
             * @description
             * Handles the delete after recieving the deleted command from the dialog view/controller
             */
            function notificationsMethodDeleteDialogConfirm(dialogData) {
                $scope.preValuesLoaded = false;
                var promiseNotificationMethod = notificationGatewayProviderResource.deleteNotificationMethod(dialogData.notificationMethod.key);
                promiseNotificationMethod.then(function () {
                    notificationsService.success("Notification Deleted");
                    init();
                }, function (reason) {
                    notificationsService.error("Notification Method Deletion Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deleteNotificationMethod
             * @function
             *
             * @description
             * Opens the delete dialog via the Umbraco dialogService
             */
            function deleteNotificationMethod(method) {
                var dialogData = dialogDataFactory.createDeleteNotificationMethodDialogData();
                dialogData.notificationMethod = method;
                dialogData.name = method.name;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: notificationsMethodDeleteDialogConfirm,
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
                    notificationsService.success("Notification Deleted");
                    init();
                }, function (reason) {
                    notificationsService.error("Notification Method Deletion Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deleteNotificationMessage
             * @function
             *
             * @description
             * Opens the delete dialog via the Umbraco dialogService
             */
            function deleteNotificationMessage(message) {
                var dialogData = dialogDataFactory.createDeleteNotificationMessageDialogData();
                dialogData.notificationMessage = message;
                dialogData.name = message.name;

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: notificationsMessageDeleteDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name notificationsMessageAddDialogConfirm
             * @function
             *
             * @description
             * Handles the save after recieving the save command from the dialog view/controller
             */
            function notificationsMessageAddDialogConfirm(dialogData) {
                var promiseNotificationMethod = notificationGatewayProviderResource.saveNotificationMessage(dialogData.notificationMessage);
                promiseNotificationMethod.then(function (keyFromServer) {
                    notificationsService.success("Notification Saved", "");
                    init();
                }, function (reason) {
                    notificationsService.error("Notification Message Saved Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name addNotificationMessage
             * @function
             *
             * @description
             * Opens the add notification dialog via the Umbraco dialogService
             */
            function addNotificationMessage(method) {
                var dialogData = dialogDataFactory.createAddEditNotificationMessageDialogData();
                dialogData.notificationMessage = notificationMessageDisplayBuilder.createDefault();

                dialogData.notificationMessage.methodKey = method.key;
                dialogData.notificationMessage.name = method.name;
                dialogData.notificationMonitors = $scope.notificationMonitors;
                dialogData.selectedMonitor = $scope.notificationMonitors[0];
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/notification.notificationmessage.add.html',
                    show: true,
                    callback: notificationsMessageAddDialogConfirm,
                    dialogData: dialogData
                });
            }

            
            function goToEditor(message) {
                var monitor = _.find($scope.notificationMonitors, function(m) {
                    return m.monitorKey === message.monitorKey;
                });

                if (monitor !== undefined) {
                    $location.url('merchello/merchello/notificationmessageeditor/' + message.key, true);
                }
            }
            
            // Initialize the controller
            init();
    }]);
