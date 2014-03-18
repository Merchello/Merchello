(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Editors.Order.ViewController
     * @function
     * 
     * @description
     * The controller for the order view page
     */
    controllers.OrderViewController = function ($scope, $routeParams, dialogService, notificationsService, merchelloInvoiceService, merchelloOrderService, merchelloPaymentService, merchelloSettingsService) {

        $scope.invoice = {};
        $scope.typeFields = [];
        $scope.shippingAddress = {};

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        $scope.loadTypeFields = function (nextMethodCall) {

            var promise = merchelloSettingsService.getTypeFields();

            promise.then(function (typeFields) {

                $scope.typeFields = _.map(typeFields, function(type) {
                    return new merchello.Models.TypeField(type);
                });

                if (nextMethodCall != undefined) {
                    nextMethodCall();
                }

            }, function (reason) {
                notificationsService.error("TypeFields Load Failed", reason.message);
            });
        };

        $scope.loadInvoice = function (id) {

            var promise = merchelloInvoiceService.getByKey(id);

            promise.then(function (invoice) {

                $scope.invoice = new merchello.Models.Invoice(invoice);

                _.each($scope.invoice.items, function (lineItem) {
                    if (lineItem.lineItemTfKey) {
                        var matchedTypeField = _.find($scope.typeFields, function(type) {
                             return type.typeKey == lineItem.lineItemTfKey;
                        });
                        lineItem.lineItemType = matchedTypeField;
                    }
                });

                $scope.loadShippingAddress($scope.invoice);
                $scope.loadPayments($scope.invoice);

            }, function (reason) {
                notificationsService.error("Invoice Load Failed", reason.message);
            });
        };

        $scope.loadShippingAddress = function (invoice) {

            var promise = merchelloOrderService.getShippingAddress(invoice.key);

            promise.then(function (address) {

                $scope.shippingAddress = new merchello.Models.Address(address);

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.error("Address Load Failed", reason.message);
            });
        };

        $scope.loadPayments = function (invoice) {

            var promise = merchelloPaymentService.getPaymentsByInvoice(invoice.key);

            promise.then(function (payments) {

                invoice.payments = _.map(payments, function (payment) {
                    return new merchello.Models.Payment(payment);
                });

                _.each(invoice.payments, function (payment) {
                    if (payment.paymentTypeFieldKey) {
                        var matchedTypeField = _.find($scope.typeFields, function (type) {
                            return type.typeKey == payment.paymentTypeFieldKey;
                        });
                        payment.paymentType = matchedTypeField;
                    }
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.error("Payments Load Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        $scope.init = function () {

            $scope.loadTypeFields(function () { $scope.loadInvoice($routeParams.id); });
           
        };

        $scope.init();


        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------




        //--------------------------------------------------------------------------------------
        // Dialogs
        //--------------------------------------------------------------------------------------

        $scope.capturePaymentDialogConfirm = function (data) {

            notificationsService.success("Capture Payment Confirm Called");

        };

        $scope.capturePayment = function () {

            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Order/Dialogs/capture.payment.html',
                show: true,
                callback: $scope.capturePaymentDialogConfirm,
                dialogData: $scope.invoice
            });

        };


        $scope.fulfillShipmentDialogConfirm = function (data) {

            notificationsService.success("Fulfill Shipment Confirm Called");

        };

        $scope.fulfillShipment = function () {

            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Order/Dialogs/fulfill.shipment.html',
                show: true,
                callback: $scope.fulfillShipmentDialogConfirm,
                dialogData: $scope.invoice
            });

        };

    };


    angular.module("umbraco").controller("Merchello.Editors.Order.ViewController", merchello.Controllers.OrderViewController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));

