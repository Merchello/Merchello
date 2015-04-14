'use strict';

describe('Merchello.Backoffice.OrderShipmentsController', function () {
    var invoiceHelper;

    beforeEach(module('umbraco'));

    beforeEach(inject(function(invoiceHelper) {
        invoiceHelper = invoiceHelper;
    }));

    xit('should pad left 2 to 02', function() {
        var expected = '02';

        var value = invoiceHelper.padLeft('2', '0', 2);

        expect(value).toEqual(expected);
    });
});
