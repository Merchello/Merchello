    /**
     * @ngdoc controller
     * @name Merchello.Editors.Sales.OverviewController
     * @function
     *
     * @description
     * The controller for the sales overview view
     */
    angular.module('merchello').controller('Merchello.Backoffice.SalesOverviewController',
        ['$scope', '$routeParams', '$timeout', '$log', '$location', 'assetsService', 'dialogService', 'localizationService', 'notificationsService', 'invoiceHelper',
            'auditLogResource', 'noteResource', 'invoiceResource', 'settingsResource', 'paymentResource', 'shipmentResource', 'paymentGatewayProviderResource',
            'orderResource', 'dialogDataFactory', 'merchelloTabsFactory', 'addressDisplayBuilder', 'countryDisplayBuilder', 'salesHistoryDisplayBuilder', 'noteDisplayBuilder',
            'invoiceDisplayBuilder', 'paymentDisplayBuilder', 'paymentMethodDisplayBuilder', 'shipMethodsQueryDisplayBuilder',
        function($scope, $routeParams, $timeout, $log, $location, assetsService, dialogService, localizationService, notificationsService, invoiceHelper,
                 auditLogResource, noteResource, invoiceResource, settingsResource, paymentResource, shipmentResource, paymentGatewayProviderResource, orderResource, dialogDataFactory,
                 merchelloTabsFactory, addressDisplayBuilder, countryDisplayBuilder, salesHistoryDisplayBuilder, noteDisplayBuilder, invoiceDisplayBuilder, paymentDisplayBuilder, paymentMethodDisplayBuilder, shipMethodsQueryDisplayBuilder) {

            // exposed properties
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.paymentMethodsLoaded = false;
            $scope.invoice = {};
            $scope.invoiceNumber = '';
            $scope.tabs = [];
            $scope.historyLoaded = false;
            $scope.currencySymbol = '';
            $scope.settings = {};
            $scope.salesHistory = {};

            $scope.paymentMethods = [];
            $scope.allPayments = [];
            $scope.payments = [];
            $scope.billingAddress = {};
            $scope.hasShippingAddress = false;
            $scope.discountLineItems = [];
            $scope.debugAllowDelete = false;
            $scope.newPaymentOpen = false;
            $scope.entityType = 'Invoice';

            $scope.canAddLineItems = true;

            $scope.remainingBalance = 0;

            // exposed methods
            //  dialogs
            $scope.capturePayment = capturePayment;
            $scope.showFulfill = true;
            $scope.openDeleteInvoiceDialog = openDeleteInvoiceDialog;
            $scope.cancelInvoice = cancelInvoice;
            $scope.processDeleteInvoiceDialog = processDeleteInvoiceDialog,
            $scope.openFulfillShipmentDialog = openFulfillShipmentDialog;
            $scope.processFulfillShipmentDialog = processFulfillShipmentDialog;

            $scope.toggleNewPaymentOpen = toggleNewPaymentOpen;
            $scope.reload = init;
            $scope.openAddressAddEditDialog = openAddressAddEditDialog;
            $scope.setNotPreValuesLoaded = setNotPreValuesLoaded;
            $scope.saveNotes = saveNotes;
            $scope.deleteNote = deleteNote;

            $scope.openProductSelection = openProductSelectionDialog;

            // localize the sales history message
            $scope.localizeMessage = localizeMessage;

            $scope.refresh = refresh;


            var countries = [];

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description - Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init () {
                $scope.preValuesLoaded = false;
                $scope.newPaymentOpen = false;
                loadInvoice($routeParams.id);
                $scope.tabs = merchelloTabsFactory.createSalesTabs($routeParams.id);
                $scope.tabs.setActive('overview');
                if(Umbraco.Sys.ServerVariables.isDebuggingEnabled) {
                    $scope.debugAllowDelete = true;
                }

            }

            function localizeMessage(msg) {
                return msg.localize(localizationService);
            }


            function openProductSelectionDialog() {
                var dialogData = {};
                dialogData.addItems = [];

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/customer.productselectionfilter.html',
                    show: true,
                    callback: processProductSelection,
                    dialogData: dialogData
                });
            }

            function processProductSelection(dialogData) {

                // Post the model back to the controller
                var invoiceAddItems = {
                    InvoiceKey: $scope.invoice.key,
                    Items: dialogData.addItems,
                    LineItemType: 'Product',
                    IsAddProduct: true
                }

                // Put the new items
                var invoiceSavePromise = invoiceResource.putInvoiceNewProducts(invoiceAddItems);
                invoiceSavePromise.then(function () {
                    $timeout(function () {
                        refresh();
                        notificationsService.success('Invoice Updated.');
                    }, 1500);
                }, function (reason) {
                    notificationsService.error("Failed to update invoice", reason.message);
                });

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
                                  if (log.message.formattedMessage ==='') {
                                     localizationService.localize(log.message.localizationKey(), log.message.localizationTokens()).then(function(value) {
                                        log.message.formattedMessage = value;
                                     });
                                  }
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
                // assert the collections are reset before populating
                $scope.shipmentLineItems = [];
                $scope.customLineItems = [];
                $scope.discountLineItems = [];
                var promise = invoiceResource.getByKey(id);
                promise.then(function (invoice) {

                    $scope.invoice = invoiceDisplayBuilder.transform(invoice);
                    $scope.billingAddress = $scope.invoice.getBillToAddress();

                    $scope.invoiceNumber = $scope.invoice.prefixedInvoiceNumber();
                    loadSettings();
                    loadPayments(id);
                    loadAuditLog(id);

                    loadShippingAddress(id);

                    $scope.showFulfill = hasUnPackagedLineItems();
                    $scope.loaded = true;

                    var shipmentLineItem = $scope.invoice.getShippingLineItems();
                    if (shipmentLineItem) {
                        $scope.shipmentLineItems.push(shipmentLineItem);
                    }

                    $scope.tabs.appendCustomerTab($scope.invoice.customerKey);

                    $scope.canAddLineItems = $scope.invoice.enableInvoiceEditQty;

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
               settingsResource.getAllCombined().then(function(combined) {
                   $scope.settings = combined.settings;
                   countries = combined.countries;
                   if ($scope.invoice.currency.symbol === '') {
                       var currency = _.find(combined.currencies, function (symbol) {
                           return symbol.currencyCode === $scope.invoice.getCurrencyCode();
                       });
                       if (currency !== undefined) {
                           $scope.currencySymbol = currency.symbol;
                       } else {
                           $scope.currencySymbol = combined.currencySymbol;
                       }
                   } else {
                       $scope.currencySymbol = $scope.invoice.currency.symbol;
                   }
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
                    $scope.allPayments = paymentDisplayBuilder.transform(payments);
                    $scope.payments = _.filter($scope.allPayments, function (p) { return !p.voided && !p.collected && p.authorized; });

                    loadPaymentMethods();
                    $scope.preValuesLoaded = true;

                    // Set the remaining balance after the payments have loaded
                    $scope.remainingBalance = invoiceHelper.round($scope.invoice.remainingBalance($scope.allPayments), 2);

                }, function(reason) {
                    notificationsService.error('Failed to load payments for invoice', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadPaymentMethods
             * @function
             *
             * @description - Load the available Merchello payment methods for the invoice.
             */
            function loadPaymentMethods() {
                if($scope.payments.length === 0) {
                    var promise = paymentGatewayProviderResource.getAvailablePaymentMethods();
                    promise.then(function(methods) {
                        $scope.paymentMethods = paymentMethodDisplayBuilder.transform(methods);
                        $scope.preValuesLoaded = true;
                        $scope.paymentMethodsLoaded = true;
                    });
                }
            }

            /**
             * @ngdoc method
             * @name loadShippingAddress
             * @function
             *
             * @description - Load the shipping address (if any) for an invoice.
             */
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
             * @name toggleNewPaymentOpen
             * @function
             *
             * @description - Toggles the new payment open variable.
             */
            function toggleNewPaymentOpen() {
                $scope.newPaymentOpen = !$scope.newPaymentOpen;
            }

            /**
             * @ngdoc method
             * @name setNotPreValuesLoaded
             * @function
             *
             * @description - Sets preValuesLoaded to false.  For use in directives.
             */
            function setNotPreValuesLoaded() {
                $scope.preValuesLoaded = false;
            }

            /**
             * @ngdoc method
             * @name capturePayment
             * @function
             *
             * @description - Open the capture payment dialog.
             */
            function capturePayment() {
                var dialogData = dialogDataFactory.createCapturePaymentDialogData();
                dialogData.setPaymentData($scope.payments[0]);
                dialogData.setInvoiceData($scope.allPayments, $scope.invoice, $scope.currencySymbol, invoiceHelper);
                if (!dialogData.isValid()) {
                    return false;
                }

                /*
                    We need to be able to swap out the editor depending on the provider here.
                */

                var promise = paymentResource.getPaymentMethod(dialogData.paymentMethodKey);
                promise.then(function(paymentMethod) {
                    var pm = paymentMethodDisplayBuilder.transform(paymentMethod);
                    if (pm.capturePaymentEditorView.editorView !== '') {
                        dialogData.capturePaymentEditorView = pm.capturePaymentEditorView.editorView;
                    } else {
                        dialogData.capturePaymentEditorView = '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.cashpaymentmethod.authorizecapturepayment.html';
                    }
                    dialogService.open({
                        template: dialogData.capturePaymentEditorView,
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
                var promiseSave = paymentResource.capturePayment(paymentRequest);
                promiseSave.then(function (payment) {
                    // added a timeout here to give the examine index
                    $timeout(function () {
                        loadInvoice(paymentRequest.invoiceKey);
                        notificationsService.success("Payment Captured");
                    }, 1500);
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

            function cancelInvoice() {
                var promiseCancelInvoice = invoiceResource.cancelInvoice($scope.invoice.key);
                promiseCancelInvoice.then(function (response) {
                    notificationsService.success('Invoice Cancelled');
                    $location.url("/merchello/merchello/saleslist/manage", true);
                }, function (reason) {
                    notificationsService.error('Failed to cancel Invoice', reason.message);
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

                    // Loop orders until I find one without getUnShippedItems()!!

                    var keepFindingOrder = true;
                    angular.forEach($scope.invoice.orders, function (order) {
                        if (keepFindingOrder) {
                            // Get unshipped items from this order
                            var unshippedItems = order.getUnShippedItems();

                            // If there are any, return them
                            if (unshippedItems.length > 0) {
                                data.order = order;
                                data.order.items = unshippedItems;
                                keepFindingOrder = false;
                            }
                        }
                    });

                    data.totalOrders = $scope.invoice.orders;
                    data.shipmentStatuses = statuses;
                    data.currencySymbol = $scope.currencySymbol;

                    // packaging
                    var quotedKey = '7342dcd6-8113-44b6-bfd0-4555b82f9503';
                    data.shipmentStatus = _.find(data.shipmentStatuses, function(status) {
                        return status.key === quotedKey;
                    });
                    data.invoiceKey = $scope.invoice.key;

                    // TODO this could eventually turn into an array
                    var shipmentLineItems = $scope.invoice.getShippingLineItems();
                    data.shipmentLineItems = shipmentLineItems;
                    if (shipmentLineItems.length) {
                        var shipMethodKey = shipmentLineItems[0].extendedData.getValue('merchShipMethodKey');
                        var request = { shipMethodKey: shipMethodKey, invoiceKey: data.invoiceKey, lineItemKey: shipmentLineItems[0].key };
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
                            //console.info(shipment);
                            loadInvoice(data.invoiceKey);
                            notificationsService.success('Shipment #' + shipment.shipmentNumber + ' created');
                        }, 1500);

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
                    // If this invoice is fullfilled, then they can't add or edit it
                    $scope.canAddLineItems = false;
                    return false;
                }
                var i = 0; // order count
                var found = false;
                while(i < $scope.invoice.orders.length && !found) {
                    var item = _.find($scope.invoice.orders[ i ].items, function(item) {
                      return (item.shipmentKey === '' || item.shipmentKey === null) && item.extendedData.getValue('merchShippable').toLowerCase() === 'true';
                    });
                    if(item !== null && item !== undefined) {
                        found = true;
                    } else {
                        i++;
                    }
                }

                return found;
            }

            /**
             * @ngdoc method
             * @name openAddressEditDialog
             * @function
             *
             * @description
             * Opens the edit address dialog via the Umbraco dialogService.
             */
            function openAddressAddEditDialog(address) {
                var dialogData = dialogDataFactory.createEditAddressDialogData();
                // if the address is not defined we need to create a default (empty) AddressDisplay
                if(address === null || address === undefined) {
                    dialogData.address = addressDisplayBuilder.createDefault();
                    dialogData.selectedCountry = countries[0];
                } else {
                    dialogData.address = address.clone();
                    dialogData.selectedCountry = address.countryCode === '' ? countries[0] :
                        _.find(countries, function(country) {
                            return country.countryCode === address.countryCode;
                        });
                }
                dialogData.countries = countries;

                if (dialogData.selectedCountry.hasProvinces()) {
                    if(dialogData.address.region !== '') {
                        dialogData.selectedProvince = _.find(dialogData.selectedCountry.provinces, function(province) {
                            return province.code === address.region;
                        });
                    }
                    if(dialogData.selectedProvince === null || dialogData.selectedProvince === undefined) {
                        dialogData.selectedProvince = dialogData.selectedCountry.provinces[0];
                    }
                }

                if (address.addressType === 'Billing') {
                    dialogData.warning = localizationService.localize('merchelloSales_noteInvoiceAddressChange');
                } else {
                    dialogData.warning = localizationService.localize('merchelloSales_noteShipmentAddressChange');
                }

                // Show email and phone
                dialogData.showPhone = true;
                dialogData.showEmail = true;

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/edit.address.html',
                    show: true,
                    callback: processAddEditAddressDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name processAddEditAddressDialog
             * @function
             *
             * @description
             * Responsible for editing an address
             */
            function processAddEditAddressDialog(dialogData) {
                var adr = dialogData.address;

                if (adr.addressType === 'Billing') {
                    $scope.invoice.setBillingAddress(adr);
                    $scope.preValuesLoaded = false;
                    var billingPromise = invoiceResource.saveInvoice($scope.invoice);
                    billingPromise.then(function () {
                        $timeout(function () {
                            loadInvoice($scope.invoice.key);
                            notificationsService.success('Billing address successfully updated.');
                        }, 1500);
                    }, function (reason) {
                        notificationsService.error("Failed to update billing address", reason.message);
                    });
                } else {
                    // we need to update the shipment line item on the invoice
                    var adrData = {
                        invoiceKey: $scope.invoice.key,
                        address: dialogData.address
                    };
                    var shippingPromise = invoiceResource.saveInvoiceShippingAddress(adrData);
                    shippingPromise.then(function () {
                        $timeout(function () {
                            loadInvoice($scope.invoice.key);
                            notificationsService.success('Shipping address successfully updated.');
                        }, 1500);
                    }, function (reason) {
                        notificationsService.error("Failed to update shippingaddress", reason.message);
                    });
                }
            }

            function saveNotes() {
                saveInvoice();
            }
            
            function deleteNote(note) {
                $scope.invoice.notes = _.reject($scope.invoice.notes, function(n) {
                    return n.key === note.key;
                });

                saveInvoice();
            }
            
            function saveInvoice() {
                invoiceResource.saveInvoice($scope.invoice).then(function(data) {
                    refresh();
                });
            }

            function refresh() {
                $timeout(function () {
                    loadInvoice($scope.invoice.key);
                }, 1500);
            }

            // initialize the controller
            init();
    }]);
