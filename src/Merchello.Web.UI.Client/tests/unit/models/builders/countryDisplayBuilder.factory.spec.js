'use strict';

describe("countryDisplayBuilder", function () {

    var jsonResult = [
        {
            key: '303ADC1C-14C9-4DD1-AD25-E26875B9EBAC',
            countryCode: 'US',
            name: 'United States of America',
            provinceLabel: 'States',
            provinces: [
                { name: 'Alabama', code: 'AL' },
                { name: 'Alaska', code: 'AK' },
                { name: 'Arizona', code: 'AZ' }
            ]
        },
        {
            key: '',
            countryCode: 'UK',
            name: 'United Kingdom',
            provinceLabel: '',
            provinces: []
        }
    ];

    beforeEach(module('umbraco'));

    it ('should be possible to create a default CountryDisplay', inject(function(countryDisplayBuilder, CountryDisplay) {

        //// Arrange
        var expected = new CountryDisplay();

        //// Act
        var country = countryDisplayBuilder.createDefault();

        //// Assert
        expect (country).toBeDefined();
        expect (_.isEqual(expected, country)).toBe(true);

    }));

    it ('should be possible to transform a jsonResult into CountryDisplay', inject(function(countryDisplayBuilder) {
        //// Arrange
        // handled in root describe

        //// Act
        var countries = countryDisplayBuilder.transform(jsonResult);

        //// Assert
        expect (countries.length).toBe(2);
        expect (_.isEqual(jsonResult [ 0 ], countries [ 0 ]));
        expect (_.isEqual(jsonResult [ 1 ], countries [ 1 ]));
    }));

    it ('should be possible to transform a single jsonResult into a CountryDisplay', inject(function(countryDisplayBuilder) {
        //// Arrange
        // handled in root describe

        //// Act
        var country = countryDisplayBuilder.transform(jsonResult[ 0 ]);

        //// Assert
        expect (country).toBeDefined();
        expect (country.provinces.length).toBe(3);
    }));

});
