(function (models, undefined) {

    models.Customer = function(customerSource) {

        var self = this;

        if (customerSource === undefined) {
            self.key = "";
            self.firstName = "";
            self.lastName = "";
            self.email = "";
            self.loginName = "";
            self.taxExempt = false;
            self.lastActivityDate = "";
            self.extendedData = [];
        } else {
            self.key = customerSource.key;
            self.firstName = customerSource.firstName;
            self.lastName = customerSource.lastName;
            self.email = customerSource.email;
            self.loginName = customerSource.loginName;
            self.taxExempt = customerSource.taxExempt;
            self.lastActivityDate = customerSource.lastActivityDate;
            self.extendedData = _.map(customerSource.extendedData, function (method) {
                return new merchello.Models.DictionaryItem(method);
            });
        }

    };

    models.DictionaryItem = function(dictionarySource) {

        var self = this;

        if (dictionarySource === undefined) {
            self.key = "";
            self.value = "";
        } else {
            self.key = dictionarySource.key;
            self.value = dictionarySource.value;
        }

    };

}(window.merchello.Models = window.merchello.Models || {}));