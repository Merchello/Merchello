    /**
     * @ngdoc model
     * @name TaxMethodDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's TaxMethodDisplay object
     */
    var TaxMethodDisplay = function() {
        var self = this;
        self.key = '';
        self.providerKey = '';
        self.name = '';
        self.countryCode = '';
        self.percentageTaxRate = 0.0;
        self.provinces = [];
    };

    angular.module('merchello.models').constant('TaxMethodDisplay', TaxMethodDisplay);
