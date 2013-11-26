(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.NotificationsController
     * @function
     * 
     * @description
     * The controller for the Notifications page
     */
    controllers.NotificationsController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.emailTemplates = [];
        $scope.flyouts = {
            editTemplate: false
        };

        $scope.loadEmailTemplates = function () {

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
            $scope.emailTemplates = _.map(mockTemplates, function (emailTemplateFromServer) {
                return new merchello.Models.EmailTemplate(emailTemplateFromServer);
            });
            // End of Mocks

        };

        $scope.editTemplateFlyout = new merchello.Models.Flyout(
            $scope.flyouts.editTemplate,
            function (isOpen) {
                $scope.flyouts.editTemplate = isOpen;
            },
            {
                clear: function () {
                    self.model = new merchello.Models.EmailTemplate();
                },
                confirm: function () {
                    var self = $scope.editTemplateFlyout;
                    // Note From Kyle: An API call will need to be wired in here to edit the existing Email Template in the database.
                    self.clear();
                    self.close();
                }
            });

        $scope.loadEmailTemplates();

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    }


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.NotificationsController", merchello.Controllers.NotificationsController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
