    /**
     * @ngdoc controller
     * @name Merchello.Editors.Sales.OverviewController
     * @function
     *
     * @description
     * The controller for the sales overview page
     */
    angular.module('merchello').controller('Merchello.Dashboards.SalesOverviewController',
        ['$scope', '$routeParams', 'assetsService', 'dialogService', 'localizationService', 'notificationsService',
            'auditLogResource', 'invoiceResource', 'paymentResource', 'shipmentResource', 'settingsResource', 'salesHistoryDisplayBuilder',
            'paymentDisplayBuilder', 'invoiceDisplayBuilder',
        function($scope, $routeParams, assetsService, dialogService, localizationService, notificationsService,
                 auditLogResource, invoiceResource, paymentResource, shipmentResource, settingsResource, salesHistoryDisplayBuilder,
                 paymentDisplayBuilder, invoiceDisplayBuilder) {

            $scope.historyLoaded = false;
            $scope.invoice = {};
            $scope.currencySymbol = '';
            $scope.salesHistory = {};
            // exposed methods
            $scope.capturePayment = capturePayment;
            $scope.openDeleteInvoiceDialog = openDeleteInvoiceDialog;
            $scope.openFulfillShipmentDialog = openFulfillShipmentDialog;
            $scope.localizeMessage = localizeMessage;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description - Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init () {
                loadInvoice($routeParams.id);
                loadSettings();
                loadAuditLog($routeParams.id);
                $scope.loaded = true;
            };

            function localizeMessage(msg) {
                return msg.localize(localizationService);
            }

            /**
             * @ngdoc method
             * @name loadAuditLog
             * @function
             *
             * @description
             * Load the Audit Log for the invoice via API.
             */
            function loadAuditLog(key) {
                if (key !== undefined) {
                    var promise = auditLogResource.getSalesHistoryByInvoiceKey(key);
                    promise.then(function (response) {
                        var history = salesHistoryDisplayBuilder.transform(response);
                        // TODO this is a patch for a problem in the API
                        if (history.dailyLogs.length) {
                            $scope.salesHistory = history.dailyLogs;
                        }
                        $scope.historyLoaded = history.dailyLogs.length > 0;
                    }, function (reason) {
                        notificationsService.error('Failed to load sales history', reason.message);
                    });
                }
            };

            /**
             * @ngdoc method
             * @name loadInvoice
             * @function
             *
             * @description - Load an invoice with the associated id.
             */
            function loadInvoice(id) {
                var promise = invoiceResource.getByKey(id);
                promise.then(function (invoice) {
                    $scope.invoice = invoiceDisplayBuilder.transform(invoice);

                }, function (reason) {
                    notificationsService.error("Invoice Load Failed", reason.message);
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            };


            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                }, function (reason) {
                    alert('Failed: ' + reason.message);
                });
            };

            /*-------------------------------------------------------------------
             * Event Handler Methods
             *-------------------------------------------------------------------*/

            /**
             * @ngdoc method
             * @name capturePayment
             * @function
             *
             * @description - Open the capture shipment dialog.
             */
            function capturePayment() {
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/capture.payment.html',
                    show: true,
                    callback: capturePaymentDialogConfirm,
                    dialogData: $scope.invoice
                });
            };

            /**
             * @ngdoc method
             * @name capturePaymentDialogConfirm
             * @function
             *
             * @description - Capture the payment after the confirmation dialog was passed through.
             */
            function capturePaymentDialogConfirm(paymentRequest) {
                var promiseSave = paymentRequest.capturePayment(paymentRequest);
                promiseSave.then(function (payment) {
                    notificationsService.success("Payment Captured");
                    $scope.loadInvoice(paymentRequest.invoiceKey);
                }, function (reason) {
                    notificationsService.error("Payment Capture Failed", reason.message);
                });
            };

            /**
             * @ngdoc method
             * @name openDeleteInvoiceDialog
             * @function
             *
             * @description - Open the delete payment dialog.
             */
            function openDeleteInvoiceDialog() {
                var dialogData = {};
                dialogData.name = 'Invoice #' + $scope.invoice.invoiceNumber;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
                    show: true,
                    callback: processDeleteInvoiceDialog,
                    dialogData: dialogData
                });
            };

            /**
             * @ngdoc method
             * @name openFulfillShipmentDialog
             * @function
             *
             * @description - Open the fufill shipment dialog.
             */
            function openFulfillShipmentDialog() {
                console.info($scope.invoice);
                dialogService.open({
                    template: '/App_Plugins/Merchello/Modules/Order/Dialogs/fulfill.shipment.html',
                    show: true,
                    callback: processFulfillShipmentDialog,
                    dialogData: $scope.invoice.orders[0]    // todo: pull from current order when multiple orders is available
                });
            };

            /**
             * @ngdoc method
             * @name processDeleteInvoiceDialog
             * @function
             *
             * @description - Delete the invoice.
             */
            function processDeleteInvoiceDialog() {
                var promiseDeleteInvoice = invoiceResource.deleteInvoice($scope.invoice.key);
                promiseDeleteInvoice.then(function (response) {
                    notificationsService.success('Invoice Deleted');
                    window.location.href = '#/merchello/merchello/invoicelist/manage';
                }, function (reason) {
                    notificationsService.error('Failed to Delete Invoice', reason.message);
                });
            };

            /**
             * @ngdoc method
             * @name processFulfillPaymentDialog
             * @function
             *
             * @description - Process the fulfill shipment functionality on callback from the dialog service.
             */
            function processFulfillShipmentDialog(data) {
                var promiseNewShipment = shipmentResource.newShipment(data);
                promiseNewShipment.then(function (shipment) {
                    // TODO this is a total hack.  A new model should be defined.
                    shipment.trackingCode = data.trackingNumber;
                    shipment.shipmentStatus = data.shipmentStatus;
                    var promiseSave = shipmentResource.putShipment(shipment, data);
                    promiseSave.then(function () {
                        notificationsService.success("Shipment Created");

                        // todo - this needs to be done outside of the promise
                        $scope.loadInvoice(data.invoiceKey);

                    }, function (reason) {
                        notificationsService.error("Save Shipment Failed", reason.message);
                    });
                }, function (reason) {
                    notificationsService.error("New Shipment Failed", reason.message);
                });
            };

            /*-------------------------------------------------------------------*/

            init();
    }]);
