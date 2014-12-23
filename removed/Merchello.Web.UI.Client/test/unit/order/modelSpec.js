'use strict';

/* jasmine specs for models go here */

describe("Merchello order models", function () {
    describe("OrderLineItem Model", function () {
        var olifs;
        var orderLineItem;

        beforeEach(function () {
            var pafs = { key: "123", optionKey: "321", name: "bane", sortOrder: 6, optionOrder: 7, isRemoved: true };
            var pvfs = {
                key: "123",
                name: "bane",
                productKey: "asdfg",
                sku: "1qa2ws",
                price: 1.00,
                costOfGoods: 2.00,
                salePrice: 3.00,
                onSale: true,
                manufacturer: "Me",
                manufacturerModelNumber: "1234567890",
                weight: 10,
                length: 11,
                width: 12,
                height: 13,
                barcode: "10101010",
                available: false,
                trackInventory: true,
                outOfStockPurchase: true,
                taxable: true,
                shippable: true,
                download: true,
                downloadMediaId: 123890,
                totalInventoryCount: 1000000000,
                attributes: [pafs, pafs],
                attributeKeys: ["123"],
                catalogInventories: []
            };

            olifs = {
                key: "123",
                containerKey: "321",
                shipmentKey: "free",
                lineItemTfKey: "me",
                sku: "XXX-333-XXXX",
                name: "Whisper Kitten",
                quantity: 1,
                price: 1231333.98,
                exported: true,
                backOrder: true,
                extendedData: [{ key: "merchProductVariantKey", value: "fun" }, { key: "otherkey", value: "dum" }],
                productVariant: pvfs //It's always going to be an empty object. Problem?
            }
            orderLineItem = new merchello.Models.OrderLineItem(olifs);
        });

        it("Should create a populated OrderLineItem", function () {
            expect(orderLineItem).toBeDefined();
            for (var i in olifs) {
                if (i != "productVariant") {
                    expect(orderLineItem[i]).toBe(olifs[i]);
                }
            }

            expect(orderLineItem["productVariant"]).toEqual({});
        });

        it("Should create an empty OrderLineItem", function () {
            orderLineItem = new merchello.Models.OrderLineItem();
            expect(orderLineItem).toBeDefined();
            for (var i in olifs) {
                if (typeof (olifs[i]) == "boolean")
                    expect(orderLineItem[i]).toBe(false);
                if (typeof (olifs[i]) == "number")
                    expect(orderLineItem[i]).toBe(0);
                if (typeof (olifs[i]) == "object")
                    expect(orderLineItem[i]).toEqual({});
                if (typeof (olifs[i]) == "string")
                    expect(orderLineItem[i]).toBe("");
            }
        });

        it("Should return the product variant key", function () {
            expect(orderLineItem).toBeDefined();
            expect(orderLineItem.getProductVariantKey()).toEqual(olifs.extendedData[0]);
            orderLineItem = new merchello.Models.OrderLineItem();
            expect(orderLineItem.getProductVariantKey()).toBe("");
        });

        it("should set the product variant", function() {
            expect(orderLineItem).toBeDefined();
            expect(orderLineItem.productVariant).toEqual({});
            orderLineItem.setProductVariant({ me: "nah" });
            expect(orderLineItem.productVariant).toEqual({ me: "nah" });
        });
    });

    describe("Order Status Model", function () {
        var orderStatus;
        var osfs;

        beforeEach(function () {
            osfs = {
                key: "123",
                name: "mon",
                alias: "tues",
                reportable: "sometimes",
                active: "now",
                sortOrder: "123"
            }
            orderStatus = new merchello.Models.OrderStatus(osfs);
        });

        it("should create a new populated order status", function () {
            expect(orderStatus).toBeDefined();
            for (var i in osfs) {
                expect(orderStatus[i]).toBe(osfs[i]);
            }
        });

        it("should create an empty order status", function () {
            orderStatus = new merchello.Models.OrderStatus();
            expect(orderStatus).toBeDefined();
            for (var i in osfs) {
                expect(orderStatus[i]).toBe("");
            }
        });
    });

    describe("Order Model", function () {
        var order;
        var ofs;

        beforeEach(function () {
            var pafs = { key: "123", optionKey: "321", name: "bane", sortOrder: 6, optionOrder: 7, isRemoved: true };
            var pvfs = {
                key: "123",
                name: "bane",
                productKey: "asdfg",
                sku: "1qa2ws",
                price: 1.00,
                costOfGoods: 2.00,
                salePrice: 3.00,
                onSale: true,
                manufacturer: "Me",
                manufacturerModelNumber: "1234567890",
                weight: 10,
                length: 11,
                width: 12,
                height: 13,
                barcode: "10101010",
                available: false,
                trackInventory: true,
                outOfStockPurchase: true,
                taxable: true,
                shippable: true,
                download: true,
                downloadMediaId: 123890,
                totalInventoryCount: 1000000000,
                attributes: [pafs, pafs],
                attributeKeys: ["123"],
                catalogInventories: []
            };

            var olifs = {
                key: "123",
                containerKey: "321",
                shipmentKey: "free",
                lineItemTfKey: "me",
                sku: "XXX-333-XXXX",
                name: "Whisper Kitten",
                quantity: 1,
                price: 1231333.98,
                exported: true,
                backOrder: true,
                extendedData: [{ key: "merchProductVariantKey", value: "fun" }, { key: "otherkey", value: "dum" }],
                productVariant: pvfs //It's always going to be an empty object. Problem?
            }
            var olifsv2 = _.extend({}, olifs, { key: "444", containerKey: "321", sku: "XXX-444-XXXX" });

            var osfs = {
                key: "123",
                name: "mon",
                alias: "tues",
                reportable: "sometimes",
                active: "now",
                sortOrder: "123"
            }

            ofs = {
                key: "123",
                versionKey: "12.3",
                invoiceKey: "3333",
                orderNumberPrefix: "X",
                orderNumber: "29382",
                orderDate: "11/11/11",
                orderStatusKey: "fufilled",
                orderStatus: osfs,
                exported: "bet your best baseball bat it is",
                items: [olifs, olifsv2]
            }
            order = new merchello.Models.Order(ofs);
        });

        it("should create a new populated order", function () {
            expect(order).toBeDefined();
            for (var i in ofs) {
                if (i != "items" && i != "orderStatus")
                    expect(order[i]).toBe(ofs[i]);
            }
            expect(order["items"][0].key).toMatch(ofs["items"][0].key);
            expect(order["orderStatus"].name).toMatch(ofs["items"].name);
        });

        it("should create an empty order", function () {
            order = new merchello.Models.Order();
            expect(order).toBeDefined();
            for (var i in ofs) {
                if (i != "items" && i != "orderStatus")
                    expect(order[i]).toBe("");
            }
            expect(order["items"].length).toBe(0);
            expect(order["orderStatus"].name).toBe("");
        });
    });

    describe("Invoice Line Item Model", function () {
        var invoiceLineItem;
        var ilifs;

        beforeEach(function () {
            ilifs = {
                key: "123",
                containerKey: "321",
                lineItemTfKey: "me",
                sku: "XXX-333-XXXX",
                name: "Whisper Kitten",
                quantity: "1",
                price: "1231333.98",
                exported: "true"
            }
            invoiceLineItem = new merchello.Models.InvoiceLineItem(ilifs);
        });

        it("should create an populated Invoice Line item", function () {
            expect(invoiceLineItem).toBeDefined();
            for (var i in ilifs) {
                expect(invoiceLineItem[i]).toBe(ilifs[i]);
            }
        });

        it("should create an empty Invoice Line item", function () {
            invoiceLineItem = new merchello.Models.InvoiceLineItem();
            expect(invoiceLineItem).toBeDefined();
            for (var i in ilifs) {
                expect(invoiceLineItem[i]).toBe("");
            }
        });
    });

    describe("Invoice Status Model", function () {
        var invoiceStatus;
        var isfs;

        beforeEach(function () {
            isfs = {
                key: "123",
                name: "martin",
                alias: "doc",
                reportable: "for realz",
                active: "yea",
                sortOrder: "3"
            }
            invoiceStatus = new merchello.Models.InvoiceStatus(isfs);
        });

        it("shout return a product", function() {
            expect(invoiceStatus).toBeDefined();
            for (var i in isfs) {
                expect(invoiceStatus[i]).toBe(isfs[i]);
            }
        });

        it("shout return an empty product", function () {
            invoiceStatus = new merchello.Models.InvoiceStatus();
            expect(invoiceStatus).toBeDefined();
            for (var i in isfs) {
                expect(invoiceStatus[i]).toBe("");
            }
        });
    });

    describe("Invoice Model", function () {
        var invoice;
        var ifs;

        beforeEach(function () {
            var ilifs = {
                key: "123",
                containerKey: "321",
                lineItemTfKey: "me",
                sku: "XXX-333-XXXX",
                name: "Whisper Kitten",
                quantity: "1",
                price: "1231333.98",
                exported: "true"
            }
            
            var isfs = {
                key: "123",
                name: "martin",
                alias: "doc",
                reportable: "for realz",
                active: "yea",
                sortOrder: "3"
            }

            var osfs = {
                key: "123",
                name: "mon",
                alias: "tues",
                reportable: "sometimes",
                active: "now",
                sortOrder: "123"
            }

            var ofs = {
                key: "123",
                versionKey: "12.3",
                invoiceKey: "3333",
                orderNumberPrefix: "X",
                orderNumber: "29382",
                orderDate: "11/11/11",
                orderStatusKey: "fufilled",
                orderStatus: osfs,
                exported: "bet your best baseball bat it is",
                items: []
            }
            
            ifs = {
                key: "123",
                versionKey: "321",
                customerKey: "333",
                invoiceNumberPrefix: "A",
                invoiceNumber: "432",
                invoiceDate: "11/11/11",
                invoiceStatusKey: "broken",
                invoiceStatus: isfs,
                billToName: "Brandon",
                billToAddress1: "32 Wallaby Way, Sidney",
                billToAddress2: "777 2nd St, Corvallis",
                billToRegion: "BC",
                billToLocality: "EN-CA",
                billToPostalCode: "9100",
                billToCountryCode: "CA",
                billToEmail: "brandon@example.com",
                billToPhone: "541-867-5309",
                billToCompany: "Proworks",
                exported: "yes",
                archived: "no",
                total: 332,
                items: [ilifs],
                orders: [ofs]
            }

            invoice = new merchello.Models.Invoice(ifs);
        });

        it("should create an invoice", function () {
            expect(invoice).toBeDefined();
            for (var i in ifs) {
                if(typeof(ifs[i]) != "object")
                    expect(invoice[i]).toBe(ifs[i]);
                if(ifs[i] instanceof Array)
                    expect(invoice[i].length).toBe(ifs[i].length);
            }
        });

        it("should create an empty invoice", function () {
            invoice = new merchello.Models.Invoice();
            expect(invoice).toBeDefined();
            for (var i in ifs) {
                if (typeof (ifs[i]) == "string")
                    expect(invoice[i]).toBe("");
                if (typeof (ifs[i]) == "boolean")
                    expect(invoice[i]).toBe(false);
                if (typeof (ifs[i]) == "number")
                    expect(invoice[i]).toBe(0);
                if (ifs[i] instanceof Array)
                    expect(invoice[i].length).toBe(0);
            }
        });

        it("should return the payment status", function () {
            expect(invoice.getPaymentStatus()).toBe(ifs.invoiceStatus.name);
        });

        it("should return the fulfillment status", function () {
            expect(invoice.getFulfillmentStatus()).toBe(ifs.orders[0].orderStatus.name);
            invoice.orders = [];
            expect(invoice.getFulfillmentStatus()).toBe("");
        });

        it("should return the product line items", function () {
            invoice.items[0].lineItemType = new merchello.Models.TypeField({ alias: "Product", name: "fame", typeKey: "old" });

            expect(invoice.getProductLineItems().length).toBe(1);
            expect(invoice.getProductLineItems()[0].name).toBe("Whisper Kitten");
        });

        it("should return the tax line items", function () {
            invoice.items[0].lineItemType = new merchello.Models.TypeField({ alias: "Tax", name: "fame", typeKey: "old" });

            expect(invoice.getTaxLineItem().name).toBe("Whisper Kitten");
        });

        it("should return the shipping line items", function () {
            invoice.items[0].lineItemType = new merchello.Models.TypeField({ alias: "Shipping", name: "fame", typeKey: "old" });

            expect(invoice.getShippingLineItem().name).toBe("Whisper Kitten");
        });
    });

    describe("Payment Model", function () {
        var payment;
        var pfs;
        var apfs;

        beforeEach(function () {
            apfs = {
                key: "2234",
                paymentKey: "123",
                invoiceKey: "223",
                appliedPaymentTfKey: "123",
                description: "A small payment in fiat currency",
                createDate: "01/01/01",
                amount: 1,
                exported: true,
                invoice: {},
                payment: {}
            }

            pfs = {
                key: "123",
                customerKey: "312",
                paymentMethodKey: "222",
                paymentTypeFieldKey: "224422",
                paymentMethodName: "bitcoin",
                referenceNumber: "39493",
                amount: 2.25,
                authorized: true,
                collected: true,
                exported: true,
                appliedPayments: [apfs]
            }
            payment = new merchello.Models.Payment(pfs);
        });

        it("should create a populated payment", function () {
            expect(payment).toBeDefined();
            for (var i in pfs) {
                if (typeof (pfs[i]) != "object")
                    expect(payment[i]).toBe(pfs[i]);
            }
            expect(payment["appliedPayments"].length).toBe(1);
            expect(payment["appliedPayments"][0].key).toBe(apfs.key);
        });

        it("should create an empty payment", function () {
            payment = new merchello.Models.Payment();
            expect(payment).toBeDefined();
            for (var i in pfs) {
                if (typeof (pfs[i]) == "string")
                    expect(payment[i]).toBe("");
                if (typeof (pfs[i]) == "boolean")
                    expect(payment[i]).toBe(false);
                if (typeof (pfs[i]) == "number")
                    expect(payment[i]).toBe(0);
            }
            expect(payment["appliedPayments"].length).toBe(0);
        });

        it("should return the status with aspects seperated by slashes", function () {
            expect(payment.getStatus()).toBe("Authorized/Captured/Exported");
            payment = _.extend(payment, { authorized: false, collected: true, exported: true });
            expect(payment.getStatus()).toBe("Captured/Exported");
            payment = _.extend(payment, { authorized: true, collected: true, exported: false });
            expect(payment.getStatus()).toBe("Authorized/Captured");
            payment = _.extend(payment, { authorized: true, collected: false, exported: false });
            expect(payment.getStatus()).toBe("Authorized");
            payment = _.extend(payment, { authorized: false, collected: false, exported: false });
            expect(payment.getStatus()).toBe("");
        });

        it("should reutrn true if the amount is greater than zero", function () {
            expect(payment.hasAmount()).toBeTruthy();
            payment.amount = 0;
            expect(payment.hasAmount()).toBeFalsy();
        });
    });

    describe("",function () {
        var appliedPayment;
        var apfs;

        beforeEach(function () {
            var pfs = {
                key: "123",
                customerKey: "312",
                paymentMethodKey: "222",
                paymentTypeFieldKey: "224422",
                paymentMethodName: "bitcoin",
                referenceNumber: "39493",
                amount: 2.25,
                authorized: true,
                collected: true,
                exported: true,
                appliedPayments: []
            }

            var ifs = {
                key: "123",
                versionKey: "321",
                customerKey: "333",
                invoiceNumberPrefix: "A",
                invoiceNumber: "432",
                invoiceDate: "11/11/11",
                invoiceStatusKey: "broken",
                invoiceStatus: {},
                billToName: "Brandon",
                billToAddress1: "32 Wallaby Way, Sidney",
                billToAddress2: "777 2nd St, Corvallis",
                billToRegion: "BC",
                billToLocality: "EN-CA",
                billToPostalCode: "9100",
                billToCountryCode: "CA",
                billToEmail: "brandon@example.com",
                billToPhone: "541-867-5309",
                billToCompany: "Proworks",
                exported: "yes",
                archived: "no",
                total: 332,
                items: [],
                orders: []
            }

            apfs = {
                key: "2234",
                paymentKey: "123",
                invoiceKey: "223",
                appliedPaymentTfKey: "123",
                description: "A small payment in fiat currency",
                createDate: "01/01/01",
                amount: 1,
                exported: true,
                invoice: ifs,
                payment: pfs
            }

            appliedPayment = new merchello.Models.AppliedPayment(apfs);
        });

        it("should create an applied payment", function () {
            expect(appliedPayment).toBeDefined();
            for (var i in apfs) {
                if (typeof(apfs[i]) != "object")
                    expect(appliedPayment[i]).toBe(apfs[i]);
            }
        });

        it("should create an empty applied payment", function () {
            appliedPayment = new merchello.Models.AppliedPayment();
            expect(appliedPayment).toBeDefined();
            for (var i in apfs) {
                if (typeof (apfs[i]) == "string")
                    expect(appliedPayment[i]).toBe("");
                if (typeof (apfs[i]) == "boolean")
                    expect(appliedPayment[i]).toBe(false);
                if (typeof (apfs[i]) == "number")
                    expect(appliedPayment[i]).toBe(0);
            }
        });

        it("should return true if the amount is greater than zero", function () {
            expect(appliedPayment.hasAmount()).toBeTruthy();
            appliedPayment.amount = 0;
            expect(appliedPayment.hasAmount()).toBeFalsy();
        });
    });

    describe("Payment Request Model", function () {
        var paymentRequest;
        var prfs;

        beforeEach(function () {
            prfs = {
                invoiceKey: "123",
                paymentKey: "321",
                paymentMethodKey: "222",
                amount: 101,
                processorArgs: ["ARRRGGG!"]
            }
            paymentRequest = new merchello.Models.PaymentRequest(prfs);
        });

        it("should create a populated payment request", function () {
            expect(paymentRequest).toBeDefined();
            for (var i in prfs) {
                if (typeof(prfs[i]) != "object")
                    expect(paymentRequest[i]).toBe(prfs[i]);
            }
            expect(paymentRequest["processorArgs"][0]).toBe(prfs["processorArgs"][0]);
        });

        it("should create an empty payment request", function () {
            paymentRequest = new merchello.Models.PaymentRequest();
            expect(paymentRequest).toBeDefined();
            for (var i in prfs) {
                if (typeof (prfs[i]) == "string")
                    expect(paymentRequest[i]).toBe("");
                if (typeof (prfs[i]) == "boolean")
                    expect(paymentRequest[i]).toBe(false);
                if (typeof (prfs[i]) == "number")
                    expect(paymentRequest[i]).toBe(0);
            }
            expect(paymentRequest["processorArgs"].length).toBe(0);
        });
    });

    describe("Shippment Model", function () {
        var shipment;
        var sfs;

        beforeEach(function () {

            var olifs = {
                key: "123",
                containerKey: "321",
                shipmentKey: "free",
                lineItemTfKey: "me",
                sku: "XXX-333-XXXX",
                name: "Whisper Kitten",
                quantity: 1,
                price: 1231333.98,
                exported: true,
                backOrder: true,
                extendedData: [{ key: "merchProductVariantKey", value: "fun" }, { key: "otherkey", value: "dum" }],
                productVariant: {}
            }
            var olifsv2 = _.extend({}, olifs, { key: "444", containerKey: "321", sku: "XXX-444-XXXX" });

            sfs = {
                key: "123",
                versionKey: "321",
                fromOrganization: "TMI",
                fromName: "Turf Merchants Inc.",
                fromAddress1: "32 Walaby Way, Sidney",
                fromAddress2: "777 2nd st, Corvallis",
                fromRegion: "BC",
                fromLocality: "EN-CA",
                fromPostalCode: "91000",
                fromCountryCode: "CA",
                fromIsCommercial: "true",
                toOrganization: "mindfly",
                toName: "Mindfly",
                toAddress1: "32 Walaby Way, Sidney",
                toAddress2: "777 2nd st, Corvallis",
                toRegion: "BC",
                toLocality: "EN-CA",
                toPostalCode: "91000",
                toCountryCode: "CA",
                toIsCommercial: "false",
                shipMethodKey: "23121",
                phone: "8675309",
                email: "notinyourlife@example.com",
                carrier: "actual pidgeons",
                trackingCode: "X#323ASR3$$#LKJFDKJ$",
                shippedDate: "11/11/11",
                items: [olifs,olifsv2]
            }
            shipment = new merchello.Models.Shipment(sfs);
        });

        it("should create a filled out shipment", function () {
            expect(shipment).toBeDefined();
            for (var i in sfs) {
                if (typeof (sfs[i]) != "object")
                    expect(shipment[i]).toBe(sfs[i]);
            }
            expect(shipment.items.length).toBe(2);
            expect(shipment.items[0].shipmentKey).toBe("free");
        });

        it("should create an empty shipment", function () {
            shipment = new merchello.Models.Shipment();
            expect(shipment).toBeDefined();
            for (var i in sfs) {
                if (typeof (sfs[i]) == "string")
                    expect(shipment[i]).toBe("");
                if (typeof (sfs[i]) == "boolean")
                    expect(shipment[i]).toBe(false);
                if (typeof (sfs[i]) == "number")
                    expect(shipment[i]).toBe(0);
            }
            expect(shipment.items.length).toBe(0);
        });
    });

    describe("Order Summary Model", function () {
        var orderSummary;
        var osfs;

        beforeEach(function () {
            osfs = {
                itemTotal: 3,
                invoiceTotal: 5,
                shippingTotal: 2,
                taxTotal: 1,
                orderPrepComplete: true
            }
            orderSummary = new merchello.Models.OrderSummary(osfs);
        });

        it("should create a populated order summary", function () {
            expect(orderSummary).toBeDefined();
            for (var i in osfs) {
                expect(orderSummary[i]).toBe(osfs[i]);
            }
        });
        it("should create a populated order summary", function () {
            orderSummary = new merchello.Models.OrderSummary();
            expect(orderSummary).toBeDefined();
            for (var i in osfs) {
                if (typeof (osfs[i]) == "string")
                    expect(orderSummary[i]).toBe("");
                if (typeof (osfs[i]) == "bool")
                    expect(orderSummary[i]).toBe(false);
                if (typeof (osfs[i]) == "number")
                    expect(orderSummary[i]).toBe(0);
            }
        });
    })
});