'use strict';

/* jasmine specs for models go here */
describe("Taxation Models", function () { 
    describe("Tax Country Model", function () {
        var taxCountry;
        var tcfs;
    
        beforeEach(function () {
            tcfs = {
                name: "Everywhere Else",
                serviceCode: "KEY",
                sortHelper: "Here are things."
            }
    
            taxCountry = new merchello.Models.TaxCountry(tcfs);
        });
    
        it("should create an empty Tax Country Object", function () {
            taxCountry = new merchello.Models.TaxCountry();
            expect(taxCountry).toBeDefined();
            for (var i in tcfs) {
                if (typeof (tcfs[i]) != "object" && i != "sortHelper")
                    expect(taxCountry[i]).toBeFalsy();
            }

            expect(taxCountry["countryName"]).toBe("");
            expect(taxCountry["country"]).toBeDefined();
            expect(taxCountry["sortHelper"]).toBe("0");
            expect(taxCountry["method"]["countryCode"]).toBe("");
        });
    
        it("should create a populated Tax Country Object", function () {
            expect(taxCountry).toBeDefined();
            for (var i in tcfs) {
                if (typeof (tcfs[i]) != "object" && i != "sortHelper")
                    expect(taxCountry[i]).toBe(tcfs[i]);
            }
            expect(taxCountry["countryName"]).toBe("");
            expect(taxCountry["country"]).toBeDefined();
            expect(taxCountry["sortHelper"]).toBe("1" + tcfs.name);
            expect(taxCountry["method"]["countryCode"]).toBe(tcfs.serviceCode);
        });

        it("should set the Country Name", function () {
            taxCountry.setCountryName("bofus");
            expect(taxCountry["countryName"]).toBe("bofus");
            expect(taxCountry["sortHelper"]).toBe("0bofus");
        });
    });

    describe("Tax Province Model", function () {
        var taxProvince;
        var tpfs;

        beforeEach(function () {
            tpfs = {
                name: "Dark with a subtle hint of rain",
                code: "KEY",
                percentAdjustment: "as in?"
            }

            taxProvince = new merchello.Models.TaxProvince(tpfs);
        });

        it("should create an empty Tax Province Object", function () {
            taxProvince = new merchello.Models.TaxProvince();
            expect(taxProvince).toBeDefined();
            for (var i in tpfs) {
                if (typeof (tpfs[i]) != "object")
                    expect(taxProvince[i]).toBeFalsy();
            }
        });

        it("should create a populated Tax Province Object", function () {
            expect(taxProvince).toBeDefined();
            for (var i in tpfs) {
                if (typeof (tpfs[i]) != "object")
                    expect(taxProvince[i]).toBe(tpfs[i]);
            }
        });
    });

    describe("Tax Method Model", function () {
        var taxMethod;
        var tmfs;

        beforeEach(function () {
            tmfs = {
                key: "9sdkifoj43t9jdfl",
                name: "Dark with a subtle hint of rain",
                providerKey: "KEY",
                countryCode: "KG",
                percentageTaxRate: 3.45,
                provinces: [{}, {}],
                dialogEditorView: {title: "thing", description: "right", editorView:"cool"}
            }

            taxMethod = new merchello.Models.TaxMethod(tmfs);
        });

        it("should create an empty Tax Method Object", function () {
            taxMethod = new merchello.Models.TaxMethod();
            expect(taxMethod).toBeDefined();
            for (var i in tmfs) {
                if (typeof (tmfs[i]) != "object")
                    expect(taxMethod[i]).toBeFalsy();
            }
            expect(taxMethod.provinces.length).toBe(0);
        });

        it("should create a populated Tax Method Object", function () {
            expect(taxMethod).toBeDefined();
            for (var i in tmfs) {
                if (typeof (tmfs[i]) != "object")
                    expect(taxMethod[i]).toBe(tmfs[i]);
            }
            expect(taxMethod.provinces.length).toBe(2);
        });

        it("should return the editor view", function () {
            expect(taxMethod.displayEditor()).toBe("cool");
        });
    });
});