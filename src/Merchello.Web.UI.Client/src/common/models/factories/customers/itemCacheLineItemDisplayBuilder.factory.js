    /**
     * @ngdoc service
     * @name merchello.models.itemCacheLineItemDisplayBuilder
     *
     * @description
     * A utility service that builds ItemCacheLineItemDisplay models
     */
    angular.module('merchello.models')
        .factory('itemCacheLineItemDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'typeFieldDisplayBuilder', 'ItemCacheLineItemDisplay',
            function(genericModelBuilder, extendedDataDisplayBuilder, typeFieldDisplayBuilder, ItemCacheLineItemDisplay) {
                var Constructor = ItemCacheLineItemDisplay;
                return {
                    createDefault: function() {
                        var lineItem = new Constructor();
                        lineItem.lineItemTypeField = typeFieldDisplayBuilder.createDefault();
                        lineItem.extendedData = extendedDataDisplayBuilder.createDefault();
                        return lineItem;
                    },
                    transform: function(jsonResult) {
                        var lineItems = genericModelBuilder.transform(jsonResult, Constructor);
                        if(angular.isArray(lineItems)) {
                            for(var i = 0; i < lineItems.length; i++) {
                                lineItems[ i ].extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                                lineItems[ i ].lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].lineItemTypeField);
                            }
                        } else {
                            lineItems.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                            lineItems.lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult.lineItemTypeField);
                        }
                        return lineItems;
                    }
                };
            }]);
