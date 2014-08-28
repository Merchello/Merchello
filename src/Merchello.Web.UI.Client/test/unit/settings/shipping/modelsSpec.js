'use strict';

/* jasmine specs for models go here */

describe("Shipping Models", function () {
    
    describe("Province Data Model", function () {
        var provinceData;
        var pdfs;

        beforeEach(function () {
            pdfs = {
                name: "Dr Jekyll",
                code: "KDLE@34#mSk23",
                allowShipping: true,
                rateAdjustment: "yeas",
                rateAdjustmentType: 1223
            }

            provinceData = new merchello.Models.ProvinceData(pdfs);
        });

        it("should create a new and populated Province Data Object", function () {
            expect(provinceData).toBeDefined();
            for (var i in pdfs) {
                expect(provinceData[i]).toBe(pdfs[i]);
            }
        });

        it("should create a new and non-populated Province Data Object", function () {
            provinceData = new merchello.Models.ProvinceData();
            expect(provinceData).toBeDefined();
            for (var i in pdfs) {
                if(typeof(pdfs[i]) != "number")
                    expect(provinceData[i]).toBeFalsy();
            }
            expect(provinceData["rateAdjustmentType"]).toBe(1);
        });
    });

    describe("Shipping Country Model", function () {
        var shippingCountry;
        var scfs;

        beforeEach(function () {
            scfs = {
                name: "Dr Jekyll",
                catalogKey: "KDLE@34#mSk23",
                countryCode: "CA",
                provinces: [{ name: "province-one", code: "NVM" }, { name: "province-two", code: "ROLF" }],
                shippingGatewayProviders: ["asodnasd", "Isn't it weird how this isn't populated?"], //Possible issue
                sortHelper: "1232"
            }

            shippingCountry = new merchello.Models.ShippingCountry(scfs);
        });

        it("should create a new and populated Shipping Country Object", function () {
            expect(shippingCountry).toBeDefined();
            for (var i in scfs) {
                if (typeof (scfs[i]) != "object" && i != "sortHelper")
                    expect(shippingCountry[i]).toBe(scfs[i]);
            }
            
            expect(shippingCountry["provinces"].length).toBe(2);
            expect(shippingCountry["shippingGatewayProviders"].length).toBe(0);

            expect(shippingCountry["sortHelper"]).toBe("0" + scfs.name);
            shippingCountry = new merchello.Models.ShippingCountry(_.extend({}, scfs, { name: "Everywhere Else" }));
            expect(shippingCountry["sortHelper"]).toBe("1Everywhere Else");
        });

        it("should create a new and non-populated Shipping Country Object", function () {
            shippingCountry = new merchello.Models.ShippingCountry();
            expect(shippingCountry).toBeDefined();
            for (var i in scfs) {
                if (typeof (scfs[i]) != "object" && i != "sortHelper")
                    expect(shippingCountry[i]).toBeFalsy();
            }
            expect(shippingCountry["sortHelper"]).toBe("0");
            expect(shippingCountry["provinces"].length).toBe(0);
            expect(shippingCountry["shippingGatewayProviders"].length).toBe(0);
        });

        it("should recieve a country and set the country name, code and provinces", function () {
            var country = {countryCode: "RU", name:"Russia", provinces:[{ name: "cold-one", code: "OUCH" }, { name: "cold-two", code: "WHY?" }]}
            shippingCountry.fromCountry(country);
            expect(shippingCountry.name).toBe(country.name);
            expect(shippingCountry.countryCode).toBe(country.countryCode);
            expect(shippingCountry.provinces.length).toBe(2);
            expect(shippingCountry.provinces[0].name).toBe("cold-one");
        });
    });

    describe("Shipping Gateway Provider Model", function () {
        var shippingGatewayProvider;
        var sgpfs;
        var smfs;

        beforeEach(function () {

            smfs = {
                key: "a334",
                name: "ship it!",
                shipCountryKey: "DE",
                providerKey: "2341",
                shipMethodTfKey: "asd3asd",
                surcharge: "334wedasd",
                serviceCode: "234eqwse",
                taxable: true,
                provinces : ["OANSDLASKDN", "ASODANSD"],
                dialogEditorView: new merchello.Models.DialogEditorView(),
            }

            sgpfs = {
                key: "123",
                name: "Geroffy",
                typeFullName: "Geroffy The Illustrious Liar",
                shipMethods: [smfs]
            }

            shippingGatewayProvider = new merchello.Models.ShippingGatewayProvider(sgpfs);
        });

        it("should create a new and populated Shipping Gateway Provider", function () {
            expect(shippingGatewayProvider).toBeDefined();
            for (var i in sgpfs) {
                if (typeof (sgpfs[i]) != "object")
                    expect(shippingGatewayProvider[i]).toBe(sgpfs[i]);
            }
            expect(shippingGatewayProvider.shipMethods.length).toBe(1);
            expect(shippingGatewayProvider.shipMethods[0].key).toBe("a334");
        });

        it("should create a new and non-populated Shipping Gateway Provider", function () {
            shippingGatewayProvider = new merchello.Models.ShippingGatewayProvider();
            expect(shippingGatewayProvider).toBeDefined();
            for (var i in sgpfs) {
                if (typeof (sgpfs[i]) != "object")
                    expect(shippingGatewayProvider[i]).toBeFalsy();
            }
            expect(shippingGatewayProvider.shipMethods.length).toBe(0);
        });

        it("should add a new method to the provider", function () {
            expect(shippingGatewayProvider.addMethod(smfs));
            expect(shippingGatewayProvider.shipMethods.length).toBe(2);
            expect(shippingGatewayProvider.addMethod(smfs));
            expect(shippingGatewayProvider.shipMethods.length).toBe(3);
        });

        it("should remove any matching methods from the provider", function () {
            expect(shippingGatewayProvider.removeMethod(smfs));
            expect(shippingGatewayProvider.shipMethods.length).toBe(0);
        });

        it("should return if it is a fixed rate or not", function () {
            expect(shippingGatewayProvider.isFixedRate()).toBe(false);
            shippingGatewayProvider.key = "aec7a923-9f64-41d0-b17b-0ef64725f576";
            expect(shippingGatewayProvider.isFixedRate()).toBe(true);
        });

    });

    describe("Shipping Method Model", function () {
        var shippingMethod;
        var shfs;
        var pdfs;

        beforeEach(function () {
            pdfs = {
                name: "Dr Jekyll",
                code: "KDLE@34#mSk23",
                allowShipping: true,
                rateAdjustment: "yeas",
                rateAdjustmentType: 1223
            }
            shfs = {
                key: "a334",
                name: "ship it!",
                shipCountryKey: "DE",
                providerKey: "2341",
                shipMethodTfKey: "asd3asd",
                surcharge: "334wedasd",
                serviceCode: "234eqwse",
                taxable: true,
                provinces: [pdfs],
                dialogEditorView: { title: "22a", description: "dialogs stuff", editorView: "Do somethings" },
            }

            shippingMethod = new merchello.Models.ShippingMethod(shfs);
        });

        it("should create a new and populated Shipping Method", function () {
            expect(shippingMethod).toBeDefined();
            for (var i in shfs) {
                if (typeof (shfs[i]) != "object")
                    expect(shippingMethod[i]).toBe(shfs[i]);
            }
            expect(shippingMethod.provinces.length).toBe(1);
            expect(shippingMethod.provinces[0].name).toBe(pdfs.name);
        });

        it("should create a new and non-populated Shipping Method", function () {
            shippingMethod = new merchello.Models.ShippingMethod();
            expect(shippingMethod).toBeDefined();
            for (var i in shfs) {
                if (typeof (shfs[i]) != "object")
                    expect(shippingMethod[i]).toBeFalsy();
            }
            expect(shippingMethod.provinces.length).toBe(0);
        });

        it("should add a new providence, or just an empty shipping region", function () {
            shippingMethod.addProvince(_.extend({}, pdfs, {key:"wut key?"}));
            expect(shippingMethod.provinces.length).toBe(2);
            shippingMethod.addProvince(_.extend({}, pdfs, { key: "wut key?" }));
            expect(shippingMethod.provinces.length).toBe(3);
        });

        it("should remove a province", function () {
            shippingMethod.addProvince(_.extend({}, pdfs, { key: "wut key?" }));
            shippingMethod.removeProvince(0);
            expect(shippingMethod.provinces.length).toBe(1);
            shippingMethod.removeProvince(0);
            expect(shippingMethod.provinces.length).toBe(0);
        });

        it("should return the dialogEditorView", function () {
            expect(shippingMethod.displayEditor()).toBe(shfs.dialogEditorView.editorView);
        });
    });

    describe("Range Model", function () {
        var range;
        var rfs;

        beforeEach(function () {
            rfs = {
                low: -123,
                high: 10
            }
            range = new merchello.Models.Range(rfs.low, rfs.high);
        });

        it("should create a populated range.", function () {
            expect(range).toBeDefined();
            expect(range.low).toBe(rfs.low);
            expect(range.high).toBe(rfs.high);
        });

        it("should create an empty range", function() {
            range = new merchello.Models.Range();
            expect(range).toBeDefined();
            expect(range.low).toBe(0);
            expect(range.high).toBe(0);
        });
    });

    describe("Fixed Rate Shipping Method Model", function () {
        var fixedRateShippingMethod;
        var frsmfs;

        beforeEach(function () {
            var shfs = {
                key: "a334",
                name: "ship it!",
                shipCountryKey: "DE",
                providerKey: "2341",
                shipMethodTfKey: "asd3asd",
                surcharge: "334wedasd",
                serviceCode: "234eqwse",
                taxable: true,
                provinces: [],
                dialogEditorView: { title: "22a", description: "dialogs stuff", editorView: "Do somethings" },
            }

            frsmfs = {
                shipMethod: shfs,
                gatewayResource: { name: "gate", serviceCode: "bAm" },
                rateTable: { shipMethodKey: "ASDASD", shipCountryKey: "asdaddfsa", rows: [{ rangeLow: 2, rate: 1.2, rangeHigh: 102 }, { rangeLow: 25, rate:3.4, rangeHigh: 52 }] },
                rateTableType: "data! data data data..."
            }
            fixedRateShippingMethod = new merchello.Models.FixedRateShippingMethod(frsmfs);
        });

        it("should create a populated Fixed Rate Shipping Method.", function () {
            expect(fixedRateShippingMethod).toBeDefined();
            for (var i in frsmfs) {
                if (typeof (frsmfs[i]) != "object")
                    expect(fixedRateShippingMethod[i]).toBe(frsmfs[i]);
            }
            expect(fixedRateShippingMethod.gatewayResource.name).toBe(frsmfs.gatewayResource.name);
            expect(fixedRateShippingMethod.rateTable.shipMethodKey).toBe(frsmfs.rateTable.shipMethodKey);
            expect(fixedRateShippingMethod.shipMethod.name).toBe(frsmfs.shipMethod.name);
        });

        it("should create an empty Fixed Rate Shipping Method", function () {
            fixedRateShippingMethod = new merchello.Models.FixedRateShippingMethod();
            expect(fixedRateShippingMethod).toBeDefined();

            for (var i in frsmfs) {
                if (typeof (frsmfs[i]) != "object")
                    expect(fixedRateShippingMethod[i]).toBe("");
            }
            expect(fixedRateShippingMethod.gatewayResource.name).toBe("");
            expect(fixedRateShippingMethod.rateTable.shipMethodKey).toBe("");
            expect(fixedRateShippingMethod.shipMethod.name).toBe("");
        });

        it("should calculate the range of all rateTiers", function () {
            expect(fixedRateShippingMethod.tierRange().low).toBe(2);
            expect(fixedRateShippingMethod.tierRange().high).toBe(102);
        });

        it("should calculate the range of all rateTier prices", function () {
            expect(fixedRateShippingMethod.tierPriceRange().low).toBe(1.2);
            expect(fixedRateShippingMethod.tierPriceRange().high).toBe(3.4);
        });
    });

    describe("Ship Rate Table Model", function () {
        var shipRateTable;
        var srtfs;
        var srtierfs;

        beforeEach(function () {
            srtierfs = {
                key: "23654",
                shipMethodKey: "GCA",
                rangeLow: 2,
                rate: 1.2,
                rangeHigh: 102,
                $$hashKey: 123123
            }
            var srtierfsv2 = { 
                key: "236as",
                rangeLow: 25,
                rate: 3.4,
                shipMethodKey: "CAL",
                rangeHigh: 52,
                $$hashKey: 1233
            }

            srtfs = {
                shipMethodKey: "ASDASD",
                shipCountryKey: "asdaddfsa",
                rows: [srtierfs, srtierfsv2]
            }
            shipRateTable = new merchello.Models.ShipRateTable(srtfs);
        });

        it("should make a new, populated, shipRateTable", function () {
            expect(shipRateTable).toBeDefined();
            for (var i in srtfs) {
                if (typeof (srtfs[i]) != "object")
                    expect(shipRateTable[i]).toBe(srtfs[i]);
            }
            expect(shipRateTable.rows.length).toBe(2);
            expect(shipRateTable.rows[0].rate).toBe(1.2);
        });

        it("should make a new, empty, shipRateTable", function () {
            shipRateTable = new merchello.Models.ShipRateTable();
            expect(shipRateTable).toBeDefined();
            for (var i in srtfs) {
                if (typeof (srtfs[i]) != "object")
                    expect(shipRateTable[i]).toBe("");
            }
            expect(shipRateTable.rows.length).toBe(0);;
        });

        it("should add a new row", function () {
            shipRateTable.addRow(new merchello.Models.ShippingRateTier(_.extend({}, srtierfs, { rangeLow: 3, rate: 1, rangeHigh: 24 })));
            expect(shipRateTable.rows.length).toBe(3);
        });

        //The function this is testing could be wrong --Micah
        it("should remove a row", function () {
            shipRateTable.rows[0].$$hashKey = srtierfs.$$hashKey;
            shipRateTable.removeRow(srtierfs);
            expect(shipRateTable.rows.length).toBe(1);
       
        });
    });

    describe("Shipping Rate Tier Model", function () {
        var shippingRateTier;
        var srtierfs;

        beforeEach(function () {
            srtierfs = {
                key: "23654",
                shipMethodKey: "GCA",
                rangeLow: 2,
                rate: 1.2,
                rangeHigh: 102
            }

            shippingRateTier = new merchello.Models.ShippingRateTier(srtierfs);
        });

        it("should create an empty Shipping Rate Tier Object", function () {
            shippingRateTier = new merchello.Models.ShippingRateTier();
            expect(shippingRateTier).toBeDefined();
            for (var i in srtierfs) {
                expect(shippingRateTier[i]).toBeFalsy();
            }
        });

        it("should create an empty Shipping Rate Tier Object", function () {
            expect(shippingRateTier).toBeDefined();
            for (var i in srtierfs) {
                expect(shippingRateTier[i]).toBe(srtierfs[i]);
            }
        });
    });

    describe("Warehouse Model", function () {
        var warehouse;
        var wfs;

        beforeEach(function () {
            wfs = {
                key: "23654",
                name: "Albert",
                address1: "Knowhere",
                address2: "Nova Core",
                locality: "GCA",
                region: "west",
                postalCode: "91000",
                countryCode: "CA",
                phone: "86775309",
                email: "email@example.com",
                isDefault: false,
                warehouseCatalogs: [{}, {}]
            }

            warehouse = new merchello.Models.Warehouse(wfs);
        });

        it("should create an empty Warehouse Object", function () {
            warehouse = new merchello.Models.Warehouse();
            expect(warehouse).toBeDefined();
            for (var i in wfs) {
                if (typeof(wfs[i]) != "object" && i != "isDefault")
                expect(warehouse[i]).toBeFalsy();
            }
            expect(warehouse.warehouseCatalogs.length).toBe(0);
            expect(warehouse.isDefault).toBeTruthy();
        });

        it("should create a populated Warehouse Object", function () {
            expect(warehouse).toBeDefined();
            for (var i in wfs) {
                if(typeof(wfs[i]) != "object")
                    expect(warehouse[i]).toBe(wfs[i]);
            }
            expect(warehouse.warehouseCatalogs.length).toBe(2);
        });
    });

    describe("Warehouse Catalog Model", function () {
        var warehouseCatalog;
        var wcfs;

        beforeEach(function () {
            wcfs = {
                description: "Dark with a subtle hint of rain",
                isDefault: true,
                name: "wilber",
                key: "O--nn",
                warehouseKey: "123"
            }

            warehouseCatalog = new merchello.Models.WarehouseCatalog(wcfs);
        });

        it("should create an empty Warehouse Catalog Object", function () {
            warehouseCatalog = new merchello.Models.WarehouseCatalog();
            expect(warehouseCatalog).toBeDefined();
            for (var i in wcfs) {
                if (typeof (wcfs[i]) != "object")
                    expect(warehouseCatalog[i]).toBeFalsy();
            }
        });

        it("should create a populated Warehouse Catalog Object", function () {
            expect(warehouseCatalog).toBeDefined();
            for (var i in wcfs) {
                if (typeof (wcfs[i]) != "object")
                    expect(warehouseCatalog[i]).toBe(wcfs[i]);
            }
        });
    });

    describe("Catalog Inventory Model", function () {
        var catalogInventory;
        var cifs;

        beforeEach(function () {
            cifs = {
                catalogName: "Dark with a subtle hint of rain",
                catalogKey: "KEY",
                count: 1000000000000,
                location: "privet drive",
                lowCount: 122,
                productVariantKey: "Ehhh",
                warehouseKey: "as in?"
            }

            catalogInventory = new merchello.Models.CatalogInventory(cifs);
        });

        it("should create an empty Catalog Inventory Object", function () {
            catalogInventory = new merchello.Models.CatalogInventory();
            expect(catalogInventory).toBeDefined();
            for (var i in cifs) {
                if (typeof (cifs[i]) != "object")
                    expect(catalogInventory[i]).toBeFalsy();
            }
        });

        it("should create a populated Catalog Inventory Object", function () {
            expect(catalogInventory).toBeDefined();
            for (var i in cifs) {
                if (typeof (cifs[i]) != "object")
                    expect(catalogInventory[i]).toBe(cifs[i]);
            }
        });
    });
});