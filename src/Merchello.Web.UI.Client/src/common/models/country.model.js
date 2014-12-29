    /**
     * @ngdoc model
     * @name Merchello.Models.Country
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

    angular.module('merchello.models').constant('CountryDisplay', CountryDisplay);