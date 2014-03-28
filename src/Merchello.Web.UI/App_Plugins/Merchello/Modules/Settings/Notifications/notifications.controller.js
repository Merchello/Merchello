(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.NotificationsController
     * @function
     * 
     * @description
     * The controller for the Notifications page
     */
    controllers.NotificationsController = function($scope) {

        $scope.emailTemplates = [];
        $scope.subscribers = [];
        $scope.flyouts = {
            editTemplate: false,
            addAddress: false,
            deleteAddress: false
        };

        $scope.loadEmailTemplates = function() {

            // Note From Kyle: A mock of getting the Email Template objects.
            var mockTemplates = [
                {
                    pk: 0,
                    name: "Order Confirmation",
                    description: "Sent to the customers and users requesting notification (set below).",
                    header: "Thank you for ordering through Geeky Soap! We're so excited to share our geekiness with you. Enjoy your products and if you have any questions, call our customer service line at 1-888-531-1234!",
                    footer: "XOXO, <br/> The Geeky Soap Team"
                },
                {
                    pk: 1,
                    name: "Order Shipped",
                    description: "Sent to the customer upon order fulfillment.",
                    header: "",
                    footer: ""
                },
                {
                    pk: 2,
                    name: "Problems with Payment Auth",
                    description: "Sent with request to contact support when credit card is denied or there is an error such as wrong billing address.",
                    header: "",
                    footer: ""
                },
                {
                    pk: 3,
                    name: "Payment Received",
                    description: "Sent to customers and users requesting notification upon payment processing.",
                    header: "",
                    footer: ""
                },
                {
                    pk: 4,
                    name: "Order Canceled",
                    description: "Sent to the customer after an order is manually canceled.",
                    header: "",
                    footer: ""
                }
            ];
            $scope.emailTemplates = _.map(mockTemplates, function(emailTemplateFromServer) {
                return new merchello.Models.EmailTemplate(emailTemplateFromServer);
            });
            // End of Mocks

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

        $scope.loadEmailTemplates();
        $scope.loadNotificationSubscribers();

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    };


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.NotificationsController", ['$scope', merchello.Controllers.NotificationsController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
