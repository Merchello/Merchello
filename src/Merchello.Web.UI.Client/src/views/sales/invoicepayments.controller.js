/**
 * @ngdoc controller
 * @name Merchello.Dashboards.InvoicePaymentsController
 * @function
 *
 * @description
 * The controller for the invoice payments view
 */
angular.module('merchello').controller('Merchello.Backoffice.InvoicePaymentsController',
    ['$scope', '$log', '$routeParams', 'dialogService', 'notificationsService', 'merchelloTabsFactory', 'invoiceHelper', 'dialogDataFactory',
        'invoiceResource', 'paymentResource', 'paymentGatewayProviderResource', 'settingsResource',
        'invoiceDisplayBuilder', 'paymentDisplayBuilder', 'paymentMethodDisplayBuilder',
        function($scope, $log, $routeParams, dialogService, notificationsService, merchelloTabsFactory, invoiceHelper, dialogDataFactory, invoiceResource, paymentResource, paymentGatewayProviderResource, settingsResource,
        invoiceDisplayBuilder, paymentDisplayBuilder, paymentMethodDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.invoice = {};
            $scope.payments = [];
            $scope.paymentMethods = [];
            $scope.remainingBalance = 0;
            $scope.settings = {};
            $scope.currencySymbol = '';
            $scope.showAddPayment = false;

            // exposed methods
            $scope.openVoidPaymentDialog = openVoidPaymentDialog;
            $scope.openRefundPaymentDialog = openRefundPaymentDialog;
            $scope.showVoid = showVoid;
            $scope.showRefund = showRefund;

            // Helper to check for ED keys
            $scope.hasExtendedDataKey = function(items, keyToFind) {
                var hasKey = false;
                if (items != null) {
                    for (var i = 0; i < items.length; i++) {
                        if (items[i].key === keyToFind) {
                            hasKey = true;
                            break;
                        }
                    }
                }
                return hasKey;
            };

            // Helper to show the data
            $scope.showAvsCvvData = function (items) {
                if (items != null) {
                    for (var i = 0; i < items.length; i++) {
                        if (items[i].key === "merchAvsCvvData") {
                            return items[i].value;
                        }
                    }   
                }
                return "-";
            };

            function init() {
                var key = $routeParams.id;
                loadInvoice(key);
                $scope.tabs = merchelloTabsFactory.createSalesTabs(key);
                $scope.tabs.setActive('payments');
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
                    // append the customer tab if needed
                    $scope.tabs.appendCustomerTab($scope.invoice.customerKey);
                    loadPayments(id);
                    loadSettings();
                    $scope.loaded = true;
                }, function (reason) {
                    notificationsService.error("Invoice Load Failed", reason.message);
                });
            };

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
                        return symbol.currencyCode === $scope.invoice.getCurrencyCode();
                    });
                    if (currency !== undefined && currency !== null) {
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
            };

            function showVoid(payment) {
                if (payment.voided) {
                    return false;
                }
                var exists = _.find($scope.paymentMethods, function(pm) { return pm.key === payment.paymentMethodKey; });
                if (exists !== undefined) {
                    return exists.voidPaymentEditorView.editorView !== '';
                } else {
                    return false;
                }
            }

            function showRefund(payment) {
                if (payment.voided || payment.appliedAmount() === 0) {
                    return false;
                }
                var exists = _.find($scope.paymentMethods, function(pm) { return pm.key === payment.paymentMethodKey; });
                if (exists !== undefined) {
                    return exists.refundPaymentEditorView.editorView !== '';
                } else {
                    return false;
                }
            }
            /**
             * @ngdoc method
             * @name loadPayments
             * @function
             *
             * @description - Load the payments applied to an invoice.
             */
            function loadPayments(key)
            {
                var paymentsPromise = paymentResource.getPaymentsByInvoice(key);
                paymentsPromise.then(function(payments) {
                    $scope.payments = paymentDisplayBuilder.transform(payments);
                    $scope.remainingBalance = invoiceHelper.round($scope.invoice.remainingBalance($scope.payments), 2);
                    loadPaymentMethods($scope.payments);
                }, function(reason) {
                    notificationsService.error('Failed to load payments for invoice', reason.message);
                });
            }

            function loadPaymentMethods(payments) {
                var keys = [];
                // we need to get unique keys here so we don't have to look up for every payment
                angular.forEach(payments, function(p) {
                    if(payments.length > 0) {
                        var found = false;
                        var i = 0;
                        while(i < keys.length && !found) {
                            if (keys[i] === p.paymentMethodKey) {
                                found = true;
                            } else {
                                i++;
                            }
                        }

                        if (!found) {
                            if (p.paymentMethodKey === null) {
                                keys.push('removed');
                            } else {
                                keys.push(p.paymentMethodKey);
                            }
                        }
                    }
                });

                angular.forEach(keys, function(key) {

                    if (key === 'removed') {
                        var empty = paymentMethodDisplayBuilder.createDefault();
                        $scope.paymentMethods.push(empty);
                    }   
                        var promise = paymentGatewayProviderResource.getPaymentMethodByKey(key);
                        promise.then(function(method) {
                            $scope.paymentMethods.push(method);
                            if ($scope.paymentMethods.length === keys.length) {
                                $scope.preValuesLoaded = true;
                            }
                        });
                    
                });

                if ($scope.paymentMethods.length === keys.length) {
                    $scope.preValuesLoaded = true;
                }
            }

            // dialog methods

            function openVoidPaymentDialog(payment) {

                var method = _.find($scope.paymentMethods, function(pm) { return pm.key === payment.paymentMethodKey; });
                if (method === undefined) {
                    return;
                }

                var dialogData = dialogDataFactory.createProcessVoidPaymentDialogData();
                dialogData.paymentMethodKey = payment.paymentMethodKey;
                dialogData.paymentKey = payment.key;
                dialogData.invoiceKey = $scope.invoice.key;
                dialogService.open({
                    template: method.voidPaymentEditorView.editorView,
                    show: true,
                    callback: processVoidPaymentDialog,
                    dialogData: dialogData
                });
            }

            function processVoidPaymentDialog(dialogData) {
                $scope.loaded = false;
                var request = dialogData.toPaymentRequestDisplay();
                var promise = paymentResource.voidPayment(request);
                promise.then(function(result) {
                    init();
                });
            }

            function openRefundPaymentDialog(payment) {
                var method = _.find($scope.paymentMethods, function(pm) { return pm.key === payment.paymentMethodKey; });
                if (method === undefined) {
                    return;
                }
                var dialogData = dialogDataFactory.createProcessRefundPaymentDialogData();
                dialogData.invoiceKey = $scope.invoice.key;
                dialogData.paymentKey = payment.key;
                dialogData.currencySymbol = $scope.currencySymbol;
                dialogData.paymentMethodKey = payment.paymentMethodKey;
                dialogData.paymentMethodName = method.name;

                dialogData.appliedAmount = payment.appliedAmount();
                dialogService.open({
                    template: method.refundPaymentEditorView.editorView,
                    show: true,
                    callback: processRefundPaymentDialog,
                    dialogData: dialogData
                });
            }

            function processRefundPaymentDialog(dialogData) {
                $scope.loaded = false;
                var request = dialogData.toPaymentRequestDisplay();
                var promise = paymentResource.refundPayment(request);
                promise.then(function(result) {
                    init();
                });
            }

            init();
}]);
