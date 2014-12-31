    /**
     * @ngdoc service
     * @name merchello.services.orderLineItemDisplayBuilder
     *
     * @description
     * A utility service that builds OrderLineItemDisplay models
     */
    angular.module('merchello.models')
        .factory('orderLineItemDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'OrderLineItemDisplay',
            function(genericModelBuilder, extendedDataDisplayBuilder, OrderLineItemDisplay) {
                var Constructor = OrderLineItemDisplay;
                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var orderLineItems = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < orderLineItems.length; i++) {
                            orderLineItems[ i ].extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                        }
                        return orderLineItems;
                    }
                };
            }]);
