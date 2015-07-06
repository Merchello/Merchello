    /**
     * @ngdoc model
     * @name TaxCountryDisplay
     * @function
     *
     * @description
     * Represents a TaxCountryDisplay
     *
     * @note
     * There is not a corresponding Merchello model
     */
    var TaxCountryDisplay = function() {
        var self = this;
        self.name = '';
        self.country = {};
        self.provider = {};
        self.taxMethod = {};
        self.gatewayResource = {};
        self.addTaxesToProduct = false;
        self.sortOrder = 0;
    };

    TaxCountryDisplay.prototype = (function() {

        function setCountryName(str) {
            this.name = str;
        }

        return {
            setCountryName: setCountryName
        };

    }());

    angular.module('merchello.models').constant('TaxCountryDisplay', TaxCountryDisplay);