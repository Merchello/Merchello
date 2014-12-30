    /**
     * @ngdoc service
     * @name merchello.services.settingDisplayBuilder
     *
     * @description
     * A utility service that builds SettingDisplay models
     */
    angular.module('merchello.models')
        .factory('shipmentDisplayBuilder',
        ['genericModelBuilder',  'shipmentStatusDisplayBuilder', 'orderLineItemDisplayBuilder',
            function(genericModelBuilder, shipmentStatusBuilder, orderLineItemBuilder) {

                var Constructor = ShipmentDisplay;

                return {
                    createDefault: function() {
                        var shipment = new Constructor();
                        shipment.shipmentStatus = shipmentStatusBuilder.createDefault();
                        return shipment;
                    },
                    transform: function(jsonResult) {
                        var shipment = genericModelBuilder.transform(jsonResult, Constructor);
                        shipment.shipmentStatus = shipmentStatusBuilder.transform(jsonResult.shipmentStatus, ShipmentStatusDisplay);
                        shipment.items = _.map(jsonResult.items, function(orderLineItem) {
                          return orderLineItemBuilder.transform(orderLineItem, OrderLineItemDisplay);
                        });
                    }
                };
            }]);
