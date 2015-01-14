    /**
     * @ngdoc model
     * @name ShipFixedRateTableDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipFixedRateTableDisplay object
     */
    var ShipFixedRateTableDisplay = function() {
        var self = this;
        self.shipMethodKey = '';
        self.shipCountryKey = '';
        self.rows = [];
    };

    angular.module('merchello.models').constant('ShipFixedRateTableDisplay', ShipFixedRateTableDisplay);