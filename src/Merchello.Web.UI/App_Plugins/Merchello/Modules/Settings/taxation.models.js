(function (models, undefined) {

    models.TaxRate = function (taxRateFromServer) {

        var self = this;

        if (taxRateFromServer == undefined) {
            self.pk = "";
            self.country = "";
            self.rate = "";
        } else {
            self.pk = taxRateFromServer.pk;
            self.country = taxRateFromServer.country;
            self.rate = taxRateFromServer.rate;
        };

    };

}(window.merchello.Models = window.merchello.Models || {}));