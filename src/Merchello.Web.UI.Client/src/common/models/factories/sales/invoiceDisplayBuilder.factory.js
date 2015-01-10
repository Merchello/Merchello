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
            'orderDisplayBuilder', 'InvoiceDisplay',
            function(genericModelBuilder, invoiceStatusDisplayBuilder, invoiceLineItemDisplayBuilder,
                     orderDisplayBuilder, InvoiceDisplay) {
                var Constructor = InvoiceDisplay;

                return {
                    createDefault: function() {
                        var invoice = new Constructor();
                        invoice.invoiceStatus = invoiceStatusDisplayBuilder.createDefault();
                        return invoice;
                    },
                    transform: function(jsonResult) {
                        var invoices = genericModelBuilder.transform(jsonResult, Constructor);
                        if (invoices.length) {
                            for(var i = 0; i < invoices.length; i++) {
                                invoices[ i ].invoiceStatus = invoiceStatusDisplayBuilder.transform(jsonResult[ i ].invoiceStatus);
                                invoices[ i ].items = invoiceLineItemDisplayBuilder.transform(jsonResult[ i ].items);
                                invoices[ i ].orders = orderDisplayBuilder.transform(jsonResult[ i ].orders);
                            }
                        } else {
                            //jsonResult = JSON.stringify(jsonResult);
                            invoices.invoiceStatus = invoiceStatusDisplayBuilder.transform(jsonResult.invoiceStatus);
                            invoices.items = invoiceLineItemDisplayBuilder.transform(jsonResult.items);
                            invoices.orders = orderDisplayBuilder.transform(jsonResult.orders);
                        }
                        return invoices;
                    }
                };
            }]);