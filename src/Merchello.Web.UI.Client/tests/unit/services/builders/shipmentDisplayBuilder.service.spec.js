    'use strict';

    describe("shipmentDisplayBuilder", function () {
        beforeEach(module('umbraco'));

        it ('should be able to create a default ShipmentDisplay (an empty AddressDisplay)', inject(function(shipmentDisplayBuilder) {

            var shipment = shipmentDisplayBuilder.createDefault();

            console.info(shipment);
            expect (shipment).toBeDefined();
        }));
    });
