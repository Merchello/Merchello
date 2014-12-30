    /**
     * @ngdoc model
     * @name QueryParameterDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ListQueryParameterDisplay object
     */
    var QueryParameterDisplay = function()
    {
        var self = this;
        self.fieldName = '';
        self.value = '';
    };

    angular.module('merchello.models').constant('QueryParameterDisplay', QueryParameterDisplay);