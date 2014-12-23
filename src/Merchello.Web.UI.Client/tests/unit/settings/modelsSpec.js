'use strict';

/* jasmine specs for models go here */
describe("Settings Models", function () { 
    describe("Province Model", function () {
        var province;
        var pfs;
    
        beforeEach(function () {
            pfs = {
                name: "asdasd",
                code: "sjkbvhj"
            }
    
            province = new merchello.Models.Province(pfs);
        });
    
        it("should create an empty Province Object", function () {
            province = new merchello.Models.Province();
            expect(province).toBeDefined();
            for (var i in pfs) {
                if (typeof (pfs[i]) != "object")
                    expect(province[i]).toBeFalsy();
            }
        });
    
        it("should create a populated Province Object", function () {
            expect(province).toBeDefined();
            for (var i in pfs) {
                if (typeof (pfs[i]) != "object")
                    expect(province[i]).toBe(pfs[i]);
            }
        });
    });

    describe("Address Model", function () {
        var address;
        var afs;

        beforeEach(function () {
            afs = {
                name: "James Miller",
                address1: "394 Salsbury Ln",
                address2: "777 NW Halmested Ln",
                locality: "EN-CA",
                region: "North",
                postalCode: "10293",
                countryCode: "CA",
                email: "james@example.com",
                phone: "2939848738",
                organization: "Pick'n Seed",
                isCommercial: true
            }

            address = new merchello.Models.Address(afs);
        });

        it("should create an empty Address Object", function () {
            address = new merchello.Models.Address();
            expect(address).toBeDefined();
            for (var i in afs) {
                if (typeof (afs[i]) != "object")
                    expect(address[i]).toBeFalsy();
            }
        });

        it("should create a populated Address Object", function () {
            expect(address).toBeDefined();
            for (var i in afs) {
                if (typeof (afs[i]) != "object")
                    expect(address[i]).toBe(afs[i]);
            }
        });
    });

    describe("Country Model", function () {
        var country;
        var cfs;

        beforeEach(function () {
            cfs = {
                name: "James Miller",
                countryCode: "CA",
                provinces: [{}, {}, {}],
                provinceLabel: "The West Most Isle"
            }

            country = new merchello.Models.Country(cfs);
        });

        it("should create an empty Country Object", function () {
            country = new merchello.Models.Country();
            expect(country).toBeDefined();
            for (var i in cfs) {
                if (typeof (cfs[i]) != "object")
                    expect(country[i]).toBeFalsy();
            }
            expect(country["provinces"].length).toBeFalsy();
        });

        it("should create a populated Country Object", function () {
            expect(country).toBeDefined();
            for (var i in cfs) {
                if (typeof (cfs[i]) != "object")
                    expect(country[i]).toBe(cfs[i]);
            }
            expect(country["provinces"].length).toBe(3);
        });
    });

    describe("Currency Model", function () {
        var currency;
        var cfs;

        beforeEach(function () {
            cfs = {
                name: "Stellar",
                currencyCode: "SLR",
                symbol: "S"
            }

            currency = new merchello.Models.Currency(cfs);
        });

        it("should create an empty Currency Object", function () {
            currency = new merchello.Models.Currency();
            expect(currency).toBeDefined();
            for (var i in cfs) {
                if (typeof (cfs[i]) != "object")
                    expect(currency[i]).toBeFalsy();
            }
        });

        it("should create a populated Currency Object", function () {
            expect(currency).toBeDefined();
            for (var i in cfs) {
                if (typeof (cfs[i]) != "object")
                    expect(currency[i]).toBe(cfs[i]);
            }
        });
    });

    describe("List Query Model", function () {
        var listQuery;
        var lqfs;

        beforeEach(function () {
            lqfs = {
                currentPage: 3,
                itemsPerPage: 23,
                parameters: [{}, {}, {}, {}],
                sortBy: "billtoname",
                sortDirection: "Descending"
            }

            listQuery = new merchello.Models.ListQuery(lqfs);
        });

        it("should create an empty List Query Object", function () {
            listQuery = new merchello.Models.ListQuery();
            expect(listQuery).toBeDefined();
            for (var i in lqfs) {
                if (typeof (lqfs[i]) != "object" && i != "sortBy" && i != "sortDirection")
                    expect(listQuery[i]).toBeFalsy();
            }
            expect(listQuery["sortBy"]).toBe("invoicenumber");
            expect(listQuery["sortDirection"]).toBe("Ascending");
            expect(listQuery.parameters.length).toBe(0);
        });

        it("should create a populated List Query Object", function () {
            expect(listQuery).toBeDefined();
            for (var i in lqfs) {
                if (typeof (lqfs[i]) != "object")
                    expect(listQuery[i]).toBe(lqfs[i]);
            }
            expect(listQuery.parameters.length).toBe(4);
        });
    });

    describe("List Query Parameter Model", function () {
        var listQueryParameter;
        var lqpfs;

        beforeEach(function () {
            lqpfs = {
                fieldName: "moon",
                value: "soon"
            }

            listQueryParameter = new merchello.Models.ListQueryParameter(lqpfs);
        });

        it("should create an empty List Query Parameter Object", function () {
            listQueryParameter = new merchello.Models.ListQueryParameter();
            expect(listQueryParameter).toBeDefined();
            for (var i in lqpfs) {
                if (typeof (lqpfs[i]) != "object")
                    expect(listQueryParameter[i]).toBeFalsy();
            }
        });

        it("should create a populated List Query Parameter Object", function () {
            expect(listQueryParameter).toBeDefined();
            for (var i in lqpfs) {
                if (typeof (lqpfs[i]) != "object")
                    expect(listQueryParameter[i]).toBe(lqpfs[i]);
            }
        });
    });

    describe("Query Result Model", function () {
        var queryResult;
        var qrfs;

        beforeEach(function () {
            qrfs = {
                currentPage: 3,
                itemsPerPage: 23,
                totalItems: 193,
                items: [{}],
                totalPages: 24
            }

            queryResult = new merchello.Models.QueryResult(qrfs);
        });

        it("should create an empty Query Result Object", function () {
            queryResult = new merchello.Models.QueryResult();
            expect(queryResult).toBeDefined();
            for (var i in qrfs) {
                if (typeof (qrfs[i]) != "object")
                    expect(queryResult[i]).toBeFalsy();
            }
            expect(queryResult["items"].length).toBe(0);
        });

        it("should create a populated Query Result Object", function () {
            expect(queryResult).toBeDefined();
            for (var i in qrfs) {
                if (typeof (qrfs[i]) != "object")
                    expect(queryResult[i]).toBe(qrfs[i]);
            }
            expect(queryResult["items"].length).toBe(1);
        });
    });

    describe("Type Field Model", function () {
        var typeField;
        var tffs;

        beforeEach(function () {
            tffs = {
                name: "Mars",
                alias: "Gone",
                typeKey: "S"
            }

            typeField = new merchello.Models.TypeField(tffs);
        });

        it("should create an empty Type Field Object", function () {
            typeField = new merchello.Models.TypeField();
            expect(typeField).toBeDefined();
            for (var i in tffs) {
                if (typeof (tffs[i]) != "object")
                    expect(typeField[i]).toBeFalsy();
            }
        });

        it("should create a populated Type Field Object", function () {
            expect(typeField).toBeDefined();
            for (var i in tffs) {
                if (typeof (tffs[i]) != "object")
                    expect(typeField[i]).toBe(tffs[i]);
            }
        });
    });

    describe("Store Settings Model", function () {
        var storeSettings;
        var ssfs;

        beforeEach(function () {
            ssfs = {
                nextOrderNumber: 123123,
                currencyCode: "SLR",
                nextInvoiceNumber: 445332345,
                dateFormat: "Stellar",
                timeFormat: "SLR",
                unitSystem: "S",
                globalShippable: true,
                globalTaxable: true,
                globalTrackInventory: true,
                globalShippingIsTaxable: true
            }

            storeSettings = new merchello.Models.StoreSettings(ssfs);
        });

        it("should create an empty Store Settings Object", function () {
            storeSettings = new merchello.Models.StoreSettings();
            expect(storeSettings).toBeDefined();
            for (var i in ssfs) {
                if (typeof (ssfs[i]) != "object")
                    expect(storeSettings[i]).toBeFalsy();
            }
        });

        it("should create a populated Store Settings Object", function () {
            expect(storeSettings).toBeDefined();
            for (var i in ssfs) {
                if (typeof (ssfs[i]) != "object")
                    expect(storeSettings[i]).toBe(ssfs[i]);
            }
        });
    });

    describe("Gateway Resource Model", function () {
        var gatewayResource;
        var grfs;

        beforeEach(function () {
            grfs = {
                name: "Marshall",
                serviceCode: "Taktak"
            }

            gatewayResource = new merchello.Models.GatewayResource(grfs);
        });

        it("should create an empty Gateway Resource Object", function () {
            gatewayResource = new merchello.Models.GatewayResource();
            expect(gatewayResource).toBeDefined();
            for (var i in grfs) {
                if (typeof (grfs[i]) != "object")
                    expect(gatewayResource[i]).toBeFalsy();
            }
        });

        it("should create a populated Gateway Resource Object", function () {
            expect(gatewayResource).toBeDefined();
            for (var i in grfs) {
                if (typeof (grfs[i]) != "object")
                    expect(gatewayResource[i]).toBe(grfs[i]);
            }
        });
    });

    describe("Gateway Provider Model", function () {
        var gatewayProvider;
        var gpfs;

        beforeEach(function () {
            gpfs = {
                key: "Marshall",
                name: "Taktak",
                providerTfKey: "123",
                description: "Things happen",
                extendedData: [{}, {}],
                encryptExtendedData: true,
                activated: true,
                dialogEditorView: {editorView: "It's a thing"}
            }

            gatewayProvider = new merchello.Models.GatewayProvider(gpfs);
        });

        it("should create an empty Gateway Provider Object", function () {
            gatewayProvider = new merchello.Models.GatewayProvider();
            expect(gatewayProvider).toBeDefined();
            for (var i in gpfs) {
                if (typeof (gpfs[i]) != "object")
                    expect(gatewayProvider[i]).toBeFalsy();
            }
            expect(gatewayProvider["extendedData"].length).toBe(0);
        });

        it("should create a populated Gateway Provider Object", function () {
            expect(gatewayProvider).toBeDefined();
            for (var i in gpfs) {
                if (typeof (gpfs[i]) != "object")
                    expect(gatewayProvider[i]).toBe(gpfs[i]);
            }
            expect(gatewayProvider["extendedData"].length).toBe(2);
            expect(gatewayProvider["extendedData"][0]).toBeDefined();
        });

        it("should the dialogeEditorView if activated and false if its not", function () {
            expect(gatewayProvider.displayEditor()).toBe(gpfs.dialogEditorView.editorView);
            gatewayProvider.activated = false;
            expect(gatewayProvider.displayEditor()).toBe(false);
        });

        it("should true if key matches and false if doesnt not", function () {
            expect(gatewayProvider.isFixedRate()).toBe(false);
            gatewayProvider.key = "aec7a923-9f64-41d0-b17b-0ef64725f576";
            expect(gatewayProvider.isFixedRate()).toBe(true);
        });
    });

    describe("Dialog Editor View Model", function () {
        var dialogEditorView;
        var devfs;

        beforeEach(function () {
            devfs = {
                title: "Prince Edward the II",
                description: "At least he has a scepter...",
                editorView: "No comment."
            }

            dialogEditorView = new merchello.Models.DialogEditorView(devfs);
        });

        it("should create an empty Currency Object", function () {
            dialogEditorView = new merchello.Models.DialogEditorView();
            expect(dialogEditorView).toBeDefined();
            for (var i in devfs) {
                if (typeof (devfs[i]) != "object")
                    expect(dialogEditorView[i]).toBeFalsy();
            }
        });

        it("should create a populated Currency Object", function () {
            expect(dialogEditorView).toBeDefined();
            for (var i in devfs) {
                if (typeof (devfs[i]) != "object")
                    expect(dialogEditorView[i]).toBe(devfs[i]);
            }
        });
    });
});