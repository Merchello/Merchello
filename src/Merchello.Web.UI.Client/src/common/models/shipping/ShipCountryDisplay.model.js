    /**
     * @ngdoc model
     * @name ShipCountryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipCountryDisplay object
     */
    var ShipCountryDisplay = function() {
        var self = this;
        self.key = '';
        self.catalogKey = '';
        self.countryCode = '';
        self.name = '';
        self.provinceLabel = '';
        self.provinces = [];
        self.hasProvinces = false;
    };

    angular.module('merchello.models').constant('ShipCountryDisplay', ShipCountryDisplay);