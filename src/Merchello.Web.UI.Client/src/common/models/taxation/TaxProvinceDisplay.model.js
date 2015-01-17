    /**
     * @ngdoc model
     * @name TaxProvinceDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's TaxProvinceDisplay object
     */
    var TaxProvinceDisplay = function() {
        var self = this;
        self.name = '';
        self.code = '';
        self.percentAdjustment = 0;
    };

    angular.module('merchello.models').constant('TaxProvinceDisplay', TaxProvinceDisplay);