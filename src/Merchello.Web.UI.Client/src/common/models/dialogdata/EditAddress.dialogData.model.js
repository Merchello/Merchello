    /**
     * @ngdoc model
     * @name EditAddressDialogData
     * @function
     *
     * @description
     * A back office model used for address data to the dialogService
     *
     */
    var EditAddressDialogData = function() {
        var self = this;
        self.address = {};
        self.countries = [];
        self.selectedCountry = {};
        self.selectedProvince = {};
    };

    angular.module('merchello.models').constant('EditAddressDialogData', EditAddressDialogData);