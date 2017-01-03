/*! Merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2017 Across the Pond, LLC.
 * Licensed MIT
 */

(function() { 

    /**
     * @ngdoc filter
     * @name greaterthan
     * @function
     * 
     * @description
     * filter where value is greater than specified value
     */
    var greaterthan = function() {
            return function (inputArr, propName, minValue) {
                var filterResult = [];
                angular.forEach(inputArr, function(inputItem) {
                if (inputItem[propName] > minValue) {
                    filterResult.push(inputItem);
                }
            });

            return filterResult;
	    };
    };

    angular.module('merchello.filters').filter('greaterthan', greaterthan);



    /**
     * @ngdoc filter
     * @name startfrom
     * @function
     * 
     * @description
     * We already have a limitTo filter built-in to angular,
     * let's make a startFrom filter 
     * Ref: http://jsfiddle.net/2ZzZB/56/
     */
    var startfrom = function() {
        return function(input, start) {
            start = +start; //parse to int
            return input.slice(start);
        };
    };

    angular.module('merchello.filters').filter('startfrom', startfrom);


    /**
     * @ngdoc filter
     * @name timeformat
     * @function
     *
     * @description
     * Filter to convert time formats between 24 hour and 12 hour
     */

    var timeFormat = function() {
        return function(date, setting) {
            if (setting === 'am-pm')
            {
                var hours = date.getHours();
                var minutes = date.getMinutes();
                var ampm = hours >= 12 ? 'pm' : 'am';
                hours = hours % 12;
                hours = hours ? hours : 12; // the hour '0' should be '12'
                minutes = minutes < 10 ? '0'+minutes : minutes;
                var strTime = hours + ':' + minutes + ' ' + ampm;
                return strTime;
            }
            else
            {
                return date.getHours() + ':' + date.getMinutes();
            }
        };
    };

    angular.module('merchello.filters').filter('timeformat', timeFormat);


})();