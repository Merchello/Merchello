'use strict';

describe('modelBuilder.service', function() {

    beforeEach(module('umbraco'));

    it ('should be able to build a Merchello.Model.Address populated with a jsonResult', inject(function(modelBuilder, addressMocks){
        var result = addressMocks.getRandomAddress();
        var address = modelBuilder.buildAddress(result);

        expect (address).toBeDefined();
        expect (address.name != '').toBe(true);
    }));
});
