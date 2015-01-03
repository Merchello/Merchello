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

});
