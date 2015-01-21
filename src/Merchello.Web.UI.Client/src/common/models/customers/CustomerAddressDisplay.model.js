    /**
     * @ngdoc model
     * @name CustomerAddressDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CustomerAddressDisplay object
     */
    var CustomerAddressDisplay = function() {
        var self = this;
        self.key = '';
        self.label = '';
        self.customerKey = '';
        self.fullName = '';
        self.address1 = '';
        self.address2 = '';
        self.locality = '';
        self.region = '';
        self.postalCode = '';
        self.addressType = '';
        self.addressTypeFieldKey = '';
        self.company = '';
        self.countryCode = '';
        self.phone = '';
        self.isDefault = false;
    };

    angular.module('merchello.models').constant('CustomerAddressDisplay', CustomerAddressDisplay);