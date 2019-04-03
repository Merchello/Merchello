    /**
    * @ngdoc model
    * @name ShipmentDisplay
    * @function
    * 
    * @description
    * Represents a JS version of Merchello's ShipmentDisplay object
    */
    var ShipmentDisplay = function () {

        var self = this;

        self.key = '';
        self.shipmentNumber = '';
        self.shipmentNumberPrefix = '';
        self.versionKey = '';
        self.fromOrganization = '';
        self.fromName = '';
        self.fromAddress1 = '';
        self.fromAddress2 = '';
        self.fromLocality = '';
        self.fromRegion = '';
        self.fromPostalCode = '';
        self.fromCountryCode = '';
        self.fromCountryName = '';
        self.fromIsCommercial = '';
        self.toOrganization = '';
        self.toName = '';
        self.toAddress1 = '';
        self.toAddress2 = '';
        self.toLocality = '';
        self.toRegion = '';
        self.toPostalCode = '';
        self.toCountryCode = '';
        self.toCountryName = '';
        self.toIsCommercial = '';
        self.shipMethodKey = '';
        self.phone = '';
        self.email = '';
        self.carrier = '';
        self.trackingCode = '';
        self.trackingUrl = '';
        self.shippedDate = '';
        self.items = [];
        self.shipmentStatus = {};
    };

    // Shipment Prototype
    // ------------------------------------------------
    ShipmentDisplay.prototype = (function () {

        //// Private members
            // returns the shipment destination as an Address
        var getDestinationAddress = function() {
                return buildAddress.call(this, this.toName, this.toAddress1, this.toAddress2, this.toLocality, this.toRegion,
                    this.toPostalCode, this.toCountryCode, this.toCountryName, this.toOrganization, this.toIsCommercial, this.phone, this.email, 'shipping');
            },

            // returns the shipment origin as an Address
            getOriginAddress = function() {
                return buildAddress.call(this, this.fromName, this.fromAddress1, this.fromAddress2, this.fromLocality,
                    this.fromRegion, this.fromPostalCode, this.fromCountryCode, this.fromCountryName, this.fromOrganization,
                    this.fromIsCommercial, '', '', 'shipping');
            },

            setDestinationAddress = function(address)
            {
                this.toName = address.name;
                this.toAddress1 = address.address1;
                this.toAddress2 = address.address2;
                this.toLocality = address.locality;
                this.toRegion = address.region;
                this.toPostalCode = address.postalCode;
                this.toCountryCode = address.countryCode;
                this.toCountryName = address.countryName;
                this.toOrganization = address.organization;
                this.toIsCommercial = address.isCommercial;
                this.phone = address.phone;
                this.email = address.email;
            },

            setOriginAddress = function(address) {
                this.fromName = address.name;
                this.fromAddress1 = address.address1;
                this.fromAddress2 = address.address2;
                this.fromLocality = address.locality;
                this.fromRegion = address.region;
                this.fromPostalCode = address.postalCode;
                this.fromCountryCode = address.countryCode;
                this.fromCountryName = address.countryName;
                this.fromOrganization = address.organization;
                this.fromIsCommercial = address.isCommercial;
            },

            // Utility to build an address
            buildAddress = function (name, address1, address2, locality, region, postalCode, countryCode, countryName, organization,
                                    isCommercial, phone, email, addressType) {
                var adr = new AddressDisplay();
                adr.name = name;
                adr.address1 = address1;
                adr.address2 = address2;
                adr.locality = locality;
                adr.region = region;
                adr.postalCode = postalCode;
                adr.countryCode = countryCode;
                adr.countryName = countryName;
                adr.organization = organization;
                adr.isCommercial = isCommercial;
                adr.phone = phone;
                adr.email = email;
                adr.addressType = addressType;
                return adr;
            };

        // public members
        return {
            /**
            * @ngdoc method
            * @name merchello.models.Shipment.getDestinationAddress
            * @function
            *
            * @description
            * Returns a merchello.models.Address representing the shipment destination
            */
            getDestinationAddress: getDestinationAddress,

            /**
            * @ngdoc method
            * @name merchello.models.Shipment.getOriginAddress
            * @function
            *
            * @description
            * Returns a merchello.models.Address representing the shipment origin
            */
            getOriginAddress: getOriginAddress,

            /**
             * @ngdoc method
             * @name merchello.models.Shipment.setDestinationAddress
             * @function
             *
             * @description
             * Sets the destination address for a shipment
             */
            setDestinationAddress: setDestinationAddress,

            /**
             * @ngdoc method
             * @name merchello.models.Shipment.setOriginAddress
             * @function
             *
             * @description
             * Sets the origin address for a shipment
             */
            setOriginAddress: setOriginAddress
        };
    }());

    angular.module('merchello.models').constant('ShipmentDisplay', ShipmentDisplay);
