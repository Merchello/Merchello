'use strict';

describe('modelBuilder.service', function() {

    beforeEach(module('umbraco'));

    it ('should be able to instantiate a TypeField', function() {
        var tf = new Merchello.Models.TypeField();

        expect (tf).toBeDefined();
        expect (typeof(tf)).toBe('object');
        expect (tf.alias).toBe('');
    });
});
