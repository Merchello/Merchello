'use strict';

describe('queryResultDisplayBuilder', function() {

    beforeEach(module('umbraco'));



    it('should be able to build an API Query Result', inject(function(invoiceMocks, queryResultDisplayBuilder, invoiceDisplayBuilder) {
        //// Arrange
        var invoices = invoiceMocks.invoicesArray();
        var mockResults = {
            currentPage: 1,
            itemsPerPage: 25,
            items: invoices,
            totalItems: 16,
            totalPages: 1
        };

        //// Act
        var result = queryResultDisplayBuilder.transform(mockResults, invoiceDisplayBuilder);

        //// Assert
        expect (result.items.length).toBe(invoices.length);
        expect (result.currentPage).toBe(1);
        expect (result.itemsPerPage).toBe(25);

    }))

});
