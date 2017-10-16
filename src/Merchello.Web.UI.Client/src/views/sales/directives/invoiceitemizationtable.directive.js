angular.module('merchello.directives').directive('invoiceItemizationTable',
    ['$q', 'localizationService', 'invoiceResource', 'invoiceHelper', 'dialogService', 'productResource',
        function ($q, localizationService, invoiceResource, invoiceHelper, dialogService, productResource) {
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
                    save: '&'
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
                    scope.lineItemPreview = function (sku) {

                        // Setup the dialog data
                        var dialogData = {
                            product: {},
                            sku: sku
                        };

                        // Get the product if it exists! We call the vairant service as this seems
                        // to return the base product too
                        productResource.getVariantBySku(sku).then(function (result) {
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
                    }


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
