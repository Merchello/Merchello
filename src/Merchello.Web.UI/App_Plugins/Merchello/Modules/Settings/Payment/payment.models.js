(function (models, undefined) {

    models.PaymentMethod = function (paymentMethodFromServer) {

        var self = this;

        if (paymentMethodFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.providerKey = "";
            self.description = "";
            self.paymentCode = "";
            self.dialogEditorView = "";
        } else {
            self.key = paymentMethodFromServer.key;
            self.name = paymentMethodFromServer.name;
            self.providerKey = paymentMethodFromServer.providerKey;
            self.description = paymentMethodFromServer.description;
            self.paymentCode = paymentMethodFromServer.paymentCode;
            self.dialogEditorView = new merchello.Models.DialogEditorView(paymentMethodFromServer.dialogEditorView);
        }

        self.displayEditor = function () {
            return self.dialogEditorView.editorView;
        };

    };

    //models.PaymentGatewayProvider = function() {

    //};

}(window.merchello.Models = window.merchello.Models || {}));