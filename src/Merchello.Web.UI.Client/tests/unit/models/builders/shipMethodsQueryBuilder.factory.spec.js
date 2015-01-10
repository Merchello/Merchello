'use strict';

describe('shipMethodsQueryBuilder', function() {

    beforeEach(module('umbraco'));

    it('should be able to parse an API response', inject(function(shipMethodsQueryDisplayBuilder, shipMethodsQueryMock) {
        //// Arrange
        var jsonResult = shipMethodsQueryMock.singleShipMethod();

        //// Act
        var query = shipMethodsQueryDisplayBuilder.transform(jsonResult);

        //// Assert
        expect(query.selected).toBeDefined();
        expect(query.selected.provinces).toBeDefined();
        expect(query.selected.provinces.length).toBeGreaterThan(50);
        expect(query.alternatives).toBeDefined();
        expect(query.alternatives[0].provinces).toBeDefined();
        expect(query.alternatives[0].provinces.length).toBeGreaterThan(50);
    }))

});
