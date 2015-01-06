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

    models.PaymentGatewayProvider = function (gatewayProviderFromServer) {

        var self = this;

        if (gatewayProviderFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.providerTfKey = "";
            self.description = "";
            self.extendedData = [];
            self.encryptExtendedData = false;
            self.activated = false;
            self.dialogEditorView = "";
        } else {
            self.key = gatewayProviderFromServer.key;
            self.name = gatewayProviderFromServer.name;
            self.providerTfKey = gatewayProviderFromServer.providerTfKey;
            self.description = gatewayProviderFromServer.description;
            self.extendedData = gatewayProviderFromServer.extendedData;
            self.encryptExtendedData = gatewayProviderFromServer.encryptExtendedData;
            self.activated = gatewayProviderFromServer.activated;
            self.dialogEditorView = new merchello.Models.DialogEditorView(gatewayProviderFromServer.dialogEditorView);
        }
        self.resources = [];
        self.methods = [];

        self.displayEditor = function () {
            return self.activated && self.dialogEditorView.editorView;
        };
    };

}(window.merchello.Models = window.merchello.Models || {}));