(function (models, undefined) {

    models.Customer = function(customerSource) {

        var self = this;

        if (customerSource === undefined) {
            self.addresses = [];
            self.email = '';
            self.extendedData = [];
            self.firstName = '';
            self.key = '';
            self.lastActivityDate = '';
            self.lastName = '';
            self.loginName = '';
            self.notes = [];
            self.taxExempt = false;
        } else {
            self.addresses = _.map(customerSource.addresses, function(address) {
                return new merchello.Models.Address(address);
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
            self.taxExempt = customerSource.taxExempt;
        }

        self.primaryLocation = function () {
            var result = '';
            // TODO: Change logic to return locality, region of primary address instead of first one.
            if (self.addresses.length > 0) {
                var address = self.addresses[0];
                result = address.locality + ', ' + address.region;
            }
            return result;
        };

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