    /**
     * @ngdoc service
     * @name merchello.models.invoiceDisplayBuilder
     *
     * @description
     * A utility service that builds InvoiceDisplay models
     */
    angular.module('merchello.models')
        .factory('invoiceDisplayBuilder',
        ['genericModelBuilder', 'invoiceStatusDisplayBuilder', 'invoiceLineItemDisplayBuilder',
            'orderDisplayBuilder', 'currencyDisplayBuilder', 'noteDisplayBuilder', 'InvoiceDisplay',
            function(genericModelBuilder, invoiceStatusDisplayBuilder, invoiceLineItemDisplayBuilder,
                     orderDisplayBuilder, currencyDisplayBuilder, noteDisplayBuilder, InvoiceDisplay) {
                var Constructor = InvoiceDisplay;

                return {
                    createDefault: function() {
                        var invoice = new Constructor();
                        invoice.invoiceStatus = invoiceStatusDisplayBuilder.createDefault();
                        invoice.currency = currencyDisplayBuilder.createDefault();
                        return invoice;
                    },
                    transform: function(jsonResult) {
                        var invoices = genericModelBuilder.transform(jsonResult, Constructor);
                        if (angular.isArray(invoices)) {
                            for (var i = 0; i < invoices.length; i++) {
                                if (invoices[i].key !== "") {
                                    invoices[i].invoiceStatus = invoiceStatusDisplayBuilder.transform(jsonResult[i].invoiceStatus);
                                    invoices[i].items = invoiceLineItemDisplayBuilder.transform(jsonResult[i].items);
                                    invoices[i].orders = orderDisplayBuilder.transform(jsonResult[i].orders);
                                    invoices[i].currency = currencyDisplayBuilder.transform(jsonResult[i].currency);
                                    invoices[i].notes = noteDisplayBuilder.transform(jsonResult[i].notes);
                                }
                            }
                        } else {
                            //jsonResult = JSON.stringify(jsonResult);
                            invoices.invoiceStatus = invoiceStatusDisplayBuilder.transform(jsonResult.invoiceStatus);
                            invoices.items = invoiceLineItemDisplayBuilder.transform(jsonResult.items);
                            invoices.orders = orderDisplayBuilder.transform(jsonResult.orders);
                            invoices.currency = currencyDisplayBuilder.transform(jsonResult.currency);
                            invoices.notes = noteDisplayBuilder.transform(jsonResult.notes);
                        }
                        return invoices;
                    }
                };
            }]);