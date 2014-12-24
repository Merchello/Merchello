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

