    /**
     * @ngdoc service
     * @name merchello.services.shipmentDisplayBuilder
     *
     * @description
     * A utility service that builds ShipmentDisplay models
     */
    angular.module('merchello.models')
        .factory('shipmentDisplayBuilder',
        ['genericModelBuilder',  'shipmentStatusDisplayBuilder', 'orderLineItemDisplayBuilder',
            function(genericModelBuilder, shipmentStatusBuilder, orderLineItemBuilder) {

                var Constructor = ShipmentDisplay;

                return {
                    // TODO the default warehouse address (AddressDisplay) could be saved as a config value
                    // and then added to the shipment origin address
                    createDefault: function() {
                        var shipment = new Constructor();
                        shipment.shipmentStatus = shipmentStatusBuilder.createDefault();
                        return shipment;
                    },
                    transform: function(jsonResult) {
                        // the possible list of shipments
                        var shipments = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < jsonResult.length; i++) {
                            // each shipment has a ShipmentStatusDisplay
                            shipments[ i ].shipmentStatus = shipmentStatusBuilder.transform(jsonResult[ i ].shipmentStatus, ShipmentStatusDisplay);
                            // add the OrderLineItemDisplay(s) associated with the shipment
                            for(var j = 0; j < jsonResult[ i ].items.length; j++)
                            {
                                // did this manually so that we did not create functions within a loop
                                shipments[ i ].items.push(orderLineItemBuilder.transform(jsonResult[ i ].items[ j ], OrderLineItemDisplay));
                            }
                        }
                        return shipments;
                    }
                };
            }]);
