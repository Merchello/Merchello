    /**
     * @ngdoc model
     * @name CustomerDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CustomerDisplay object
     */
    var CustomerDisplay = function() {
        var self = this;
        self.firstName = '';
        self.key = '';
        self.lastActivityDate = '';
        self.lastName = '';
        self.loginName = '';
        self.notes = '';
        self.email = '';
        self.taxExempt = false;
        self.extendedData = {};
        self.addresses = [];
        self.invoices = [];
    };

    CustomerDisplay.prototype = (function() {

        function getDefaultAddress(addressType) {
            return _.find(this.addresses, function(address) {
                return address.addressType === addressType && address.isDefault === true;
            });
        }

        function getAddressesByAddressType(addressType) {
            return _.filter(this.addresses, function(address) {
                return address.addressType === addressType;
            });
        }

        // returns a value indicating whether or not the customer has addresses
        function hasAddresses() {
            return this.addresses.length > 0;
        }

        // returns a value indicating whether or not the customer has a default address of a given type
        function hasDefaultAddressOfType(addressType) {
            var address = getDefaultAddress.call(this, addressType);
            return address !== null && address !== undefined;
        }

        // gets the default billing address
        function getDefaultBillingAddress() {
            var address = getDefaultAddress.call(this, 'billing');
            if(address === null || address === undefined) {
                address = new CustomerAddressDisplay();
                address.addressType = 'billing';
            }
            return address;
        }

        // gets the collection of billing addresses
        function getBillingAddresses() {
            return getAddressesByAddressType.call(this, 'billing');
        }

        // get default shipping address
        function getDefaultShippingAddress() {
            var address = getDefaultAddress.call(this, 'shipping');
            if(address === null || address === undefined) {
                address = new CustomerAddressDisplay();
                address.addressType = 'shipping';
            }
            return address;
        }

        // gets the shipping address collection
        function getShippingAddresses() {
            return getAddressesByAddressType.call(this, 'shipping');
        }

        // gets the last invoice billed to the customer
        function getLastInvoice() {
            if (this.invoices.length > 0) {
                var sorted = _.sortBy(this.invoices, function(invoice) {
                    return -1 * invoice.invoiceNumber;
                });
                if(sorted === undefined || sorted === null) {
                    return new InvoiceDisplay();
                } else {
                    return sorted[0];
                }
            } else {
                return new InvoiceDisplay();
            }
        }

        return {
            getLastInvoice: getLastInvoice,
            hasAddresses: hasAddresses,
            hasDefaultAddressOfType: hasDefaultAddressOfType,
            getDefaultBillingAddress: getDefaultBillingAddress,
            getBillingAddresses: getBillingAddresses,
            getDefaultShippingAddress: getDefaultShippingAddress,
            getShippingAddresses: getShippingAddresses
        }

    }());

    angular.module('merchello.models').constant('CustomerDisplay', CustomerDisplay);
