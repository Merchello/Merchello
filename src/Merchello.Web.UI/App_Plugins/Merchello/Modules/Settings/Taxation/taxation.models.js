(function (models, undefined) {

    models.TaxProvince = function (provinceDataFromServer) {

        var self = this;

        if (provinceDataFromServer == undefined) {
            self.name = "";
            self.code = "";
            self.percentAdjustment = "";
        } else {
            self.name = provinceDataFromServer.name;
            self.code = provinceDataFromServer.code;
            self.percentAdjustment = provinceDataFromServer.percentAdjustment;
        }

    };

    models.TaxMethod = function (taxMethodFromServer) {

        var self = this;

        if (taxMethodFromServer == undefined) {
            self.key = "";
            self.providerKey = "";
            self.name = "";
            self.countryCode = "";
            self.percentageTaxRate = 0.0;
            self.provinces = [];
        } else {
            self.key = taxMethodFromServer.key;
            self.providerKey = taxMethodFromServer.providerKey;
            self.name = taxMethodFromServer.name;
            self.countryCode = taxMethodFromServer.countryCode;
            self.percentageTaxRate = taxMethodFromServer.percentageTaxRate;
            self.provinces = _.map(taxMethodFromServer.provinces, function (province) {
                return new merchello.Models.TaxProvince(province);
            });
        };

    };

}(window.merchello.Models = window.merchello.Models || {}));