'use strict';

/* jasmine specs for models go here */

describe("Payment Models", function () {

    describe("PaymentMethod", function () {
        var paymentMethod;
        var pmfs;

        beforeEach(function () {
            pmfs = {
                key: "122",
                name: "doom",
                providerKey: "123",
                description: "Money Money Money",
                paymentCode: "101010101",
                dialogEditorView: { title: "22a", description: "dialogs stuff", editorView: "Do somethings" }
            }
            paymentMethod = new merchello.Models.PaymentMethod(pmfs);
        });

        it("should create a non empty payment method object based on the data passed in", function () {
            expect(paymentMethod).toBeDefined();
            for (var i in pmfs) {
                if (typeof (pmfs[i]) != "object")
                    expect(paymentMethod[i]).toBe(pmfs[i]);
            }
            expect(paymentMethod["dialogEditorView"].title).toBe(pmfs["dialogEditorView"].title);
        });

        it("should create an empty payment method object", function () {
            paymentMethod = new merchello.Models.PaymentMethod();
            expect(paymentMethod).toBeDefined();
            for (var i in pmfs) {
                if (typeof (pmfs[i]) != "object")
                    expect(paymentMethod[i]).toBe("");
            }
            expect(paymentMethod["dialogEditorView"]).toBe("");
        });

        it("should display the editor", function () {
            expect(paymentMethod.displayEditor()).toBe(pmfs.dialogEditorView.editorView);
        });
    });
    describe("PaymentGatewayProvider Model", function () {
        var paymentGatewayProvider;
        var pgpfs;

        beforeEach(function () {
            pgpfs = {
                key: "122",
                name: "doom",
                providerTfKey: "123",
                description: "Money Money Money",
                extendedData: ["humdigigity", "gateway"],
                activated: true,
                dialogEditorView: { title: "22a", description: "dialogs stuff", editorView: "Do somethings" }
            }
            paymentGatewayProvider = new merchello.Models.PaymentGatewayProvider(pgpfs);
        });

        it("should create a non empty payment gateway provider based on the data passed in", function () {
            expect(paymentGatewayProvider).toBeDefined();
            for (var i in pgpfs) {
                if (typeof (pgpfs[i]) != "object")
                    expect(paymentGatewayProvider[i]).toBe(pgpfs[i]);
            }
            expect(paymentGatewayProvider["dialogEditorView"].title).toBe(pgpfs["dialogEditorView"].title);
        });

        it("should create an empty payment gateway provider", function () {
            paymentGatewayProvider = new merchello.Models.PaymentGatewayProvider();
            expect(paymentGatewayProvider).toBeDefined();
            for (var i in pgpfs) {
                if (typeof (pgpfs[i]) == "string")
                    expect(paymentGatewayProvider[i]).toBe("");
                if (typeof (pgpfs[i]) == "boolean")
                    expect(paymentGatewayProvider[i]).toBe(false);
                if (typeof (pgpfs[i]) == "number")
                    expect(paymentGatewayProvider[i]).toBe(0);
            }
            expect(paymentGatewayProvider["dialogEditorView"]).toBe("");
        });

        it("should display the editor", function () {
            expect(paymentGatewayProvider.displayEditor()).toBe(pgpfs.dialogEditorView.editorView);
        });
    });
});