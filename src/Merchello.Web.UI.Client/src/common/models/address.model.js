
    /**
    * @ngdoc model
    * @name merchello.models.address
    * @function
    * 
    * @description
    * Represents a JS version of Merchello's AddressDetails object
    */
    merchello.models.Address = function() {
        this.name = '';
        this.address1 = '';
        this.address2 = '';
        this.locality = '';
        this.region = '';
        this.postalCode = '';
        this.countryCode = '';
        this.addressType = '';
        this.organization = '';
        this.phone = '';
        this.email = '';
        this.isCommercial = false;
    };

