(function (models, undefined) {                                                                                                          

    models.Province = function (provinceFromServer) {

        var self = this;

        if (provinceFromServer == undefined) {
            self.name = "";
            self.code = "";
        } else {
            self.name = provinceFromServer.name;
            self.code = provinceFromServer.code;
        }

    };

    models.Address = function (addressFromServer) {

        var self = this;

        if (addressFromServer == undefined) {
            self.name = "";
            self.address1 = "";
            self.address2 = "";
            self.locality = "";
            self.region = "";
            self.postalCode = "";
            self.countryCode = "";
            self.email = "";
            self.phone = "";
            self.organization = "";
            self.isCommercial = false;
        } else {
            self.name = addressFromServer.name;
            self.address1 = addressFromServer.address1;
            self.address2 = addressFromServer.address2;
            self.locality = addressFromServer.locality;
            self.region = addressFromServer.region;
            self.postalCode = addressFromServer.postalCode;
            self.countryCode = addressFromServer.countryCode;
            self.email = addressFromServer.email;
            self.phone = addressFromServer.phone;
            self.organization = addressFromServer.organization;
            self.isCommercial = addressFromServer.isCommercial;
        };

    };

    models.Country = function (countryFromServer) {

        var self = this;

        if (countryFromServer == undefined) {
            self.key = "";
            self.countryCode = "";
            self.name = "";
            self.provinceLabel = "";
            self.provinces = [];
        } else {
            self.key = countryFromServer.key;
            self.countryCode = countryFromServer.countryCode;
            self.name = countryFromServer.name;
            self.provinceLabel = countryFromServer.provinceLabel;
            self.provinces = _.map(countryFromServer.provinces, function (province) {
                return new merchello.Models.Province(province);
            });
        };

    };

    models.Currency = function(currencyFromServer) {

        var self = this;

        if (currencyFromServer == undefined) {
            self.name = "";
            self.currencyCode = "";
            self.symbol = "";
        } else {
            self.name = currencyFromServer.name;
            self.currencyCode = currencyFromServer.currencyCode;
            self.symbol = currencyFromServer.symbol;
        }
    };

    models.TypeField = function (typeFromServer) {

        var self = this;

        if (typeFromServer == undefined) {
            self.alias = "";
            self.name = "";
            self.typeKey = "";
        } else {
            self.alias = typeFromServer.alias;
            self.name = typeFromServer.name;
            self.typeKey = typeFromServer.typeKey;
        }
    };

    models.StoreSettings = function (settingsFromServer) {

        var self = this;

        if (settingsFromServer == undefined) {
            self.currencyCode = "";
            self.nextOrderNumber = 0;
            self.nextInvoiceNumber = 0;
            self.dateFormat = "";
            self.timeFormat = "";
            self.globalShippable = false;
            self.globalTaxable = false;
            self.globalTrackInventory = false;
            self.globalShippingIsTaxable = false;
        }
        else {
            self.currencyCode = settingsFromServer.currencyCode;
            self.nextOrderNumber = parseInt(settingsFromServer.nextOrderNumber);
            self.nextInvoiceNumber = parseInt(settingsFromServer.nextInvoiceNumber);
            self.dateFormat = settingsFromServer.dateFormat;
            self.timeFormat = settingsFromServer.timeFormat;
            self.globalShippable = settingsFromServer.globalShippable;
            self.globalTaxable = settingsFromServer.globalTaxable;
            self.globalTrackInventory = settingsFromServer.globalTrackInventory;
            self.globalShippingIsTaxable = settingsFromServer.globalShippingIsTaxable;
        }
    };

}(window.merchello.Models = window.merchello.Models || {}));