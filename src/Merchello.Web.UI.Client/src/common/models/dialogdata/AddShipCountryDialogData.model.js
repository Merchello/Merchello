    /**
     * @ngdoc model
     * @name AddShipCountryDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for adding ship countries to shipping configurations.
     */
    var AddShipCountryDialogData = function() {
        var self = this;
        self.availableCountries = [];
        self.selectedCountry = {};
    };

    angular.module('merchello.models').constant('AddShipCountryDialogData', AddShipCountryDialogData);
