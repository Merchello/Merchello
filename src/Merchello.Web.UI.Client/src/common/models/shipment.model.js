    /**
    * @ngdoc model
    * @name merchello.models.shipment
    * @function
    * 
    * @description
    * Represents a JS version of Merchello's ShipmentDetails object
    */
    merchello.models.Shipment = function () {
            this.key = '';
            this.shipmentNumber = '';
            this.shipmentNumberPrefix = '';
            this.versionKey = '';
            this.fromOrganization = '';
            this.fromName = '';
            this.fromAddress1 = '';
            this.fromAddress2 = '';
            this.fromLocality = '';
            this.fromRegion = '';
            this.fromPostalCode = '';
            this.fromCountryCode = '';
            this.fromIsCommercial = '';
            this.toOrganization = '';
            this.toName = '';
            this.toAddress1 = '';
            this.toAddress2 = '';
            this.toLocality = '';
            this.toRegion = '';
            this.toPostalCode = '';
            this.toCountryCode = '';
            this.toIsCommercial = '';
            this.shipMethodKey = '';
            this.phone = '';
            this.email = '';
            this.carrier = '';
            this.trackingCode = '';
            this.shippedDate = '';
            this.items = [];
            this.shipmentStatus = new merchello.models.ShipmentStatus();
    };


    merchello.models.Shipment.prototype = function() {

        //// Private members

            // returns the shipment destination as an Address
        var getDestinationAddress = function() {
            return buildAddress(this.toName, this.toAddress1, this.toAddress2, this.toLocality, this.toRegion, this.toPostalCode, this.toCountryCode, this.toOrganization, this.toIsCommercial, '', '', 'shipping');
            },

            // returns the shipment origin as an Address
            getOriginAddress = function() {
                return buildAddress(this.fromName, this.fromAddress1, this.fromAddress2, this.fromLocality, this.fromRegion, this.fromPostalCode, this.fromCountryCode, this.fromOrganization, this.fromIsCommercial, '', '', 'shipping');
            },

            // utility to build an address
            buildAddress = function() {
                var adr = new merchello.models.Address();  
                adr.name = name;
                adr.address1 = address1;
                adr.address2 = address2;
                adr.locality = locality;
                adr.region = region;
                adr.postalCode = postalCode;
                adr.countryCode = countryCode;
                adr.organization = organization;
                adr.isCommercial = isCommercial;
                adr.phone = phone;
                adr.email = email;
                adr.addressType = addressType;
                return adr;
            }

        // public members
        return {
            getDestinationAddress: getDestinationAddress,
            getOriginAddress: getOriginAddress
        };

    }();
