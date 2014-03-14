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
                return new merchello.Models.Province(province)
            });
        };

    };

    models.Currency = function(currencyFromServer) {

        var self = this;

        if (currencyFromServer == undefined) {
            self.name = "";
            self.currencyCode = "";
            self.symbol = "";
        }
        else {
            self.name = currencyFromServer.name;
            self.currencyCode = currencyFromServer.currencyCode;
            self.symbol = currencyFromServer.symbol;
        }
    }

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