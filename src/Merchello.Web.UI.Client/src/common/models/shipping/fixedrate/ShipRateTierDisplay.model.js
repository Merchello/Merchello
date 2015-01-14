    /**
     * @ngdoc model
     * @name ShipRateTierDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipRateTierDisplay object
     */
    var ShipRateTierDisplay = function() {
        var self = this;
        self.key = '';
        self.shipMethodKey = '';
        self.rangeLow = 0.0;
        self.rangeHigh = 0.0;
        self.rate = 0.0;
    };

    angular.module('merchello.models').constant('ShipRateTierDisplay', ShipRateTierDisplay);