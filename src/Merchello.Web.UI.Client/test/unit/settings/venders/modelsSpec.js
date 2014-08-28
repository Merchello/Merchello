'use strict';

/* jasmine specs for models go here */

describe("Vendor Models", function () {
    describe("Vendor Model", function () {
        var vendor;
        var vfs;

        beforeEach(function () {
            vfs = {
                key: "Dark with a subtle hint of rain",
                name: "KEYSTONE Systems",
                contact: "Jon Smith",
                phone: "541 555 5554",
                address1: "privet drive",
                address2: "Ehhh",
                locality: "EN-UK",
                region: "122",
                postalCode: "813456",
                country: "UK"
            }

            vendor = new merchello.Models.Vendor(vfs);
        });

        it("should create an empty Vendor Object", function () {
            vendor = new merchello.Models.Vendor();
            expect(vendor).toBeDefined();
            for (var i in vfs) {
                if (typeof (vfs[i]) != "object")
                    expect(vendor[i]).toBeFalsy();
            }
        });

        it("should create a populated Vendor Object", function () {
            expect(vendor).toBeDefined();
            for (var i in vfs) {
                if (typeof (vfs[i]) != "object")
                    expect(vendor[i]).toBe(vfs[i]);
            }
        });
    });
});