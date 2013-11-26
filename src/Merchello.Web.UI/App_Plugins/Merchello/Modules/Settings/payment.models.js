(function (models, undefined) {

    models.ManualPaymentMethod = function (manualPaymentMethodFromServer) {

        var self = this;

        if (manualPaymentMethodFromServer == undefined) {
            self.pk = "";
            self.name = "";
            self.description = "";
        } else {
            self.pk = manualPaymentMethodFromServer.pk;
            self.name = manualPaymentMethodFromServer.name;
            self.description = manualPaymentMethodFromServer.description;
        };

    };

}(window.merchello.Models = window.merchello.Models || {}));