'use strict';

/* jasmine specs for models go here */

describe("Mechello Address Model", function () {
    var $scope;

    beforeEach(inject(function ($rootScope) {
        var address = new Merchello.Models.Address();
    }));

    describe("Address property assignment", function() {

        it("Should create a new Address and be able to set properties", function() {
            
            expect(address.name).toBe('Rusty');
        });

    });

});