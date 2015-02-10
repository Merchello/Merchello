'use strict';

    describe("addressDisplayBuilder", function () {

        beforeEach(module('umbraco'));
/*
        beforeEach(function() {
            this.addMatchers({
                toBeLengthOf: function(count) {
                    this.message = function() {
                        return 'Actual length was ' + this.actual;
                    }
                    return this.actual === count;
                }
            });
        });
*/
        it ('should be able to create a default AddressDisplay (an empty AddressDisplay)', inject(function(addressDisplayBuilder, AddressDisplay) {
            //// Arrange
            var expected = new AddressDisplay();

            //// Act
            var address = addressDisplayBuilder.createDefault();
            //console.info(address);

            //// Assert
            expect (address).toBeDefined();
            expect (expected).toEqual(address);
        }));

        it ('should populate an addresses with a transform from a json result', inject(function(addressDisplayBuilder, addressMocks) {
            //// Arrange
            var jsonResult = addressMocks.getAddressArray();
            var length = jsonResult.length;

            //// Act
            var addresses = addressDisplayBuilder.transform(jsonResult);

            //// Assert
            //expect (addresses.length).toBeLengthOf(length);
            expect(jsonResult).not.toEqual(addresses)
            expect(addresses[0].name).toBe(jsonResult[0].name);
        }));
    });
