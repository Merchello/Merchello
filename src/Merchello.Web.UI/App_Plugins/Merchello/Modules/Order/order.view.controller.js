(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Editors.Order.ViewController
     * @function
     * 
     * @description
     * The controller for the order view page
     */
    controllers.OrderViewController = function ($scope, $routeParams, dialogService, notificationsService, merchelloInvoiceService) {

        $scope.invoice = {};

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        $scope.loadInvoice = function (id) {

            var promise = merchelloInvoiceService.getByKey(id);

            promise.then(function (invoice) {

                $scope.invoice = new merchello.Models.Invoice(invoice);

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.error("Invoice Load Failed", reason.message);
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

            $scope.loadInvoice($routeParams.id);

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

