    /**
     * @ngdoc model
     * @name AddEditCustomerAddressDialogData
     * @function
     *
     * @description
     *  A dialog data object for adding or editing CustomerAddressDisplay objects
     */
    var AddEditCustomerAddressDialogData = function() {
        var self = this;
        self.customerAddress = {};
        self.countries = [];
        self.selectedCountry = {};
        self.selectedProvince = {};
        self.setDefault = false;
    };

    angular.module('merchello.models').constant('AddEditCustomerAddressDialogData', AddEditCustomerAddressDialogData);