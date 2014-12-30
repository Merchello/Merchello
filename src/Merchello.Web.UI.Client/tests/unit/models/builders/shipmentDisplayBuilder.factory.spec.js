    'use strict';

    describe("shipmentDisplayBuilder", function () {
        beforeEach(module('umbraco'));

        it ('should be able to create a default ShipmentDisplay (an empty AddressDisplay)', inject(function(shipmentDisplayBuilder) {

            //// Arrange
            var expected = new ShipmentDisplay();
            expected.shipmentStatus = new ShipmentStatusDisplay();

            //// Act
            var shipment = shipmentDisplayBuilder.createDefault();

            //console.info(shipment);
            expect (shipment).toBeDefined();
            expect (_.isEqual(expected, shipment)).toBe(true);
        }));
    });
