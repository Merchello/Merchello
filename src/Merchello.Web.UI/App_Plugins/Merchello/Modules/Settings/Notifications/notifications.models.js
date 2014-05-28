(function (models, undefined) {
    models.NotificationMethod = function (notificationMethodFromServer) {

        var self = this;

        if (notificationMethodFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.providerKey = "";
            self.description = "";
            self.serviceCode = "";
            self.notificationMessages = [];
        } else {
            self.key = notificationMethodFromServer.key;
            self.name = notificationMethodFromServer.name;
            self.providerKey = notificationMethodFromServer.providerKey;
            self.description = notificationMethodFromServer.description;
            self.serviceCode = notificationMethodFromServer.serviceCode;
            self.notificationMessages = notificationMethodFromServer.notificationMessages;
        }

        self.displayEditor = function () {
            return self.dialogEditorView.editorView;
        };

    };

    models.NotificationMessage = function (emailTemplateFromServer) {

        var self = this;

        if (emailTemplateFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.description = "";
            self.fromAddress = "";
            self.replyTo = "";
            self.bodyText = "";
            self.maxLength = "";
            self.bodyTextIsFilePath = "";
            self.monitorKey = "";
            self.methodKey = "";
            self.recipients = "";
            self.sendToCustomer = "";
            self.disabled = "";
        } else {
            self.key = emailTemplateFromServer.key;
            self.name = emailTemplateFromServer.name;
            self.description = emailTemplateFromServer.description;
            self.fromAddress = emailTemplateFromServer.fromAddress;
            self.replyTo = emailTemplateFromServer.replyTo;
            self.bodyText = emailTemplateFromServer.bodyText;
            self.maxLength = emailTemplateFromServer.maxLength;
            self.bodyTextIsFilePath = emailTemplateFromServer.bodyTextIsFilePath;
            self.monitorKey = emailTemplateFromServer.monitorKey;
            self.methodKey = emailTemplateFromServer.methodKey;
            self.recipients = emailTemplateFromServer.recipients;
            self.sendToCustomer = emailTemplateFromServer.sendToCustomer;
            self.disabled = emailTemplateFromServer.disabled;
        }

    };

    models.NotificationGatewayProvider = function (gatewayProviderFromServer) {

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

    models.NotificationSubscriber = function(notificationSubscriberFromServer) {

        var self = this;

        if (notificationSubscriberFromServer == undefined) {
            self.pk = "";
            self.email = "";
        } else {
            self.pk = notificationSubscriberFromServer.pk;
            self.email = notificationSubscriberFromServer.email;
        }
    };

}(window.merchello.Models = window.merchello.Models || {}));