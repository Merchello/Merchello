    /**
     * @ngdoc model
     * @name ProductAttributeDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProductOptionDisplay object
     */
    var ProductOptionDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.required = true;
        self.sortOrder = 1;
        self.choices = [];
    };

    angular.module('merchello.models').constant('ProductOptionDisplay', ProductOptionDisplay);