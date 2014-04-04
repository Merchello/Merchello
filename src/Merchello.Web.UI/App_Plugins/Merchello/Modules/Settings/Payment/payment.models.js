(function (models, undefined) {

    models.PaymentMethod = function (paymentMethodFromServer) {

        var self = this;

        if (paymentMethodFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.providerKey = "";
            self.description = "";
            self.paymentCode = "";
        } else {
            self.key = paymentMethodFromServer.key;
            self.name = paymentMethodFromServer.name;
            self.providerKey = paymentMethodFromServer.providerKey;
            self.description = paymentMethodFromServer.description;
            self.paymentCode = paymentMethodFromServer.paymentCode;
        }

    };

    //models.PaymentGatewayProvider = function() {

    //};

}(window.merchello.Models = window.merchello.Models || {}));