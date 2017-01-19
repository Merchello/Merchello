    /**
     * @ngdoc service
     * @name merchello.models.invoiceItemItemizationDisplayBuilder
     *
     * @description
     * A utility service that builds InvoiceItemItemizationDisplay models
     */
    angular.module('merchello.models')
        .factory('invoiceItemItemizationDisplayBuilder',
            ['genericModelBuilder','invoiceLineItemDisplayBuilder', 'InvoiceItemItemizationDisplay',
            function (genericModelBuilder, invoiceLineItemDisplayBuilder, InvoiceItemItemizationDisplay) {

                var Constructor = InvoiceItemItemizationDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var itemizations = genericModelBuilder.transform(jsonResult, Constructor);
                        if (angular.isArray(itemizations)) {
                            for(var i = 0; i < itemizations.length; i++) {
                                itemizations[ i ].adjustments = invoiceLineItemDisplayBuilder.transform(jsonResult[ i ].adjustments);
                                itemizations[ i ].custom = invoiceLineItemDisplayBuilder.transform(jsonResult[ i ].custom);
                                itemizations[ i ].discounts = invoiceLineItemDisplayBuilder.transform(jsonResult[ i ].discounts);
                                itemizations[ i ].products = invoiceLineItemDisplayBuilder.transform(jsonResult[ i ].products);
                                itemizations[ i ].shipping = invoiceLineItemDisplayBuilder.transform(jsonResult[ i ].shipping);
                                itemizations[ i ].tax = invoiceLineItemDisplayBuilder.transform(jsonResult[ i ].tax);
                            }
                        } else {
                            //jsonResult = JSON.stringify(jsonResult);
                            itemizations.adjustments = invoiceLineItemDisplayBuilder.transform(jsonResult.adjustments);
                            itemizations.custom = invoiceLineItemDisplayBuilder.transform(jsonResult.custom);
                            itemizations.discounts = invoiceLineItemDisplayBuilder.transform(jsonResult.discounts);
                            itemizations.products = invoiceLineItemDisplayBuilder.transform(jsonResult.products);
                            itemizations.shipping = invoiceLineItemDisplayBuilder.transform(jsonResult.shipping);
                            itemizations.tax = invoiceLineItemDisplayBuilder.transform(jsonResult.tax);
                        }
                        return itemizations;
                    }
                };
        }]);
