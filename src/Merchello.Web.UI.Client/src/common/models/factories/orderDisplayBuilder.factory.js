    /**
     * @ngdoc service
     * @name merchello.services.gatewayResourceDisplayBuilder
     *
     * @description
     * A utility service that builds GatewayResourceDisplay models
     */
    angular.module('merchello.models')
        .factory('orderDisplayBuilder', 'orderStatusDisplayBuilder', 'orderLineItemDisplayBuilder',
        ['genericModelBuilder',
            function(genericModelBuilder, orderStatusDisplayBuilder, orderLineItemDisplayBuilder) {
                var Constructor = OrderDisplay;

                return {
                    createDefault: function() {
                        var order = new Constructor();
                        order.orderStatus = orderLineItemDisplayBuilder.createDefault();
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
