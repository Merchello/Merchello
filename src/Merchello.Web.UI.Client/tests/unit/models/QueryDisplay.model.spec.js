'use strict';

describe('QueryDisplay', function() {

    beforeEach(module('umbraco'));

    it('should be able to add a parameter', function() {
        //// Arrange
        var query = new QueryDisplay();
        var parameter = new QueryParameterDisplay();
        parameter.fieldName = 'testField';
        parameter.value = 'testValue';

        //// Act
        query.addParameter(parameter);

        //// Assert
        expect(query.parameters.length).toBe(1);
    });

});
