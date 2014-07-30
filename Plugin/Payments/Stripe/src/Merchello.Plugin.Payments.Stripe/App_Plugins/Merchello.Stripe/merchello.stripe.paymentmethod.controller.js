(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.Payment.Dialogs.PaymentMethodController
     * @function
     * 
     * @description
     * The controller for the adding / editing payment methods on the Payment page
     */
    controllers.StripeMethodController = function ($scope) {


    };

    angular.module("umbraco").controller("Merchello.Plugin.GatewayProviders.Payments.Dialogs.StripePaymentMethodController", ['$scope', merchello.Controllers.StripePaymentMethodController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));