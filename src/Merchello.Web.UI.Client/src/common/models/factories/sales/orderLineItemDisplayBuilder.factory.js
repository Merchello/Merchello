    /**
     * @ngdoc service
     * @name merchello.models.orderLineItemDisplayBuilder
     *
     * @description
     * A utility service that builds OrderLineItemDisplay models
     */
    angular.module('merchello.models')
        .factory('orderLineItemDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'typeFieldDisplayBuilder', 'OrderLineItemDisplay',
            function(genericModelBuilder, extendedDataDisplayBuilder, typeFieldDisplayBuilder, OrderLineItemDisplay) {
                var Constructor = OrderLineItemDisplay;
                return {
                    createDefault: function() {
                        var orderLineItem = new Constructor();
                        orderLineItem.extendedData = extendedDataDisplayBuilder.createDefault();
                        orderLineItem.lineItemTypeField = typeFieldDisplayBuilder.createDefault();
                        return orderLineItem;
                    },
                    transform: function(jsonResult) {
                        var orderLineItems = genericModelBuilder.transform(jsonResult, Constructor);
                        if (orderLineItems.length) {
                            for (var i = 0; i < orderLineItems.length; i++) {
                                orderLineItems[i].extendedData = extendedDataDisplayBuilder.transform(jsonResult[i].extendedData);
                                orderLineItems[i].lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult[i].lineItemTypeField);
                            }
                        } else {
                            orderLineItems.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                            orderLineItems.lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult.lineItemTypeField);
                        }
                        return orderLineItems;
                    }
                };
            }]);
