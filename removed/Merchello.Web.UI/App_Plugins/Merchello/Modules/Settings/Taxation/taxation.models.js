(function (models, undefined) {

    models.TaxCountry = function (data) {
        
        var self = this;

        self.method = new merchello.Models.TaxMethod();    // Only used for binding, other data from a gateway resource
        if (data == undefined) {
            self.name = "";
            self.serviceCode = "";
            self.sortHelper = "0";
        } else {
            self.name = data.name;
            self.serviceCode = data.serviceCode;
            self.method.countryCode = data.serviceCode;
            self.sortHelper = _.isEqual(self.name, "Everywhere Else") ? "1" + self.name : "0" + self.name;
        }
        self.countryName = "";
        self.country = {};

        self.setCountryName = function(name) {
            self.countryName = name;
            self.sortHelper = _.isEqual(self.countryName, "Everywhere Else") ? "1" + self.countryName : "0" + self.countryName;
        };
    };

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

        self.hasProvinces = false;
        self.provider = new merchello.Models.GatewayProvider({ key: "-1", name: "Not Taxed", description: "" });
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

            if (!_.isEmpty(self.provinces)) {
                self.hasProvinces = true;
            }
            self.dialogEditorView = new merchello.Models.DialogEditorView(taxMethodFromServer.dialogEditorView);
        };

        self.displayEditor = function () {
            return self.dialogEditorView.editorView;
        };

    };

}(window.merchello.Models = window.merchello.Models || {}));