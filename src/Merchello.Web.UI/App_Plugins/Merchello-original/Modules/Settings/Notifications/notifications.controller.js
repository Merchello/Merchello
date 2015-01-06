(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.NotificationsController
     * @function
     * 
     * @description
     * The controller for the Notifications page
     */
    controllers.NotificationsController = function ($scope, notificationsService, dialogService, merchelloNotificationsService) {
        $scope.currentTab = "Template";

        $scope.notificationGatewayProviders = [];
        $scope.notificationTriggers = [];
        $scope.notificationMessage = new merchello.Models.NotificationMessage();
        $scope.notificationMethods = [];

        $scope.subscribers = [];
        $scope.flyouts = {
            editTemplate: false,
            addAddress: false,
            deleteAddress: false
        };

        /**
         * @ngdoc method
         * @name getProviderByKey
         * @function
         * 
         * @description
         * Helper method to get a provider from the notificationGatewayProviders array using the provider key passed in.
         */
        $scope.getProviderByKey = function (providerkey) {
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
        $scope.loadAllNotificationGatewayProviders = function () {

            var promiseAllProviders = merchelloNotificationsService.getAllGatewayProviders();
            promiseAllProviders.then(function (allProviders) {

                $scope.notificationGatewayProviders = _.map(allProviders, function (providerFromServer) {
                    return new merchello.Models.NotificationGatewayProvider(providerFromServer);
                });

                _.each($scope.notificationGatewayProviders, function (provider) {
                    $scope.loadNotificationGatewayResources(provider.key);
                    $scope.loadNotificationMethods(provider.key);
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {

                notificationsService.error("Available Notification Providers Load Failed", reason.message);

            });

        };

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
        $scope.loadNotificationGatewayResources = function (providerKey) {

            var provider = $scope.getProviderByKey(providerKey);

            var promiseAllResources = merchelloNotificationsService.getGatewayResources(provider.key);
            promiseAllResources.then(function (allResources) {

                provider.resources = _.map(allResources, function (resourceFromServer) {
                    return new merchello.Models.GatewayResource(resourceFromServer);
                });

                if (provider.resources.length > 0) {
                    provider.selectedResource = provider.resources[0];
                }

            }, function (reason) {

                notificationsService.error("Available Notification Provider Resources Load Failed", reason.message);

            });

        };

        /**
         * @ngdoc method
         * @name loadAllNotificationTriggers
         * @function
         * 
         * @description
         * Loads the triggers for the notification messages.
         */
        $scope.loadAllNotificationTriggers = function () {
            var promise = merchelloNotificationsService.getAllNotificationTriggers();
            promise.then(function (notificationTriggers) {
                $scope.notificationTriggers = notificationTriggers;
            });
        };

        /**
         * @ngdoc method
         * @name loadNotificationMethods
         * @function
         * 
         * @description
         * Load the notification gateway methods from the notification gateway service, then wrap the results
         * in Merchello models and add to the provider in the methods collection.
         */
        $scope.loadNotificationMethods = function (providerKey) {

            var provider = $scope.getProviderByKey(providerKey);

            var promiseAllResources = merchelloNotificationsService.getNotificationProviderNotificationMethods(providerKey);
            promiseAllResources.then(function (allMethods) {

                provider.methods = _.map(allMethods, function (methodFromServer) {
                    return new merchello.Models.NotificationMethod(methodFromServer);
                });

            }, function (reason) {

                notificationsService.error("Notification Methods Load Failed", reason.message);

            });

        };

        $scope.loadNotificationSubscribers = function() {

            // Note From Kyle: A mock of getting the email subscribers objects.
            var mockSubscribers = [
                {
                    pk: 0,
                    email: "janae@mindfly.com"
                },
                {
                    pk: 1,
                    email: "heather@mindfly.com"
                },
                {
                    pk: 2,
                    email: "rusty@mindfly.com"
                }
            ];
            $scope.subscribers = _.map(mockSubscribers, function(notificationSubscriberFromServer) {
                return new merchello.Models.NotificationSubscriber(notificationSubscriberFromServer);
            });
            // End of Mocks

        };

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

        /**
         * @ngdoc method
         * @name addNotificationMessageToMethodClick
         * @function
         * 
         * @description
         * Saves a Notification Message to the Notification Method.
         */
        $scope.addNotificationMessageToMethodClick = function (methodKey, notificationMessage) {
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
        $scope.addNotificationsDialogConfirm = function(dialogData) {
            var promiseNotificationMethod = merchelloNotificationsService.saveNotificationMethod(dialogData);

            promiseNotificationMethod.then(function(notificationFromServer) {
                $scope.notificationMethods.push(new merchello.Models.NotificationMethod(notificationFromServer));
                location.reload();
                notificationsService.success("Notification Method Created!", "");
            }, function(reason) {

                notificationsService.error("Notification Method Create Failed", reason.message);

            });
        };

        /**
        * @ngdoc method
        * @name addNotificationMethod
        * @function
        * 
        * @description
        * Opens the add notification method dialog via the Umbraco dialogService.
        */
        $scope.addNotificationMethod = function (provider, method) {
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
        };

        /**
         * @ngdoc method
         * @name notificationsMethodDeleteDialogConfirm
         * @function
         * 
         * @description
         * Handles the delete after recieving the deleted command from the dialog view/controller
         */
        $scope.notificationsMethodDeleteDialogConfirm = function (dialogData) {
            var promiseNotificationMethod = merchelloNotificationsService.deleteNotificationMethod(dialogData.key);

            promiseNotificationMethod.then(function () {
                location.reload();
                notificationsService.success("Notification Deleted");
            }, function (reason) {

                notificationsService.error("Notification Method Deletion Failed", reason.message);

            });
        };

        /**
         * @ngdoc method
         * @name deleteNotificationMethod
         * @function
         * 
         * @description
         * Opens the delete dialog via the Umbraco dialogService
         */
        $scope.deleteNotificationMethod = function (method) {
            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Settings/Notifications/Dialogs/notificationsdelete.html',
                show: true,
                callback: $scope.notificationsMethodDeleteDialogConfirm,
                dialogData: method
            });
        };

        /**
         * @ngdoc method
         * @name notificationsMessageDeleteDialogConfirm
         * @function
         * 
         * @description
         * Handles the delete after recieving the deleted command from the dialog view/controller
         */
        $scope.notificationsMessageDeleteDialogConfirm = function (dialogData) {
            var promiseNotificationMethod = merchelloNotificationsService.deleteNotificationMessage(dialogData.key);

            promiseNotificationMethod.then(function () {
                notificationsService.success("Notification Deleted");
            }, function (reason) {

                notificationsService.error("Notification Method Deletion Failed", reason.message);

            });
        };
        /**
         * @ngdoc method
         * @name deleteNotificationMessage
         * @function
         * 
         * @description
         * Opens the delete dialog via the Umbraco dialogService
         */
        $scope.deleteNotificationMessage = function (method) {
            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Settings/Notifications/Dialogs/notificationsdelete.html',
                show: true,
                callback: $scope.notificationsMessageDeleteDialogConfirm,
                dialogData: method
            });
        };
        
        /**
        * @ngdoc method
        * @name notificationsMessageAddDialogConfirm
        * @function
        * 
        * @description
        * Handles the save after recieving the save command from the dialog view/controller
        */
        $scope.notificationsMessageAddDialogConfirm = function (message) {
            message.monitorKey = message.monitorKey.monitorKey;
            var promiseNotificationMethod = merchelloNotificationsService.saveNotificationMessage(message);

            promiseNotificationMethod.then(function (keyFromServer) {
                notificationsService.success("Notification Saved", "");
                location.reload();
            }, function (reason) {

                notificationsService.error("Notification Message Saved Failed", reason.message);

            });
        };

        /**
         * @ngdoc method
         * @name addNotificationMessage
         * @function
         * 
         * @description
         * Opens the add notification dialog via the Umbraco dialogService
         */
        $scope.addNotificationMessage = function(method) {
            var dialogData = new merchello.Models.NotificationMessage();
            dialogData.methodKey = method.key;
            dialogData.notificationTriggers = $scope.notificationTriggers;
            dialogService.open({
                    template: '/App_Plugins/Merchello/Modules/Settings/Notifications/Dialogs/notificationsmessage.html',
                    show: true,
                    callback: $scope.notificationsMessageAddDialogConfirm,
                    dialogData: dialogData
            });
        };

        $scope.loadAllNotificationGatewayProviders();
        $scope.loadNotificationSubscribers();
        $scope.loadAllNotificationTriggers();

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    };


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.NotificationsController", ['$scope', 'notificationsService', 'dialogService', 'merchelloNotificationsService', merchello.Controllers.NotificationsController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
