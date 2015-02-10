
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

