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
                        if (angular.isArray(orders)) {
                            for(var i = 0; i < orders.length; i++) {
                                orders[ i ].orderStatus = orderStatusDisplayBuilder.transform(jsonResult[ i ].orderStatus);
                                orders[ i ].items = orderLineItemDisplayBuilder.transform(jsonResult[ i ].items);
                            }
                        } else {
                            if (jsonResult.orderStatus) {
                                orders.orderStatus = orderStatusDisplayBuilder.transform(jsonResult.orderStatus);
                            }
                            if (jsonResult.items) {
                                orders.items = orderLineItemDisplayBuilder.transform(jsonResult.items);
                            }
                        }
                        return orders;
                    }
                };
            }]);
