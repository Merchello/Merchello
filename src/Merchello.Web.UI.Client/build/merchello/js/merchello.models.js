/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2014 Merchello;
 * Licensed MIT
 */

(function(models, undefined) { 

    // Represents a JS version of Merchello's AddressDetails object
    models.Address = function () {
        this.address1 = '';
        this.address2 = '';
        this.locality = '';
        this.region = '';
        this.postalCode = '';
        this.countryCode = '';
        this.addressType = '';
        this.organization = '';
        this.name = '';
        this.phone = '';
        this.email = '';
        this.isCommercial = false;
    }

    //// Prototype
    models.Address.prototype = function() {

        // private members
        //// initializes the address with resource data
        
        // TODO move this to a generic mixin
        // http://javascriptweblog.wordpress.com/2011/05/31/a-fresh-look-at-javascript-mixins/
        var populate = function(data) {
            if (data != undefined) {
                this.address1 = data.address1;
                this.address2 = data.address2;
                this.locality = data.locality;
                this.region = data.region;
                this.postalCode = data.postalCode;
                this.countryCode = data.countryCode;
                this.addressType = data.addressType;
                this.organization = data.organization;
                this.name = data.name;
                this.phone = data.phone;
                this.email = data.email;
                this.isCommercial = data.isCommercial;
            }
        }

        // public members
        return {
            populate: populate
        };
    }


}(merchello.Models = merchello.Models || {}));