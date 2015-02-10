'use strict';

describe('ExtendedDataDisplay prototype methods', function() {

    beforeEach(module('umbraco'));

    it('getValue should return a value if the value is present', inject(function(extendedDataDisplayBuilder, extendedDataMocks) {
        //// Arrange
        var json = extendedDataMocks.getExtendedData();
        var extendedData = extendedDataDisplayBuilder.transform(json);

        //// Act
        var value = extendedData.getValue('merchShipMethodKey');

        //// expect
        expect (value).toBe('7f236971-4342-4515-96f2-d38045e6014b');
    }));

});
