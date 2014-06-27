'use strict';

/* jasmine specs for startfrom filter go here */

describe('catalog models', function () {

    var product = new merchello.Models.Product();

    beforeEach(angular.mock.module('umbraco'));
    beforeEach(angular.mock.module('umbraco.resources'));
    beforeEach(function() {
        product = new merchello.Models.Product();
    });

    it('should have a valid empty Product', function() {
        expect(product).not.toBe(null);
        expect(product.price).toEqual(0.0);
    });

    it('product should have a valid empty ProductOption and it can be removed', function () {
        product.addBlankOption();
        expect(product.productOptions.length).toBe(1);
        expect(product.productOptions[0]).not.toBe(null);
        product.removeOption(product.productOptions[0]);
        expect(product.productOptions.length).toBe(0); 
    });

});
