    /**
     * @ngdoc model
     * @name ShipMethodDisplay
     *
     * @description
     * Represents a JS version of Merchello's ShipMethodDisplay object
     */
    var ShipMethodDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.providerKey = '';
        self.shipCountryKey = '';
        self.surchare = 0.0;
        self.serviceCode = '';
        self.taxable = false;
        self.provinces = [];
    };

    angular.module('merchello.models').constant('ShipMethodDisplay', ShipMethodDisplay);