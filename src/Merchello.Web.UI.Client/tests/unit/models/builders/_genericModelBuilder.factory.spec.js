'use strict';

describe("genericModelBuilder.service", function () {

    beforeEach(module('umbraco'));

    describe('address transformation tests', function() {

        it ('should transform an array of json addresses to an array of Merchello.Models.Address',
            inject(function(genericModelBuilder, addressMocks, AddressDisplay) {

                // Arrange
                var constructor = AddressDisplay;
                var addressArray = addressMocks.getAddressArray();

                //// Act
                var addresses = genericModelBuilder.transform(addressArray, AddressDisplay);

                //// Assert
                expect (typeof(addresses)).toBe('object');
                expect (addresses.length).toBe(addressArray.length);
        }));

        it ('address should not contain invalid properties',
            inject(function(genericModelBuilder, AddressDisplay) {
                // Arrange
                var constructor = AddressDisplay;
                var badAddress = {
                    name : 'Mindfly',
                    address1: '114 W. Magnolia St.',
                    address2: 'Suite 300',
                    locality: 'Bellingham',
                    region : 'WA',
                    postalCode:  '98225',
                    countryCode: 'US',

                    // bad data
                    //property1: 'should not be here',
                    //property2: 'should not be here'
                };

                //// Act
                var address = genericModelBuilder.transform(badAddress, constructor);

                //// Assert
                expect (typeof(address)).toBe('object');
                expect (address.name).toBe('Mindfly');
                expect (address.address1).toBe('114 W. Magnolia St.');
                expect (address.address2).toBe('Suite 300');
                expect (address.locality).toBe('Bellingham');
                expect (address.region).toBe('WA');
                expect (address.postalCode).toBe('98225')
                expect (address.countryCode).toBe('US');

                expect (address.property1).toBeUndefined();
                expect (address.property2).toBeUndefined();
            }));
    });
});
