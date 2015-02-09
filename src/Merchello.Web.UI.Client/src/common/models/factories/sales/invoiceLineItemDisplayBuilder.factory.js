    /**
     * @ngdoc service
     * @name merchello.models.invoiceLineItemDisplayBuilder
     *
     * @description
     * A utility service that builds InvoiceLineItemDisplay models
     */
    angular.module('merchello.models')
        .factory('invoiceLineItemDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'typeFieldDisplayBuilder', 'InvoiceLineItemDisplay',
            function(genericModelBuilder, extendedDataDisplayBuilder, typeFieldDisplayBuilder, InvoiceLineItemDisplay) {
                var Constructor = InvoiceLineItemDisplay;
                return {
                    createDefault: function() {
                        var invoiceLineItem = new Constructor();
                        invoiceLineItem.lineItemTypeField = typeFieldDisplayBuilder.createDefault();
                        invoiceLineItem.extendedData = extendedDataDisplayBuilder.createDefault();
                        return invoiceLineItem;
                    },
                    transform: function(jsonResult) {
                        var invoiceLineItems = genericModelBuilder.transform(jsonResult, Constructor);
                        if(angular.isArray(invoiceLineItems)) {
                            for(var i = 0; i < invoiceLineItems.length; i++) {
                                invoiceLineItems[ i ].extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                                invoiceLineItems[ i ].lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].lineItemTypeField);
                            }
                        } else {
                            invoiceLineItems.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                            invoiceLineItems.lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult.lineItemTypeField);
                        }
                        return invoiceLineItems;
                    }
                };
            }]);
