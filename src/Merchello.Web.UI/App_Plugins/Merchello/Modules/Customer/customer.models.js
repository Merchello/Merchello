(function (models, undefined) {

    models.Customer = function(customerSource) {

        var self = this;

        if (customerSource == undefined) {
            self.addresses = [];
            self.email = '';
            self.extendedData = [];
            self.firstName = '';
            self.key = '';
            self.lastActivityDate = '';
            self.lastName = '';
            self.loginName = '';
            self.notes = '';
            self.taxExempt = false;
        } else {
            self.addresses = _.map(customerSource.addresses, function(address) {
                return new merchello.Models.CustomerAddress(address);
            });
            self.email = customerSource.email;     
            self.extendedData = _.map(customerSource.extendedData, function (item) {
                return new merchello.Models.DictionaryItem(item);
            });
            self.firstName = customerSource.firstName;
            self.key = customerSource.key;
            self.lastActivityDate = customerSource.lastActivityDate;
            self.lastName = customerSource.lastName;
            self.loginName = customerSource.loginName;
            self.notes = customerSource.notes;
            self.taxExempt = customerSource.taxExempt;
        }

        self.primaryLocation = function () {
            var result = '';
            // TODO: Change logic to return locality, region of primary address instead of first one.
            if (self.addresses.length > 0) {
                var address;
                var primaryAddress = false;
                for (var i = 0; i < self.addresses.length; i++) {
                    if (self.addresses[i].isDefault) {
                        primaryAddress = self.addresses[i];
                    }
                }
                if (!primaryAddress) {
                    primaryAddress = self.addresses[0];
                }
                result = primaryAddress.locality + ', ' + primaryAddress.region;
            }
            return result;
        };

    };

    models.CustomerAddress = function(customerAddressSource) {

        var self = this;

        if (customerAddressSource == undefined) {
            self.address1 = '';
            self.address2 = '';
            self.addressType = '';
            self.addressTypeFieldKey = '';
            self.company = '';
            self.countryCode = '';
            self.customerKey = '';
            self.fullName = '';
            self.isDefault = false;
            self.key = '';
            self.label = '';
            self.locality = '';
            self.phone = '';
            self.postalCode = '';
            self.region = '';
        } else {
            self.address1 = customerAddressSource.address1;
            self.address2 = customerAddressSource.address2;
            self.addressType = customerAddressSource.addressType;
            self.addressTypeFieldKey = customerAddressSource.addressTypeFieldKey;
            self.company = customerAddressSource.company;
            self.countryCode = customerAddressSource.countryCode;
            self.customerKey = customerAddressSource.customerKey;
            self.fullName = customerAddressSource.fullName;
            self.isDefault = customerAddressSource.isDefault;
            self.key = customerAddressSource.key;
            self.label = customerAddressSource.label;
            self.locality = customerAddressSource.locality;
            self.phone = customerAddressSource.phone;
            self.postalCode = customerAddressSource.postalCode;
            self.region = customerAddressSource.region;
        }

    };

    models.DictionaryItem = function(dictionarySource) {

        var self = this;

        if (dictionarySource === undefined) {
            self.key = '';
            self.value = '';
        } else {
            self.key = dictionarySource.key;
            self.value = dictionarySource.value;
        }

    };

}(window.merchello.Models = window.merchello.Models || {}));