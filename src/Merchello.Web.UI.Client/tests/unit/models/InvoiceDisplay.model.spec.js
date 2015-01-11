'use strict';

describe('InvoiceDisplay prototype tests', function() {

    beforeEach(module('umbraco'));

    it('should be able to get a payment status', inject(function(invoiceDisplayBuilder, invoiceMocks) {
        //// Arrange
        var json = invoiceMocks.randomInvoice();
        var invoice = invoiceDisplayBuilder.transform(json);

        //// Act
        var status = invoice.getPaymentStatus();

        //// Assert
        expect (status == 'Paid' || status == 'Unpaid').toBeTruthy();
    }));

    it('should be able to call hasOrder', inject(function(invoiceDisplayBuilder, invoiceMocks) {
        //// Arrange
        var json = invoiceMocks.randomInvoice();
        var invoice = invoiceDisplayBuilder.transform(json);

        //// Act
        var hasOrders = invoice.hasOrder();
        var fulfillmentStatus = invoice.getFulfillmentStatus();

        //// Assert
        expect (hasOrders).toBeDefined();
        expect(fulfillmentStatus).toBeDefined();
    }));

    it('should be able to retrieve a shipment line item', inject(function(invoiceDisplayBuilder, invoiceMocks) {
       //// Arrange
        var json = invoiceMocks.randomInvoice();
        var invoice = invoiceDisplayBuilder.transform(json);

        //// Act
        //var lineItem = invoice.getShippingLineItems();

        //// expect
        //expect(lineItem).toBeDefined();
    }));

});
