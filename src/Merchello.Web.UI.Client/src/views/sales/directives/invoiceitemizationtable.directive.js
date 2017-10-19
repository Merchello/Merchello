angular.module('merchello.directives').directive('invoiceItemizationTable',
    ['$q', '$timeout', 'localizationService', 'invoiceResource', 'invoiceHelper', 'dialogService', 'productResource', 'notificationsService',
        function ($q, $timeout, localizationService, invoiceResource, invoiceHelper, dialogService, productResource, notificationsService) {
            return {
                restrict: 'E',
                replace: true,
                scope: {
                    invoice: '=',
                    payments: '=',
                    allPayments: '=',
                    paymentMethods: '=',
                    preValuesLoaded: '=',
                    currencySymbol: '=',
                    canEditLineItems:'=',
                    save: '&',
                    reload:'&'
                },
                templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/invoiceitemizationtable.tpl.html',
                link: function (scope, elm, attr) {

                    scope.loaded = false;
                    scope.authorizedCapturedLabel = '';
                    scope.taxTotal = 0;
                    scope.shippingTotal = 0;
                    scope.customLineItems = [];
                    scope.discountLineItems = [];
                    scope.adjustmentLineItems = [];
                    scope.remainingBalance = 0;

                    scope.itemization = {};

                    function init() {

                        // ensure that the parent scope promises have been resolved
                        scope.$watch('preValuesLoaded', function (pvl) {
                            if (pvl === true) {
                                loadInvoice();
                            }
                        });
                    }


                    // Previews a line item on invoice in a dialog
                    scope.lineItemPreview = function(sku) {

                        // Setup the dialog data
                        var dialogData = {
                            product: {},
                            sku: sku
                        };

                        // Get the product if it exists! We call the vairant service as this seems
                        // to return the base product too
                        productResource.getVariantBySku(sku).then(function(result) {
                            // If we get something back then add it to the diaglogData
                            if (result) {
                                dialogData.product = result;
                            }
                        });

                        dialogService.open({
                            template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.previewlineitem.html',
                            show: true,
                            dialogData: dialogData
                        });
                    };


                    // The dialog that deals with lineitem quantity changes and deletions
                    scope.editLineItem = function (lineItem) {

                        var dialogData = {
                            quantity: lineItem.quantity,
                            lineItem: lineItem,
                            deleteLineItem: false,
                            canDelete: scope.invoice.items.length > 1
                        };

                        dialogService.open({
                            template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.edit.lineitem.html',
                            show: true,
                            dialogData: dialogData,
                            callback: updateLineItem
                        });

                    };

                    // Update a product line item on the invoice (Edit or Delete)
                    function updateLineItem(lineItemDialogData) {
                        var updateInvoice = false;
                        var keepFindingProduct = true;

                        if (lineItemDialogData.deleteLineItem) {

                            var indexLocation = 0;

                            // Loop through items                           
                            angular.forEach(scope.invoice.items, function (item) {
                                if (keepFindingProduct) {
                                    if (lineItemDialogData.lineItem.sku === item.sku) {
                                        // Stop finding and break (As no break in angular loop, this is best way)
                                        keepFindingProduct = false;
                                    } else {
                                        // Update the index location as no match
                                        indexLocation++;
                                    }
                                }
                            });

                            // We know we found the lineitem if this is set to false
                            if (keepFindingProduct === false) {

                                // Delete the line item from the invoice display and put the invoice
                                scope.invoice.items.splice(indexLocation, 1);

                                // Set the invoice to be updated
                                updateInvoice = true;   
                            }

                        } else {

                            // See if the quantity has changed and then        
                            angular.forEach(scope.invoice.items, function (item) {
                                if (keepFindingProduct) {
                                    if (lineItemDialogData.lineItem.sku === item.sku
                                        && lineItemDialogData.quantity !== item.quantity) {

                                        // Update the quantity to what the user set
                                        item.quantity = lineItemDialogData.quantity;

                                        // Stop finding (As no break in angular loop, this is best way)
                                        keepFindingProduct = false;

                                        // Set product to be updated
                                        updateInvoice = true;
                                    }   
                                }
                            });
                        }

                        // See if we need to update the invoice
                        if (updateInvoice) {
                            // Force the put invoice to sync the line items too
                            scope.invoice.SyncLineItems = true;

                            // Put the invoice 
                            var invoiceSavePromise = invoiceResource.saveInvoice(scope.invoice);
                            invoiceSavePromise.then(function () {
                                $timeout(function () {
                                    scope.reload();
                                    notificationsService.success('Invoice line Items Updated.');
                                }, 1500);
                            }, function (reason) {
                                notificationsService.error("Failed to update invoice line items", reason.message);
                            });
                        }
                    };

                    function loadInvoice() {
                        var taxLineItem = scope.invoice.getTaxLineItem();
                        scope.taxTotal = taxLineItem !== undefined ? taxLineItem.price : 0;
                        scope.shippingTotal = scope.invoice.shippingTotal();

                        scope.customLineItems = scope.invoice.getCustomLineItems();
                        scope.discountLineItems = scope.invoice.getDiscountLineItems();
                        scope.adjustmentLineItems = scope.invoice.getAdjustmentLineItems();

                        angular.forEach(scope.adjustmentLineItems, function(item) {
                            item.userName = item.extendedData.getValue("userName");
                            item.email = item.extendedData.getValue("email");
                        });

                        scope.remainingBalance = invoiceHelper.round(scope.invoice.remainingBalance(scope.allPayments), 2);

                        var label  = scope.remainingBalance == '0' ? 'merchelloOrderView_captured' : 'merchelloOrderView_authorized';

                        $q.all([
                            localizationService.localize(label),
                            invoiceResource.getItemItemization(scope.invoice.key)
                        ]).then(function(data) {
                            scope.authorizedCapturedLabel = data[0];
                            scope.itemization = data[1];
                            scope.loaded = true;
                        });

                    }

                    // initialize the directive
                    init();
                }
            }
        }]);
