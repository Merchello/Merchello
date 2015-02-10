    /**
     * @ngdoc model
     * @name OrderStatusDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's OrderStatusDisplay object
     */
    var OrderStatusDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.alias = '';
        self.reportable = '';
        self.active = true;
        self.sortOrder = '';
    };

    angular.module('merchello.models').constant('OrderStatusDisplay', OrderStatusDisplay);