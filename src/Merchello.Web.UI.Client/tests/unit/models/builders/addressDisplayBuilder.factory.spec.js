'use strict';

    describe("addressDisplayBuilder", function () {
        beforeEach(module('umbraco'));

        it ('should be able to create a default AddressDisplay (an empty AddressDisplay)', inject(function(addressDisplayBuilder) {
            //// Arrange
            var expected = new AddressDisplay();

            //// Act
            var address = addressDisplayBuilder.createDefault();
            //console.info(address);

            //// Assert
            expect (address).toBeDefined();
            expect (_.isEqual(expected, address)).toBe(true);
        }));

        it ('should populate an addresses with a transform from a json result', inject(function(addressDisplayBuilder, addressMocks) {
            //// Arrange
            var jsonResult = addressMocks.getAddressArray();

            //// Act
            var addresses = addressDisplayBuilder.transform(jsonResult);

            //// Assert
            expect (addresses.length).toBe(jsonResult.length);
            expect (_.isEqual(jsonResult, addresses)).toBe(false); // more default properties exist on AddressDisplay
            expect(addresses[0].name).toBe(jsonResult[0].name);
        }));
    });
