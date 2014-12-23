'use strict';

/* jasmine specs for startfrom filter go here */

describe('catalog directives', function () {

    var elm, scope;

    beforeEach(angular.mock.module('umbraco'));
    beforeEach(module('merchellotemplates'));

    beforeEach(inject(function ($rootScope, $compile) {
        elm = angular.element('<product-variant-main-properties product-variant="productVariant" creating-variant="creatingVariant" editing-variant="editingVariant"></product-variant-main-properties>');

        scope = $rootScope;

        scope.productVariant = new merchello.Models.ProductVariant();
        scope.productVariant.price = 45.00; 
        scope.productVariant.taxable = true;
        scope.creatingVariant = true;
        scope.editingVariant = false;

        $compile(elm)(scope);
        scope.$digest();
    }));

    it('should have several divs', function () {
        var divs = elm.find('div');

        expect(divs.length).toBe(12);

        // .row-fluid items
        var sections = elm.children();

        expect(sections.length).toBe(4);
    });

    it('should have correct data in the price field', function () {
        var $priceInput = $(elm).find('input#price');

        expect($priceInput.length).toBe(1);
        expect($priceInput.attr('name')).toBe('price');

        expect($priceInput[0].outerHTML).toBe(45.00);

    });

    it('should have taxable checked', function () {
        var $checkbox = $(elm).find('input[name="taxable"]');

        expect($checkbox.length).toBe(1);
        expect($checkbox.attr('name')).toBe('taxable');

        expect($checkbox.is(':checked')).toBe(true);

    });

    it('should have shippable unchecked', function () {
        var $checkbox = $(elm).find('input[name="shippable"]');

        expect($checkbox.length).toBe(1);
        expect($checkbox.attr('name')).toBe('shippable');

        expect($checkbox.is(':checked')).toBe(false);

    });
});
