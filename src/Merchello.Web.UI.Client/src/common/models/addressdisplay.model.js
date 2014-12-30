    /**
    * @ngdoc model
    * @name AddressDisplay
    * @function
    * 
    * @description
    * Represents a JS version of Merchello's AddressDisplay object
    */
    var AddressDisplay = function () {

        var self = this;

        self.name = '';
        self.address1 = '';
        self.address2 = '';
        self.locality = '';
        self.region = '';
        self.postalCode = '';
        self.countryCode = '';
        self.addressType = '';
        self.organization = '';
        self.phone = '';
        self.email = '';
        self.isCommercial = false;
    };

    angular.module('merchello.models').constant('AddressDisplay', AddressDisplay);
