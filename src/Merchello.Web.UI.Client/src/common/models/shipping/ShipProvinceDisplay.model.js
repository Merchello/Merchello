    /**
     * @ngdoc model
     * @name ShipMethodDisplay
     *
     * @description
     * Represents a JS version of Merchello's ShipProvinceDisplay object
     */
    var ShipProvinceDisplay = function() {
        var self = this;
        self.allowShipping = false;
        // TODO this should be converted to a string in the API for consistency
        self.rateAdjustment = 1;  // possible values are 1 & 2
    };

    angular.module('merchello.models').constant('ShipProvinceDisplay', ShipProvinceDisplay);