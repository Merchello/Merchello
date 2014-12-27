    /**
     * @ngdoc model
     * @name Merchello.Models.Country
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CountryDisplay object
     */
    Merchello.Models.Country = function() {
        var self = this;

        self.key = '';
        self.countryCode = '';
        self.name = '';
        self.provinceLabel = '';
        self.provinces = [];
    };
