'use strict';

describe('WarehouseDisplay prototype methods', function() {

    var mocks, builder;

    beforeEach(module('umbraco'));

    beforeEach(inject(function(warehouseMocks, warehouseDisplayBuilder) {
        mocks = warehouseMocks;
        builder = warehouseDisplayBuilder;
    }));

    it('getAddress should return the warehouse address', function() {
        //// Arrange
        var json = mocks.defaultWarehouse();
        var warehouse = builder.transform(json);

        //// Act
        var address = warehouse.getAddress();

        //// Assert
        expect(address).toBeDefined();
        expect(address.locality).toBe('Bellingham');

    });

});
