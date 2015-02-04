    /**
     * @ngdoc controller
     * @name Merchello.Editors.Sales.OverviewController
     * @function
     *
     * @description
     * The controller for the sales overview view
     */
    angular.module('merchello').controller('Merchello.Backoffice.SalesOverviewController',
        ['$scope', '$routeParams', '$timeout', '$log', '$location', 'assetsService', 'dialogService', 'localizationService', 'notificationsService',
            'auditLogResource', 'invoiceResource', 'settingsResource', 'paymentResource', 'shipmentResource',
            'orderResource', 'dialogDataFactory', 'merchelloTabsFactory', 'addressDisplayBuilder', 'salesHistoryDisplayBuilder',
            'invoiceDisplayBuilder', 'paymentDisplayBuilder', 'paymentMethodDisplayBuilder', 'shipMethodsQueryDisplayBuilder',
        function($scope, $routeParams, $timeout, $log, $location, assetsService, dialogService, localizationService, notificationsService,
                 auditLogResource, invoiceResource, settingsResource, paymentResource, shipmentResource, orderResource, dialogDataFactory,
                 merchelloTabsFactory, addressDisplayBuilder, salesHistoryDisplayBuilder, invoiceDisplayBuilder, paymentDisplayBuilder, paymentMethodDisplayBuilder, shipMethodsQueryDisplayBuilder) {

            // exposed properties
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.invoice = {};
            $scope.tabs = [];
            $scope.historyLoaded = false;
            $scope.remainingBalance = 0.0;
            $scope.shippingTotal = 0.0;
            $scope.taxTotal = 0.0;
            $scope.currencySymbol = '';
            $scope.settings = {};
            $scope.salesHistory = {};
            $scope.paymentMethods = {};
            $scope.payments = [];
            $scope.billingAddress = {};
            $scope.hasShippingAddress = false;
            $scope.authorizedCapturedLabel = '';
            $scope.shipmentLineItems = [];

            // exposed methods
            //  dialogs
            $scope.capturePayment = capturePayment;
            $scope.showFulfill = true;
            $scope.openDeleteInvoiceDialog = openDeleteInvoiceDialog;
            $scope.processDeleteInvoiceDialog = processDeleteInvoiceDialog,
            $scope.openFulfillShipmentDialog = openFulfillShipmentDialog;
            $scope.processFulfillShipmentDialog = processFulfillShipmentDialog;

            // localize the sales history message
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
                $scope.tabs = merchelloTabsFactory.createSalesTabs($routeParams.id);
                $scope.tabs.setActive('overview');
                $scope.loaded = true;
            }

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
                            angular.forEach(history.dailyLogs, function(daily) {
                              angular.forEach(daily.logs, function(log) {
                                 localizationService.localize(log.message.localizationKey(), log.message.localizationTokens()).then(function(value) {
                                    log.message.formattedMessage = value;
                                 });
                              });
                            });
                        }
                        $scope.historyLoaded = history.dailyLogs.length > 0;
                    }, function (reason) {
                        notificationsService.error('Failed to load sales history', reason.message);
                    });
                }
            }

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
                    $scope.billingAddress = $scope.invoice.getBillToAddress();
                    var taxLineItem = $scope.invoice.getTaxLineItem();
                    $scope.taxTotal = taxLineItem !== undefined ? taxLineItem.price : 0;
                    $scope.shippingTotal = $scope.invoice.shippingTotal();
                    loadSettings();
                    loadPayments(id);
                    loadAuditLog(id);
                    loadShippingAddress(id);
                    $scope.showFulfill = hasUnPackagedLineItems();
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                    var shipmentLineItem = $scope.invoice.getShippingLineItems();
                    if (shipmentLineItem) {
                        $scope.shipmentLineItems.push(shipmentLineItem);
                    }

                   $scope.tabs.appendCustomerTab($scope.invoice.customerKey);

                }, function (reason) {
                    notificationsService.error("Invoice Load Failed", reason.message);
                });
            }


           /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {
               var settingsPromise = settingsResource.getAllSettings();
               settingsPromise.then(function(settings) {
                   $scope.settings = settings;
               }, function(reason) {
                   notificationsService.error('Failed to load global settings', reason.message);
               })

                var currencySymbolPromise = settingsResource.getAllCurrencies();
                currencySymbolPromise.then(function (symbols) {
                    var currency = _.find(symbols, function(symbol) {
                        return symbol.currencyCode === $scope.invoice.getCurrencyCode()
                    });
                    if (currency !== undefined) {
                    $scope.currencySymbol = currency.symbol;
                    } else {
                        // this handles a legacy error where in some cases the invoice may not have saved the ISO currency code
                        // default currency
                        var defaultCurrencyPromise = settingsResource.getCurrencySymbol();
                        defaultCurrencyPromise.then(function (currencySymbol) {
                            $scope.currencySymbol = currencySymbol;
                        }, function (reason) {
                            notificationService.error('Failed to load the default currency symbol', reason.message);
                        });
                    }
                }, function (reason) {
                    alert('Failed: ' + reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadPayments
             * @function
             *
             * @description - Load the Merchello payments for the invoice.
             */
            function loadPayments(key) {
                var paymentsPromise = paymentResource.getPaymentsByInvoice(key);
                paymentsPromise.then(function(payments) {
                    $scope.payments = paymentDisplayBuilder.transform(payments);
                    $scope.remainingBalance = $scope.invoice.remainingBalance($scope.payments);
                    $scope.authorizedCapturedLabel  = $scope.remainingBalance == '0' ? 'merchelloOrderView_captured' : 'merchelloOrderView_authorized';
                }, function(reason) {
                    notificationsService.error('Failed to load payments for invoice', reason.message);
                });
            }

            function loadShippingAddress(key) {
                var shippingAddressPromise = orderResource.getShippingAddress(key);
                shippingAddressPromise.then(function(result) {
                      $scope.shippingAddress = addressDisplayBuilder.transform(result);
                      $scope.hasShippingAddress = true;
                }, function(reason) {
                    notificationsService.error('Failed to load shipping address', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name capturePayment
             * @function
             *
             * @description - Open the capture shipment dialog.
             */
            function capturePayment() {
                var dialogData = dialogDataFactory.createCapturePaymentDialogData();
                dialogData.setPaymentData($scope.payments[0]);
                dialogData.setInvoiceData($scope.payments, $scope.invoice, $scope.currencySymbol);
                if (!dialogData.isValid()) {
                    return false;
                }
                var promise = paymentResource.getPaymentMethod(dialogData.paymentMethodKey);
                promise.then(function(paymentMethod) {
                    var pm = paymentMethodDisplayBuilder.transform(paymentMethod);
                    if (pm.authorizeCapturePaymentDialogEditorView !== '') {
                        dialogData.authorizeCapturePaymentEditorView = pm.authorizeCapturePaymentEditorView.editorView;
                    } else {
                        dialogData.authorizeCapturePaymentEditorView = '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.cashpaymentmethod.authorizecapturepayment.html';
                    }
                    console.info(pm);
                    dialogService.open({
                        template: dialogData.authorizeCapturePaymentEditorView,
                        show: true,
                        callback: capturePaymentDialogConfirm,
                        dialogData: dialogData
                    });
                });
            }

            /**
             * @ngdoc method
             * @name capturePaymentDialogConfirm
             * @function
             *
             * @description - Capture the payment after the confirmation dialog was passed through.
             */
            function capturePaymentDialogConfirm(paymentRequest) {
                $scope.preValuesLoaded = false;
                console.info(paymentRequest);
                var promiseSave = paymentResource.capturePayment(paymentRequest);
                promiseSave.then(function (payment) {
                    // added a timeout here to give the examine index
                    $timeout(function() {
                        notificationsService.success("Payment Captured");
                        loadInvoice(paymentRequest.invoiceKey);
                    }, 400);
                }, function (reason) {
                    notificationsService.error("Payment Capture Failed", reason.message);
                });
            }

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
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteInvoiceDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name openFulfillShipmentDialog
             * @function
             *
             * @description - Open the fufill shipment dialog.
             */
            function openFulfillShipmentDialog() {
                var promiseStatuses = shipmentResource.getAllShipmentStatuses();
                promiseStatuses.then(function(statuses) {
                    var data = dialogDataFactory.createCreateShipmentDialogData();
                    data.order = $scope.invoice.orders[0]; // todo: pull from current order when multiple orders is available
                    data.order.items = data.order.getUnShippedItems();
                    data.shipmentStatuses = statuses;

                    // packaging
                    var quotedKey = '7342dcd6-8113-44b6-bfd0-4555b82f9503';
                    data.shipmentStatus = _.find(data.shipmentStatuses, function(status) {
                        return status.key === quotedKey;
                    });
                    data.invoiceKey = $scope.invoice.key;

                    // TODO this could eventually turn into an array
                    var shipmentLineItem = $scope.invoice.getShippingLineItems();
                    if ($scope.shipmentLineItems[0]) {
                        var shipMethodKey = $scope.shipmentLineItems[0].extendedData.getValue('merchShipMethodKey');
                        var request = { shipMethodKey: shipMethodKey, invoiceKey: data.invoiceKey, lineItemKey: $scope.shipmentLineItems[0].key };
                        var shipMethodPromise = shipmentResource.getShipMethodAndAlternatives(request);
                        shipMethodPromise.then(function(result) {
                            data.shipMethods = shipMethodsQueryDisplayBuilder.transform(result);
                            data.shipMethods.selected = _.find(data.shipMethods.alternatives, function(method) {
                                return method.key === shipMethodKey;
                            });
                            dialogService.open({
                                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.create.shipment.html',
                                show: true,
                                callback: $scope.processFulfillShipmentDialog,
                                dialogData: data
                            });

                        });
                    }
                });
            }

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
                    $location.url("/merchello/merchello/saleslist/manage", true);
                }, function (reason) {
                    notificationsService.error('Failed to Delete Invoice', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name processFulfillPaymentDialog
             * @function
             *
             * @description - Process the fulfill shipment functionality on callback from the dialog service.
             */
            function processFulfillShipmentDialog(data) {
                $scope.preValuesLoaded = false;
                if(data.shipmentRequest.order.items.length > 0) {
                    var promiseNewShipment = shipmentResource.newShipment(data.shipmentRequest);
                    promiseNewShipment.then(function (shipment) {
                        $timeout(function() {
                            notificationsService.success('Shipment #' + shipment.shipmentNumber + ' created');
                            loadInvoice(data.invoiceKey);
                        }, 400);

                    }, function (reason) {
                        notificationsService.error("New Shipment Failed", reason.message);
                    });
                } else {
                    $scope.preValuesLoaded = true;
                    notificationsService.warning('Shipment would not contain any items', 'The shipment was not created as it would not contain any items.');
                }
            }

            /**
             * @ngdoc method
             * @name hasUnPackagedLineItems
             * @function
             *
             * @description - Process the fulfill shipment functionality on callback from the dialog service.
             */
            function hasUnPackagedLineItems() {
                var fulfilled = $scope.invoice.getFulfillmentStatus() === 'Fulfilled';
                if (fulfilled) {
                    return false;
                }
                var i = 0; // order count
                var found = false;
                while(i < $scope.invoice.orders.length && !found) {
                    var item = _.find($scope.invoice.orders[ i ].items, function(item) {
                      return item.shipmentKey === '' || item.shipmentKey === null;
                    });
                    if(item !== null && item !== undefined) {
                        found = true;
                    } else {
                        i++;
                    }
                }

                return found;
            }

            // initialize the controller
            init();
    }]);
