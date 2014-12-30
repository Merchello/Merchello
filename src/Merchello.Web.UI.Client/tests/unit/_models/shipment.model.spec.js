'use strict';

describe("ShipmentDisplay", function () {


    beforeEach(module('umbraco'));

    it ("should be possible to set the 'to' address",
        inject(function(shipmentDisplayBuilder, genericModelBuilder, addressMocks) {

        //// Arrange
        var shipment = shipmentDisplayBuilder.createDefault();
        var address = addressMocks.getRandomAddress(genericModelBuilder);

        //// Act
        shipment.setDestinationAddress(address);

        //// Assert
        expect (shipment.toName).toBe(address.name);
        expect (shipment.toAddress1).toBe(address.address1);
        expect (shipment.toAddress2).toBe(address.address2)
        expect (shipment.toLocality).toBe(address.locality);
        expect (shipment.toRegion).toBe(address.region);
        expect (shipment.toPostalCode).toBe(address.postalCode);
        expect (shipment.toCountryCode).toBe(address.countryCode);
        expect (shipment.phone).toBe(address.phone);
        expect (shipment.email).toBe(address.email);
    }));

    it ('should be possible to get the destination as an address', inject(function(shipmentDisplayBuilder, genericModelBuilder, addressMocks) {

        //// Arrange
        var shipment = shipmentDisplayBuilder.createDefault();
        var address = addressMocks.getRandomAddress(genericModelBuilder);
        shipment.setDestinationAddress(address);

        //// Act
        var destination = shipment.getDestinationAddress();

        //// Assert
        expect (destination.name).toBe(address.name);
        expect (destination.address1).toBe(address.address1);
        expect (destination.address2).toBe(address.address2)
        expect (destination.locality).toBe(address.locality);
        expect (destination.region).toBe(address.region);
        expect (destination.postalCode).toBe(address.postalCode);
        expect (destination.countryCode).toBe(address.countryCode);
        expect (destination.phone).toBe(address.phone);
        expect (destination.email).toBe(address.email);
    }));

    it ('should be possible to set the origin address', inject(function(shipmentDisplayBuilder, genericModelBuilder, addressMocks) {

        //// Arrange
        var shipment = shipmentDisplayBuilder.createDefault();
        var address = addressMocks.getRandomAddress(genericModelBuilder);

        //// Act
        shipment.setOriginAddress(address);

        //// Assert
        expect (shipment.fromName).toBe(address.name);
        expect (shipment.fromAddress1).toBe(address.address1);
        expect (shipment.fromAddress2).toBe(address.address2)
        expect (shipment.fromLocality).toBe(address.locality);
        expect (shipment.fromRegion).toBe(address.region);
        expect (shipment.fromPostalCode).toBe(address.postalCode);
        expect (shipment.fromCountryCode).toBe(address.countryCode);
    }));

    it ('should be possible to get the origin address from a shipment', inject(function(shipmentDisplayBuilder, genericModelBuilder, addressMocks) {

        //// Arrange
        var shipment = shipmentDisplayBuilder.createDefault();
        var address = addressMocks.getRandomAddress(genericModelBuilder);
        shipment.setOriginAddress(address);

        //// Act
        var origin = shipment.getOriginAddress();

        //// Assert
        expect (origin.name).toBe(address.name);
        expect (origin.address1).toBe(address.address1);
        expect (origin.address2).toBe(address.address2)
        expect (origin.locality).toBe(address.locality);
        expect (origin.region).toBe(address.region);
        expect (origin.postalCode).toBe(address.postalCode);
        expect (origin.countryCode).toBe(address.countryCode);
        expect (origin.phone).toBe(address.phone);
        expect (origin.email).toBe(address.email);
    }));
});
