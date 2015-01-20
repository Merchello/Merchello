    angular.module('merchello').controller('Merchello.Backoffice.NotificationProvidersController',
        ['$scope', 'notificationsService', 'dialogService', 'merchelloTabsFactory', 'dialogDataFactory', 'gatewayResourceDisplayBuilder',
            'notificationGatewayProviderResource', 'notificationGatewayProviderDisplayBuilder', 'notificationMethodDisplayBuilder',
            function($scope, notificationsService, dialogService, merchelloTabsFactory, dialogDataFactory, gatewayResourceDisplayBuilder,
                     notificationGatewayProviderResource, notificationGatewayProviderDisplayBuilder, notificationMethodDisplayBuilder) {

            $scope.currentTab = "Template";
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.tabs = [];

            $scope.notificationGatewayProviders = [];
            $scope.notificationTriggers = [];
            $scope.notificationMessage = {};
            $scope.notificationMethods = [];

            $scope.subscribers = [];
            $scope.flyouts = {
                editTemplate: false,
                addAddress: false,
                deleteAddress: false
            };

            function init() {
                loadAllNotificationGatewayProviders();
                //loadAllNotificationTriggers();
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
            function loadAllNotificationTriggers() {
                var promise = merchelloNotificationsService.getAllNotificationTriggers();
                promise.then(function (notificationTriggers) {
                    $scope.notificationTriggers = notificationTriggers;
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
                    console.info(provider);
                }, function (reason) {
                    notificationsService.error("Notification Methods Load Failed", reason.message);
                });
            }

            /*
            $scope.addAddressFlyout = new merchello.Models.Flyout(
                $scope.flyouts.addAddress,
                function(isOpen) {
                    $scope.flyouts.addAddress = isOpen;
                },
                {
                    clear: function() {
                        var self = $scope.addAddressFlyout;
                        self.model = new merchello.Models.NotificationSubscriber();
                    },
                    confirm: function() {
                        var self = $scope.addAddressFlyout;
                        var newKey = $scope.subscribers.length;
                        // Note From Kyle: This key-creation logic will need to be modified to fit whatever works for the database.
                        self.model.pk = newKey;
                        $scope.subscribers.push(self.model);
                        // Note From Kyle: An API call will need to be wired in here to add the new Subscriber to the email notification list in the database.
                        self.clear();
                        self.close();
                    }
                });


            $scope.deleteAddressFlyout = new merchello.Models.Flyout(
                $scope.flyouts.deleteAddress,
                function(isOpen) {
                    $scope.flyouts.deleteAddress = isOpen;
                },
                {
                    clear: function() {
                        var self = $scope.deleteAddressFlyout;
                        self.model = new merchello.Models.NotificationSubscriber();
                    },
                    confirm: function() {
                        var self = $scope.deleteAddressFlyout;
                        var idx = -1;
                        for (i = 0; i < $scope.subscribers.length; i++) {
                            if ($scope.subscribers[i].pk == self.model.pk) {
                                idx = i;
                            }
                        }
                        if (idx > -1) {
                            $scope.subscribers.splice(idx, 1);
                            // Note From Kyle: An API call will need to be wired in here to delete the subscriber from the notification list in the database.
                        }
                        self.clear();
                        self.close();
                    }
                });

             */

            /**
             * @ngdoc method
             * @name addNotificationMessageToMethodClick
             * @function
             *
             * @description
             * Saves a Notification Message to the Notification Method.
             */
            function addNotificationMessageToMethodClick(methodKey, notificationMessage) {
                var redirectKey = "create";
                if (notificationMessage == undefined) {
                    notificationMessage = new merchello.Models.NotificationMessage();
                    notificationMessage.methodKey = methodKey;
                }
                else {
                    redirectKey = notificationMessage.key;
                }
                $scope.notificationMessage = notificationMessage;
                window.location.hash = "#/merchello/merchello/NotificationsEdit/" + redirectKey;
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
                var promiseNotificationMethod = merchelloNotificationsService.saveNotificationMethod(dialogData);

                promiseNotificationMethod.then(function(notificationFromServer) {
                    $scope.notificationMethods.push(new merchello.Models.NotificationMethod(notificationFromServer));
                    location.reload();
                    notificationsService.success("Notification Method Created!", "");
                }, function(reason) {

                    notificationsService.error("Notification Method Create Failed", reason.message);

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
            function addNotificationMethod(provider, method) {
                if (method == undefined) {
                    method = new merchello.Models.NotificationMethod();
                    method.providerKey = provider.key; //Todo: When able to add external providers, make this select the correct provider
                    method.serviceCode = "Email";
                    method.name = "Email";
                }
                method.resources = provider.resources;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Modules/Settings/Notifications/Dialogs/notificationsmethod.html',
                    show: true,
                    callback: $scope.addNotificationsDialogConfirm,
                    dialogData: method
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
                var promiseNotificationMethod = merchelloNotificationsService.deleteNotificationMethod(dialogData.key);

                promiseNotificationMethod.then(function () {
                    location.reload();
                    notificationsService.success("Notification Deleted");
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
                dialogService.open({
                    template: '/App_Plugins/Merchello/Modules/Settings/Notifications/Dialogs/notificationsdelete.html',
                    show: true,
                    callback: $scope.notificationsMethodDeleteDialogConfirm,
                    dialogData: method
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
                var promiseNotificationMethod = merchelloNotificationsService.deleteNotificationMessage(dialogData.key);

                promiseNotificationMethod.then(function () {
                    notificationsService.success("Notification Deleted");
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
            function deleteNotificationMessage(method) {
                dialogService.open({
                    template: '/App_Plugins/Merchello/Modules/Settings/Notifications/Dialogs/notificationsdelete.html',
                    show: true,
                    callback: $scope.notificationsMessageDeleteDialogConfirm,
                    dialogData: method
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
            function notificationsMessageAddDialogConfirm(message) {
                message.monitorKey = message.monitorKey.monitorKey;
                var promiseNotificationMethod = merchelloNotificationsService.saveNotificationMessage(message);

                promiseNotificationMethod.then(function (keyFromServer) {
                    notificationsService.success("Notification Saved", "");
                    location.reload();
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
                var dialogData = new merchello.Models.NotificationMessage();
                dialogData.methodKey = method.key;
                dialogData.notificationTriggers = $scope.notificationTriggers;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Modules/Settings/Notifications/Dialogs/notificationsmessage.html',
                    show: true,
                    callback: $scope.notificationsMessageAddDialogConfirm,
                    dialogData: dialogData
                });
            }

                // Initialize the controller
                init();
    }]);
