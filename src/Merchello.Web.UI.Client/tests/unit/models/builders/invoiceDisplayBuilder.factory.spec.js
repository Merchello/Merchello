'use strict';

xdescribe('invoiceDisplayBuilder', function() {

    beforeEach(module('umbraco'));

    it('should be able to create a default invoice', inject(function(invoiceDisplayBuilder) {

        //// Arrange
        // nothing to do

        //// Act
        var invoice = invoiceDisplayBuilder.createDefault();
        console.info(invoice);

        //// Assert
        expect(invoice).toBeDefined();
        expect(invoice.invoiceStatus.key).toBeDefined();

    }));

});
