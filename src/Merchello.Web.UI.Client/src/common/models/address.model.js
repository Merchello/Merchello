    /**
    * @ngdoc model
    * @name Merchello.Models.Address
    * @function
    * 
    * @description
    * Represents a JS version of Merchello's AddressDisplay object
    */
    Merchello.Models.Address = function () {

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
