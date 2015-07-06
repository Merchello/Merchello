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
        self.productTaxMethod = false;
    };

    TaxMethodDisplay.prototype = (function() {

        function hasProvinces() {
            return this.provinces.length > 0;
        }

        return {
            hasProvinces: hasProvinces
        };
    }());

    angular.module('merchello.models').constant('TaxMethodDisplay', TaxMethodDisplay);
