'use strict';

/* jasmine specs for startfrom filter go here */

describe('startfrom filter', function () {

    beforeEach(angular.mock.module('umbraco'));

    it('has a startfrom filter', inject(function (startfromFilter) {
        expect(startfromFilter([1, 2], 1)).not.toBeNull();
    }));

    // test cases - testing for success
    var list = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13];
    var startFrom = 3;
    var listTarget = [4, 5, 6, 7, 8, 9, 10, 11, 12, 13];

    // test cases - testing for failure
    var listTargetFalse = [3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13];

    it("should start at the third item", inject(function (startfromFilter) {
        expect(startfromFilter(list, startFrom)).toEqual(listTarget);
        expect(startfromFilter(list, startFrom)).toNotEqual(listTargetFalse);
    }));
});
