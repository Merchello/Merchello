'use strict';

describe('merchello.module assertions', function() {

    beforeEach(module('umbraco'));

    xit ('should be when merchello is bootstraped that umbraco.packages has the correct umbraco modules injected', function() {
        //// Arrange
        // this is done in the merchello module setup

        //// Act
        var module = angular.module('umbraco.packages');
        var requires = module.requires;

        //// Assert
        //// jQuery's inArray returns the index of the value or -1
        expect ($.inArray('umbraco.filters', requires) > -1).toBe(true);
        expect ($.inArray('umbraco.directives', requires) > -1).toBe(true);
        expect ($.inArray('umbraco.services', requires) > -1).toBe(true);
    })

});
