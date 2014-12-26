'use strict';

/* jasmine specs for models go here */

describe("Merchello.Models.Address tests", function () {
    var address;

    beforeEach(inject(function () {
        address = new Merchello.Models.Address();
    }));

    describe("Address property assignment", function() {

        it("Should create a new Address and be able to set properties", function() {
            address.name = 'Rusty';
            expect(address.name).toBe('Rusty');
        });

    });

});