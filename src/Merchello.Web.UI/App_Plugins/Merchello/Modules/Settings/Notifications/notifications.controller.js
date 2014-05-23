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

        $scope.emailTemplates = [];
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
         * Helper method to get a provider from the paymentGatewayProviders array using the provider key passed in.
         */
        $scope.getProviderByKey = function (providerkey) {
            return _.find($scope.notificationGatewayProviders, function (gatewayprovider) { return gatewayprovider.key == providerkey; });
        }

        /**
         * @ngdoc method
         * @name loadAllPaymentGatewayProviders
         * @function
         * 
         * @description
         * Load the payment gateway providers from the payment gateway service, then wrap the results
         * in Merchello models and add to the scope via the paymentGatewayProviders collection.
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
         * @name loadPaymentGatewayResources
         * @function
         * 
         * @description
         * Load the payment gateway resources from the payment gateway service, then wrap the results
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
         * @name loadPaymentMethods
         * @function
         * 
         * @description
         * Load the payment gateway methods from the payment gateway service, then wrap the results
         * in Merchello models and add to the provider in the methods collection.
         */
        $scope.loadNotificationMethods = function (providerKey) {

            var provider = $scope.getProviderByKey(providerKey);

            var promiseAllResources = merchelloNotificationsService.GetNotificationProviderNotificationMethods(providerKey);
            promiseAllResources.then(function (allMethods) {

                provider.methods = _.map(allMethods, function (methodFromServer) {
                    return new merchello.Models.NotificationMethod(methodFromServer);
                });

            }, function (reason) {

                notificationsService.error("Notification Methods Load Failed", reason.message);

            });

        };

        $scope.loadAllNotificationTriggers = function() {
            var promise = merchelloNotificationsService.getAllNotificationTriggers();
            promise.then(function (notificationTriggers) {
                $scope.notificationTriggers = notificationTriggers;
            });
        };

        $scope.loadEmailTemplates = function() {

            var promise = merchelloNotificationsService.getNotificationMessages();
	        promise.then(function(notifications) {
	        	$scope.emailTemplates = _.map(notifications, function (emailTemplateFromServer) {
	        		return new merchello.Models.EmailTemplate(emailTemplateFromServer);
	        	});
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

        $scope.editTemplateFlyout = new merchello.Models.Flyout(
            $scope.flyouts.editTemplate,
            function(isOpen) {
                $scope.flyouts.editTemplate = isOpen;
            },
            {
                clear: function() {
                    var self = $scope.editTemplateFlyout;
                    self.model = new merchello.Models.EmailTemplate();
                },
                confirm: function() {
                    var self = $scope.editTemplateFlyout;
                    // Note From Kyle: An API call will need to be wired in here to edit the existing Email Template in the database.
                    self.clear();
                    self.close();
                }
            });

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
         * @name addCountryDialogConfirm
         * @function
         * 
         * @description
         * Handles the save after recieving the country to add from the dialog view/controller
         */
        $scope.addNotificationsDialogConfirm = function(dialogData) {
            var promiseNotificationMethod = merchelloNotificationsService.saveNotificationMethod(dialogData);

            promiseNotificationMethod.then(function(notificationFromServer) {

                $scope.notificationMethods.push(new merchello.Models.NotificationMethod(notificationFromServer));

            }, function(reason) {

                notificationsService.error("Notification Method Create Failed", reason.message);

            });
        };

        /**
         * @ngdoc method
         * @name addCountry
         * @function
         * 
         * @description
         * Opens the add country dialog via the Umbraco dialogService.
         */
        $scope.addNotificationMethod = function (provider, method) {
            if (method == undefined) {
                method = new merchello.Models.NotificationMethod();
                method.providerKey = provider.key; //Todo: When able to add external providers, make this select the correct provider
                method.serviceCode = "Email";
                method.name = "Email";
            }
            
            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Settings/Notifications/Dialogs/notificationsmethod.html',
                show: true,
                callback: $scope.addNotificationsDialogConfirm,
                dialogData: method
            });
        };

        $scope.loadAllNotificationGatewayProviders();
        $scope.loadEmailTemplates();
        $scope.loadNotificationSubscribers();
        $scope.loadAllNotificationTriggers();

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    };


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.NotificationsController", ['$scope', 'notificationsService', 'dialogService', 'merchelloNotificationsService', merchello.Controllers.NotificationsController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
