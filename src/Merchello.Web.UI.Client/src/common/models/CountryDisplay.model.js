    /**
     * @ngdoc model
     * @name CountryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CountryDisplay object
     */
    var CountryDisplay = function() {
        var self = this;

        self.key = '';
        self.countryCode = '';
        self.name = '';
        self.provinceLabel = '';
        self.provinces = [];
    };

    CountryDisplay.prototype = (function() {

        function hasProvinces() {
            return this.provinces.length > 0;
        }

        return {
            hasProvinces: hasProvinces
        };

    }());

    angular.module('merchello.models').constant('CountryDisplay', CountryDisplay);