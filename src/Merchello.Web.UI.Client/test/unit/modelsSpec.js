'use strict';

/* jasmine specs for models go here */

describe("Mechello Models", function () {
    var $scope;

    beforeEach(inject(function ($rootScope) {
        $scope = $rootScope.$new();
    }));

    describe("Product.Models.ProductAttribute", function () {
        it("Should create an empty product attribute if there is not productAttributeFromServer", function () {
            var productatt = new merchello.Models.ProductAttribute();
            expect(productatt.key).toBe("");
        });
        it("Should create an populated product attribute if there is a productAttributeFromServer", function () {
            var pafs = { key: "123", optionKey: "321", name: "bane", sortOrder: 6, optionOrder: 7, isRemoved: true };
            var productatt = new merchello.Models.ProductAttribute(pafs);
            expect(productatt.key).toBe("123");
            expect(productatt.optionKey).toBe("321");
            expect(productatt.name).toBe("bane");
            expect(productatt.sortOrder).toBe(6);
            expect(productatt.optionOrder).toBe(6);       //Possible Issue: Needs Review
            expect(productatt.isRemoved).toBe(false);   //Possible Issue: Needs Review
        });
    });

    describe("Product.Models.ProductOption", function () {
        it("Should create an empty product option if there is not productOptionFromServer", function () {
            var productopt = new merchello.Models.ProductOption();
            expect(productopt.key).toBe("");
        });
        it("Should create an populated product option if there is a productOptionFromServer", function () {
            var pofs = { key: "123", name: "bane", required: "aye", sortOrder: 6};
            var productopt = new merchello.Models.ProductOption(pofs);
            expect(productopt.key).toBe("123");
            expect(productopt.required).toBe("aye");
            expect(productopt.name).toBe("bane");
            expect(productopt.sortOrder).toBe(6);
        });
    });

});