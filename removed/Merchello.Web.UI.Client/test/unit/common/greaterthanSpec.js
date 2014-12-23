'use strict';

/* jasmine specs for startfrom filter go here */

describe('greaterthan filter', function () {

    beforeEach(angular.mock.module('umbraco'));

    it('has a greaterthan filter', inject(function (greaterthanFilter) {
        expect(greaterthanFilter([{test: 1}], 'test', 0)).not.toBeNull();
    }));

    // test cases - testing for success
    var list = [{ test: 1, it: 5 }, { test: 5, it: 10 }, { test: 6, it: 1 }];
    var prop = 'test';
    var value = 4;
    var listTarget = [{ test: 5, it: 10 }, { test: 6, it: 1 }];

    // test cases - testing for failure
    var listTargetFalse = [{ test: 1, it: 5 }, { test: 5, it: 10 }, { test: 6, it: 1 }];

    it("should filter out items less than 5", inject(function (greaterthanFilter) {
        expect(greaterthanFilter(list, prop, value)).toEqual(listTarget);
        expect(greaterthanFilter(list, prop, value)).toNotEqual(listTargetFalse);
    }));
});
