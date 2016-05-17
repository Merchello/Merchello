angular.module('merchello.directives').directive('invoiceItemizationTable',
    ['localizationService', 'invoiceHelper',
        function(localizationService, invoiceHelper) {

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

                    function init() {

                        // ensure that the parent scope promises have been resolved
                        scope.$watch('preValuesLoaded', function(pvl) {
                            if(pvl === true) {
                               loadInvoice();
                            }
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

                        scope.remainingBalance =
                            invoiceHelper.round(scope.invoice.remainingBalance(scope.allPayments), 2);

                        var label  = scope.remainingBalance == '0' ? 'merchelloOrderView_captured' : 'merchelloOrderView_authorized';

                        localizationService.localize(label).then(function(value) {
                            scope.authorizedCapturedLabel = value;
                        });

                        scope.loaded = true;
                    }


                    // utility method to assist in building scope line item collections
                    function aggregateScopeLineItemCollection(lineItems, collection) {
                        if(angular.isArray(lineItems)) {
                            angular.forEach(lineItems, function(item) {
                                collection.push(item);
                            });
                        } else {
                            if(lineItems !== undefined) {
                                collection.push(lineItems);
                            }
                        }
                    }
                    
                    // initialize the directive
                    init();
                }
            }
        }]);
