'use strict';

describe('shipmentStatusDisplayBuilder', function() {

    beforeEach(module('umbraco'));

    it('should be able to parse an API result', inject(function(shipmentMocks, shipmentStatusDisplayBuilder) {
        //// Arrange
        var jsonResult = shipmentMocks.shipmentStatuses();

        //// Act
        var statuses = shipmentStatusDisplayBuilder.transform(jsonResult);

        //// Assert
        expect(statuses.length).toBe(5);
    }));

});
