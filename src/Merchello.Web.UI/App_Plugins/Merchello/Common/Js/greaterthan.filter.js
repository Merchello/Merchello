(function (filters, undefined) {

    /**
     * @ngdoc filter
     * @name greaterthan
     * @function
     * 
     * @description
     * filter where value is greater than specified value
     */
    filters.greaterthan = function() {
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

    angular.module("umbraco").filter('greaterthan', merchello.Filters.greaterthan);

}(window.merchello.Filters = window.merchello.Filters || {}));

