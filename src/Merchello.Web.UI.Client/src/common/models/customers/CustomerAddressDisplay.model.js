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
        self.countryName = '';
        self.phone = '';
        self.email = '';
        self.isDefault = false;
    };

    CustomerAddressDisplay.prototype = (function() {

        function isEmpty() {
            var result = false;
            if (this.address1 === '' || this.locality === '' || this.address1 === null || this.locality === null) {
                result = true;
            }
            return result;
        }

        // maps CustomerAddressDisplay to AddressDisplay
        function asAddressDisplay() {
            var address = new AddressDisplay();
            angular.extend(address, this);
            // corrections
            address.name = this.fullName;
            address.organization = this.company;
            return address;
        }

        return {
            isEmpty: isEmpty,
            asAddressDisplay: asAddressDisplay
        };
    }());

    angular.module('merchello.models').constant('CustomerAddressDisplay', CustomerAddressDisplay);