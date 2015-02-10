'use strict';

describe("currencyDisplayBuilder", function () {

    var jsonResult = [
        { name: 'US Dollar', currencyCode: 'USD', symbol: '$' },
        { name: 'Euro', currencyCode: 'EUR', symbol: '\u20AC' }
    ];

    beforeEach(module('umbraco'));

    it ('should be possible to create a default CurrencyDisplay', inject(function(currencyDisplayBuilder, CurrencyDisplay) {

        //// Arrange
        var expected = new CurrencyDisplay();

        //// Act
        var currency = currencyDisplayBuilder.createDefault();

        //// Assert
        expect (currency).toBeDefined();
        expect (expected).toEqual(currency)

    }));

    it ('should be possible to transform a jsonResult into CurrencyDisplay', inject(function(currencyDisplayBuilder) {
        //// Arrange
        // handled in root describe

        //// Act
        var currencies = currencyDisplayBuilder.transform(jsonResult);

        //// Assert
        expect (currencies.length).toBe(2);
        expect (_.isEqual(jsonResult [ 0 ], currencies [ 0 ]));
        expect (_.isEqual(jsonResult [ 1 ], currencies [ 1 ]));
    }));

    it ('should be possible to transform a single jsonResult into a CurrencyDisplay', inject(function(currencyDisplayBuilder) {
        //// Arrange
        // handled in root describe

        //// Act
        var currency = currencyDisplayBuilder.transform(jsonResult[ 0 ]);

        //// Assert
        expect (currency).toBeDefined();
        expect (jsonResult[ 0 ].name).toBe(currency.name);
        expect (jsonResult[ 0 ].currencyCode).toBe(currency.currencyCode);
        expect (jsonResult[ 0 ].symbol).toEqual(currency.symbol);
    }));

});
