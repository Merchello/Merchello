    /**
     * @ngdoc model
     * @name AddShipCountryDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for adding ship countries to shipping configurations.
     */
    var AddEditWarehouseDialogData = function() {
        var self = this;
        self.warehouse = {};
        self.availableCountries = [];
        self.selectedCountry = {};
        self.selectedProvince = {};
    };

    angular.module('merchello.models').constant('AddEditWarehouseDialogData', AddEditWarehouseDialogData);
