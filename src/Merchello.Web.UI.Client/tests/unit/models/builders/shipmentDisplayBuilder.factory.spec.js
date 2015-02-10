    'use strict';

    describe("shipmentDisplayBuilder", function () {
        beforeEach(module('umbraco'));

        it ('should be able to create a default ShipmentDisplay (an empty AddressDisplay)', inject(function(shipmentDisplayBuilder, ShipmentDisplay, ShipmentStatusDisplay) {

            //// Arrange
            var expected = new ShipmentDisplay();
            expected.shipmentStatus = new ShipmentStatusDisplay();

            //// Act
            var shipment = shipmentDisplayBuilder.createDefault();

            //console.info(shipment);
            expect (shipment).toBeDefined();
            expect (_.isEqual(expected, shipment)).toBe(true);
        }));

        it('should be able to transform results from ShipmentApiService into an array of ShipmentDisplay', inject(function(shipmentDisplayBuilder, shipmentMocks) {
           //// Arrange
            var results = shipmentMocks.shipmentsArray();

            //// Act
            var shipments = shipmentDisplayBuilder.transform(results);

            ///// Assert
            expect(results.length).toBe(shipments.length);

        }));
    });
