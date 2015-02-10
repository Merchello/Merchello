'use strict';

describe('QueryDisplay', function() {

    beforeEach(module('umbraco'));

    it('should be able to add a parameter', inject(function(QueryDisplay, QueryParameterDisplay) {
        //// Arrange
        var query = new QueryDisplay();
        var parameter = new QueryParameterDisplay();
        parameter.fieldName = 'testField';
        parameter.value = 'testValue';

        //// Act
        query.addParameter(parameter);

        //// Assert
        expect(query.parameters.length).toBe(1);
        expect(_.isEqual(query.parameters[0], parameter));
    }));

    it('should be able to add a filter parameter', inject(function(QueryDisplay) {

        //// Arrange
        var query = new QueryDisplay();

        //// Act
        query.addFilterTermParam('test');

        //// Assert
        expect (query.parameters.length).toBe(1);
        expect (query.parameters[0].fieldName = 'term');
        expect (query.parameters[0].value = 'test');
    }));
});
