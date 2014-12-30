'use strict';

describe("currencyDisplayBuilder", function () {

    var jsonResult = [
        { name: 'US Dollar', currencyCode: 'USD', symbol: '$' },
        { name: 'Euro', currencyCode: 'EUR', symbol: '\u20AC' }
    ];

    beforeEach(module('umbraco'));

    it ('should be possible to create a default CurrencyDisplay', inject(function(currencyDisplayBuilder) {

        //// Arrange
        var expected = new CurrencyDisplay();

        //// Act
        var currency = currencyDisplayBuilder.createDefault();

        //// Assert
        expect (currency).toBeDefined();
        expect (_.isEqual(expected, currency)).toBe(true);

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
        var currency = currencyDisplayBuilder.transform(jsonResult[ 1 ]);

        //// Assert
        expect (currency).toBeDefined();
        expect (jsonResult[ 1 ].name).toBe(currency.name);
        expect (jsonResult[ 1 ].currencyCode).toBe(currency.currencyCode);
        expect (jsonResult[ 1 ].symbol).toBe(currency.symbol);
    }));

});
