'use strict';

/* jasmine specs for models go here */

describe("Merchello customer models", function () {
    describe("Customer Model", function () {
        var customer;
        var cfs;
        var cafs;
        var cafsv2;

        beforeEach(function () {
            cafs = {
                address1: "123 here",
                address2: "1232 mare",
                addressType: "good",
                addressTypeFieldKey: "madress",
                company: "proworks",
                countryCode: "CA",
                customerKey: "#123",
                fullName: "The Doctor",
                isDefault: true,
                key: "#345",
                label: "we are never",
                locality: "EN-CA",
                phone: "541-867-5309",
                postalCode: "90001",
                region: "cold"
            };
            cafsv2 = _.extend({}, cafs, { address1: "456 there", address2: "4562 fair", isDefault:false});
            cfs = {
                addresses: [cafs, cafsv2],
                email: 'micah@example.com',
                extendedData: [{ data: "data" }],
                firstName: 'john',
                key: '#123',
                lastActivityDate: '11/11/11 11:11:11',
                lastName: 'smith',
                loginName: 'The Doctor',
                notes: 'Odd enough',
                taxExempt: true
            };
            customer = new merchello.Models.Customer(cfs);
        });

        it("should be populated with default stuff", function () {
            //var undefined = "WAT";
            customer = new merchello.Models.Customer();
            expect(customer).not.toBeUndefined();
            _.each(["email", "firstName", "lastActivityDate", "key", "lastName", "loginName", "notes"], function(i) {
                expect(customer[i]).toBe("");
            });

            expect(customer.addresses.length).toBe(0);
            expect(customer.extendedData.length).toBe(0);

            expect(customer.primaryLocation()).toBe("");
        });

        it("should be populated with data from source", function () {
            //var undefined = "WAT";
            expect(customer).not.toBeUndefined();
            _.each(["email", "firstName", "key", "lastActivityDate", "lastName", "loginName", "notes", "taxExempt"], function (i) {
                expect(customer[i]).toBe(cfs[i]);
            });

            expect(customer.addresses.length).toBe(2);
            expect(customer.extendedData.length).toBe(1);

            expect(customer.primaryLocation()).toBe(cafs.locality + ", " + cafs.region);

            cfs.addresses = [_.extend(cafs, { isDefault: false }), cafsv2];
            customer = new merchello.Models.Customer(cfs);
            expect(customer.primaryLocation()).toBe(cafs.locality + ", " + cafs.region);

            cfs.addresses = [_.extend(cafs, { isDefault: false }), _.extend(cafsv2, { isDefault: false })];
            customer = new merchello.Models.Customer(cfs);
            expect(customer.primaryLocation()).toBe(cafsv2.locality + ", " + cafsv2.region);
        });
    });

    describe("Cusomer Address", function () {
        var customerAddress;
        var cafs;

        beforeEach(function () {
            cafs = {
                address1: "123 here",
                address2: "1232 mare",
                addressType: "good",
                addressTypeFieldKey: "madress",
                company: "proworks",
                countryCode: "CA",
                customerKey: "#123",
                fullName: "The Doctor",
                isDefault: true,
                key: "#345",
                label: "we are never",
                locality: "EN-CA",
                phone: "541-867-5309",
                postalCode: "90001",
                region: "cold"
            };
            customerAddress = new merchello.Models.CustomerAddress(cafs);
        });

        it("Should Create a fully populated customer address", function () {
            expect(customerAddress).toBeDefined();
            _.each(_.keys(cafs), function (i) {
                expect(customerAddress[i]).toBe(cafs[i]);
            });
        });

        it("Should Create an empty customer address", function () {
            customerAddress = new merchello.Models.CustomerAddress();
            expect(customerAddress).toBeDefined();
            _.each(_.keys(cafs), function (i) {
                if (i !== "isDefault")
                    expect(customerAddress[i]).toBe("");
            });
        });
    });

    describe("Dictionary Item", function () {
        it("should create a dictionary item", function () {
            var dictItem = new merchello.Models.DictionaryItem({ key:"one", value:"thing" });
            expect(dictItem).toBeDefined();
            expect(dictItem.key).toBe("one");
            expect(dictItem.value).toBe("thing");
        });

        it("should create an empty dictionary item", function () {
            var dictItem = new merchello.Models.DictionaryItem();
            expect(dictItem).toBeDefined();
            expect(dictItem.key).toBe("");
            expect(dictItem.value).toBe("");
        });
    });
});