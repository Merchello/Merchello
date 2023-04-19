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
                    canEditLineItems: '=',

                    save: '&',
                    reload: '&'
                },
                templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/invoiceitemizationtable.tpl.html',
                link: function(scope, elm, attr) {

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
                        scope.$watch('preValuesLoaded',
                            function(pvl) {
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
                    scope.editLineItem = function(lineItem, lineItemType) {

                        var dialogData = {
                            key: lineItem.key,
                            quantity: lineItem.quantity,
                            sku: lineItem.sku,
                            name: lineItem.name,
                            price: lineItem.price,
                            lineItem: lineItem,
                            deleteLineItem: false,
                            canDelete: scope.invoice.items.length > 1,
                            lineItemType: lineItemType
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

                        var keepFindingProduct = true;
                        // Post the model back to the controller
                        var invoiceAddItems = {};

                        if (lineItemDialogData.deleteLineItem) {

                            // Loop through items                           
                            angular.forEach(scope.invoice.items,
                                function(item) {
                                    if (keepFindingProduct) {
                                        if (lineItemDialogData.lineItem.sku === item.sku) {

                                            // Make an invoice AddItemsModel
                                            invoiceAddItems = {
                                                InvoiceKey: scope.invoice.key,
                                                LineItemType: lineItemDialogData.lineItemType,
                                                Items: [
                                                    {
                                                        OriginalSku: item.sku,
                                                        Quantity: 0
                                                    }
                                                ]
                                            };

                                            // Stop finding and break (As no break in angular loop, this is best way)
                                            keepFindingProduct = false;
                                        }
                                    }
                                });

                        } else {

                            // Just send everything up and we'll deal with it on the server      
                            // TODO - Need to manage all these on the server, check for product key
                            angular.forEach(scope.invoice.items,
                                function(item) {
                                    if (item.lineItemType === "Product" && item.key === lineItemDialogData.key) {
                                        // Make an invoice AddItemsModel
                                        invoiceAddItems = {
                                            InvoiceKey: scope.invoice.key,
                                            LineItemType: lineItemDialogData.lineItemType,
                                            Items: [
                                                {
                                                    Quantity: lineItemDialogData.quantity,
                                                    OriginalQuantity: item.quantity,
                                                    Sku: lineItemDialogData.sku,
                                                    OriginalSku: item.sku,
                                                    Name: lineItemDialogData.name,
                                                    OriginalName: item.name,
                                                    Price: lineItemDialogData.price,
                                                    OriginalPrice: item.price,
                                                    Key: item.key
                                                }
                                            ]
                                        }
                                    }
                                });
                        }

                        // Put the new items
                        var invoiceSavePromise = invoiceResource.putInvoiceNewProducts(invoiceAddItems);
                        invoiceSavePromise.then(function() {
                                $timeout(function() {
                                        scope.reload();
                                        loadInvoice();
                                        notificationsService.success('Invoice updated.');
                                    },
                                    1500);
                            },
                            function(reason) {
                                notificationsService.error("Failed", reason.data);
                            });


                    }

                    scope.deleteDiscount = function(discount) {

                        var dialogData = {
                            discount: discount
                        };

                        dialogService.open({
                            template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.delete.discount.html',
                            show: true,
                            dialogData: dialogData,
                            callback: removeDiscount
                        });

                    };

                    function removeDiscount(dialogData) {
                        var invoiceSavePromise = invoiceResource.deleteDiscount(scope.invoice.key, dialogData.discount.sku);
                        invoiceSavePromise.then(function () {
                                $timeout(function () {
                                        scope.reload();
                                        loadInvoice();
                                        notificationsService.success('Invoice updated.');
                                    },
                                    1500);
                            },
                            function (reason) {
                                notificationsService.error("Failed to update invoice", reason.message);
                            });
                    }

                    function loadInvoice() {
                        var taxLineItem = scope.invoice.getTaxLineItem();
                        scope.taxTotal = taxLineItem !== undefined ? taxLineItem.price : 0;
                        scope.shippingTotal = scope.invoice.shippingTotal();

                        scope.customLineItems = scope.invoice.getCustomLineItems();
                        scope.discountLineItems = scope.invoice.getDiscountLineItems();
                        scope.adjustmentLineItems = scope.invoice.getAdjustmentLineItems();

                        angular.forEach(scope.adjustmentLineItems,
                            function(item) {
                                item.userName = item.extendedData.getValue("userName");
                                item.email = item.extendedData.getValue("email");
                            });

                        scope.remainingBalance =
                            invoiceHelper.round(scope.invoice.remainingBalance(scope.allPayments), 2);

                        var label = scope.remainingBalance == '0'
                            ? 'merchelloOrderView_captured'
                            : 'merchelloOrderView_authorized';

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
            };
        }]);
