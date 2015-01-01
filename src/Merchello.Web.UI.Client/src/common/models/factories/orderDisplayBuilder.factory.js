    /**
     * @ngdoc service
     * @name merchello.models.gatewayResourceDisplayBuilder
     *
     * @description
     * A utility service that builds GatewayResourceDisplay models
     */
    angular.module('merchello.models')
        .factory('orderDisplayBuilder',
        ['genericModelBuilder', 'orderStatusDisplayBuilder', 'orderLineItemDisplayBuilder', 'OrderDisplay',
            function(genericModelBuilder, orderStatusDisplayBuilder, orderLineItemDisplayBuilder, OrderDisplay) {
                var Constructor = OrderDisplay;

                return {
                    createDefault: function() {
                        var order = new Constructor();
                        order.orderStatus = orderStatusDisplayBuilder.createDefault();
                        return order;
                    },
                    transform: function(jsonResult) {
                        var orders = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < orders.length; i++) {
                            orders[ i ].orderStatus = orderStatusDisplayBuilder.transform(jsonResult[ i ].orderStatus);
                            orders[ i ].items = orderLineItemDisplayBuilder.transform(jsonResult[ i ].items);
                        }
                        return orders;
                    }
                };
            }]);
