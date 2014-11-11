(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Editors.Order.ViewController
     * @function
     * 
     * @description
     * The controller for the order view page
     */
    controllers.OrderViewController = function ($scope, $routeParams, assetsService, dialogService, localizationService, notificationsService, merchelloAuditService, merchelloInvoiceService, merchelloOrderService, merchelloPaymentService, merchelloShipmentService, merchelloSettingsService) {

        /*-------------------------------------------------------------------
         * Initialization Methods
         * ------------------------------------------------------------------*/

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description - Method called on intial page load.  Loads in data from server and sets up scope.
         */
        $scope.init = function () {
            $scope.setVariables();
            $scope.loadTypeFields(function () { $scope.loadInvoice($routeParams.id); });
            $scope.loadSettings();
        };

        /**
         * @ngdoc method
         * @name loadAuditLog
         * @function
         * 
         * @description
         * Load the Audit Log for the invoice via API.
         */
        $scope.loadAuditLog = function (key) {
            if (key !== undefined) {
                var promise = merchelloAuditService.getSalesHistoryByInvoiceKey(key);
                promise.then(function (response) {
                    if (response.dailyLogs) {
                        if (response.dailyLogs.length > 0) {
                            var dailyLogs = _.map(response.dailyLogs, function (log) {
                                return new merchello.Models.DailyLog(log);
                            });
                            $scope.buildLocalizedShippingHistory(dailyLogs);
                        }
                    }
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
        $scope.loadInvoice = function (id) {
            var promise = merchelloInvoiceService.getByKey(id);
            promise.then(function (invoice) {
                console.info(invoice);
                $scope.invoice = new merchello.Models.Invoice(invoice);
                console.info($scope.invoice);
                _.each($scope.invoice.items, function (lineItem) {
                    if (lineItem.lineItemTfKey) {
                        var matchedTypeField = _.find($scope.typeFields, function (type) {
                            return type.typeKey == lineItem.lineItemTfKey;
                        });
                        lineItem.lineItemType = matchedTypeField;
                    }
                });
                $scope.loadShippingAddress($scope.invoice);
                $scope.loadPayments($scope.invoice);
                $scope.loadShipments($scope.invoice);
                $scope.loadAuditLog($scope.invoice.key);
            }, function (reason) {
                notificationsService.error("Invoice Load Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name loadPayments
         * @function
         * 
         * @description - Load the payments for the provided invoice.
         */
        $scope.loadPayments = function (invoice) {
            var promise = merchelloPaymentService.getAppliedPaymentsByInvoice(invoice.key);
            promise.then(function (appliedPayments) {
                invoice.appliedPayments = appliedPayments;
                invoice.payments = [];
                if (invoice.appliedPayments.length > 0) {
                    invoice.payments = _.uniq(_.map(invoice.appliedPayments, function (appliedPayment) {
                        return appliedPayment.payment;
                    }));
                }
                _.each(invoice.appliedPayments, function (appliedPayment) {
                    if (appliedPayment.appliedPaymentTfKey) {
                        var matchedTypeField = _.find($scope.typeFields, function (type) {
                            return type.typeKey == appliedPayment.appliedPaymentTfKey;
                        });
                        appliedPayment.appliedPaymentType = matchedTypeField;
                    }
                });
                // used for rendering the payment history
                invoice.groupedAppliedPayments = _.groupBy(invoice.appliedPayments, function (appliedPayment) {
                    return appliedPayment.payment.paymentMethodName;
                });
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            }, function (reason) {
                notificationsService.error("Payments Load Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         * 
         * @description - Load the Merchello settings.
         */
        $scope.loadSettings = function () {
            var currencySymbolPromise = merchelloSettingsService.getCurrencySymbol();
            currencySymbolPromise.then(function (currencySymbol) {
                $scope.currencySymbol = currencySymbol;

            }, function (reason) {
                alert('Failed: ' + reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name loadShipments
         * @function
         * 
         * @description - Load the shipments associated with the provided invoice.
         */
        $scope.loadShipments = function (invoice) {
            if ($scope.hasOrder()) {
                var promise = merchelloShipmentService.getShipmentsByInvoice(invoice);
                promise.then(function (shipments) {
                    invoice.shipments = _.map(shipments, function (shipment) {
                        return new merchello.Models.Shipment(shipment);
                    });
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Shipments Load Failed", reason.message);
                });
            }
        };

        /**
         * @ngdoc method
         * @name loadShippingAddress
         * @function
         * 
         * @description
         * Load the shipping address associated with the provided invoice.
         */
        $scope.loadShippingAddress = function (invoice) {
            if ($scope.hasOrder()) {
                var promise = merchelloOrderService.getShippingAddress(invoice.key);
                promise.then(function (address) {
                    $scope.shippingAddress = new merchello.Models.Address(address);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Address Load Failed", reason.message);
                });
            }
        };

        /**
         * @ngdoc method
         * @name loadTypeFields
         * @function
         * 
         * @description - Load in the type fields.
         */
        $scope.loadTypeFields = function (nextMethodCall) {
            var promise = merchelloSettingsService.getTypeFields();
            promise.then(function (typeFields) {
                $scope.typeFields = _.map(typeFields, function (type) {
                    return new merchello.Models.TypeField(type);
                });
                if (nextMethodCall != undefined) {
                    nextMethodCall();
                }
            }, function (reason) {
                notificationsService.error("TypeFields Load Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name setVariables
         * @function
         * 
         * @description - Sets the $scope variables.
         */
        $scope.setVariables = function () {
            $scope.historyLoaded = false;
            $scope.invoice = {};
            $scope.typeFields = [];
            $scope.shippingAddress = {};
            $scope.salesHistory = {
                days: []
            };
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
        $scope.capturePayment = function () {
            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Order/Dialogs/capture.payment.html',
                show: true,
                callback: $scope.capturePaymentDialogConfirm,
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
        $scope.capturePaymentDialogConfirm = function (paymentRequest) {
            var promiseSave = merchelloPaymentService.capturePayment(paymentRequest);
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
        $scope.openDeleteInvoiceDialog = function () {
            var dialogData = {};
            dialogData.name = 'Invoice #' + $scope.invoice.invoiceNumber;
            dialogService.open({
                template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
                show: true,
                callback: $scope.processDeleteInvoiceDialog,
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
        $scope.openFulfillShipmentDialog = function () {
            console.info($scope.invoice);
            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Order/Dialogs/fulfill.shipment.html',
                show: true,
                callback: $scope.processFulfillShipmentDialog,
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
        $scope.processDeleteInvoiceDialog = function () {
            var promiseDeleteInvoice = merchelloInvoiceService.deleteInvoice($scope.invoice.key);
            promiseDeleteInvoice.then(function (response) {
                notificationsService.success('Invoice Deleted');
                window.location.href = '#/merchello/merchello/OrderList/manage';
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
        $scope.processFulfillShipmentDialog = function (data) {
            var promiseNewShipment = merchelloShipmentService.newShipment(data);
            promiseNewShipment.then(function (shipment) {
                shipment.trackingCode = data.trackingNumber;
                var promiseSave = merchelloShipmentService.putShipment(shipment, data);
                promiseSave.then(function () {
                    notificationsService.success("Shipment Created");
                    $scope.loadInvoice(data.invoiceKey);
                }, function (reason) {
                    notificationsService.error("Save Shipment Failed", reason.message);
                });
            }, function (reason) {
                notificationsService.error("New Shipment Failed", reason.message);
            });
        };

        /*-------------------------------------------------------------------
         * Helper Methods
         * ------------------------------------------------------------------*/

        /**
         * @ngdoc method
         * @name buildLocalizedShippingHistory
         * @function
         * 
         * @description - Build a list of the shipping history.
         */
        $scope.buildLocalizedShippingHistory = function (dailyLogs) {
            $scope.salesHistory.days = [];
            _.each(dailyLogs, function (day) {
                var newDay = {
                    date: day.day.split('T')[0],
                    logs: []
                };
                newDay.logs = _.map(day.logs, function (log) {
                    var time = log.recordDate.split('T')[1];
                    time = time.split(':')[0] + ':' + time.split(':')[1];
                    return {
                        time: time,
                        type: log.entityType,
                        messageObject: log.message,
                        message: ''
                    }
                });
                _.each(newDay.logs, function (logItem) {
                    var key = logItem.messageObject.area + '_' + logItem.messageObject.key;
                    localizationService.localize(key).then(function (value) {
                        logItem.message = $scope.formatLogMessage(value, logItem.messageObject);
                    });
                });
                $scope.salesHistory.days.push(newDay);
            });
            $scope.historyLoaded = true;
        };

        /**
         * @ngdoc method
         * @name formatLogMessage
         * @function
         * 
         * @description - Format the provided textstring with the appropriate log item values from the message object.
         */
        $scope.formatLogMessage = function (textString, message) {
            switch (message.key) {
                case 'invoiceCreated':
                case 'invoiceDeleted':
                case 'orderCreated':
                    textString = textString.replace('%0%', message.invoiceNumber);
                    break;
                case 'orderDeleted':
                    textString = textString.replace('%0%', message.orderNumber);
                    break;
                case 'shipmentCreated':
                    textString = textString.replace('%0%', message.itemCount);
                    break;
                case 'paymentAuthorize':
                case 'paymentCaptured':
                    textString = textString.replace('%0%', message.invoiceTotal);
                    textString = textString.replace('%1%', message.currencyCode);
                    break;
                case 'paymentRefunded':
                    textString = textString.replace('%0%', message.refundAmount);
                    textString = textString.replace('%1%', message.currencyCode);
                    break;
                default:
                    break;
            };
            return textString;
        };

        /**
         * @ngdoc method
         * @name hasAppliedPayments
         * @function
         * 
         * @description - Returns false if the invoice has no applied payments.
         */
        $scope.hasAppliedPayments = function () {
            var result = false;
            if ($scope.invoice.appliedPayments) {
                if ($scope.invoice.appliedPayments.length > 0) {
                    result = true;
                }
            }
            return result;
        };

        /**
         * @ngdoc method
         * @name hasOrder
         * @function
         * 
         * @description - Returns false if the invoice has no orders.
         */
        $scope.hasOrder = function () {
            var result = false;
            if ($scope.invoice.orders !== undefined) {
                if ($scope.invoice.orders.length > 0) {
                    result = true;
                }
            }
            return result;
        };

        /**
         * @ngdoc method
         * @name hasShipments
         * @function
         * 
         * @description - Returns false if the invoice has no shipments.
         */
        $scope.hasShipments = function() {
            var result = false;
            if ($scope.invoice.shipments) {
                if ($scope.invoice.shipments.length > 0) {
                    result = true;
                }
            }
            return result;
        };

        /**
         * @ngdoc method
         * @name isPaid
         * @function
         * 
         * @description - Returns true if the invoice has been paid. Otherwise it returns false.
         */
        $scope.isPaid = function () {
            var result = false;
            if (typeof $scope.invoice.getPaymentStatus === "function") {
                var status = $scope.invoice.getPaymentStatus();
                if (status === "Paid") {
                    result = true;
                }
            }
            return result;
        };


        /*-------------------------------------------------------------------*/

        $scope.init();

    };


    angular.module("umbraco").controller("Merchello.Editors.Order.ViewController", ['$scope', '$routeParams', 'assetsService', 'dialogService', 'localizationService', 'notificationsService', 'merchelloAuditService', 'merchelloInvoiceService', 'merchelloOrderService', 'merchelloPaymentService', 'merchelloShipmentService', 'merchelloSettingsService', merchello.Controllers.OrderViewController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));

