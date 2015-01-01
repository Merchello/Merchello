'use strict';

describe('invoiceDisplayBuilder', function() {

    beforeEach(module('umbraco'));

    it ('should be able to create a default invoice', inject(function(invoiceDisplayBuilder) {

        //// Arrange
        // nothing to do

        //// Act
        var invoice = invoiceDisplayBuilder.createDefault();

        //// Assert
        expect(invoice).toBeDefined();
        expect(invoice.invoiceStatus.key).toBeDefined();

    }));

    it ('should be able to create invoices from an API json result', inject(function(invoiceDisplayBuilder, invoiceMocks) {
        //// Arrange
        var jsonResult = invoiceMocks.invoicesArray();

        //// Act
        var invoices = invoiceDisplayBuilder.transform(jsonResult);

        //// Assert
        expect (jsonResult.length).toBe(invoices.length);
    }));

    it ('should be able to create an invoice from a single jsonResult', inject(function(invoiceDisplayBuilder, invoiceMocks) {
        //// Arrange
        var jsonResult = invoiceMocks.randomInvoice();

        //// Act
        var invoice = invoiceDisplayBuilder.transform(jsonResult);

        //// Assert
        expect (invoice).toBeDefined();

    }));

});
