'use strict';

    describe("addressDisplayBuilder", function () {
        beforeEach(module('umbraco'));

        it ('should be able to create a default AddressDisplay (an empty AddressDisplay)', inject(function(addressDisplayBuilder) {
            //// Arrange
            // nothing to do

            //// Act
            var address = addressDisplayBuilder.createDefault();
            console.info(address);

            //// Assert
            expect (address).toBeDefined();
            expect (address.name).toBe('');
            expect (address.address1).toBe('');
            expect (address.address2).toBe('');
            expect (address.locality).toBe('');
            expect (address.region).toBe('');
            expect (address.postalCode).toBe('');
            expect (address.countryCode).toBe('');
            expect (address.addressType).toBe('');
            expect (address.organization).toBe('');
            expect (address.phone).toBe('');
            expect (address.email).toBe('');
            //expect (address.isCommercial).toBeDefined();
        }));


    });
