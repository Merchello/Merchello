'use strict';

describe('AddressDisplay prototype methods', function() {

    beforeEach(module('umbraco'));

    it('isEmpty should return a value of true if the address is empty', inject(function(addressDisplayBuilder) {
        //// Arrange
        var address = addressDisplayBuilder.createDefault();

        //// Act
        var value = address.isEmpty();

        //// expect
        expect (value).toBe(true);
    }));

});
