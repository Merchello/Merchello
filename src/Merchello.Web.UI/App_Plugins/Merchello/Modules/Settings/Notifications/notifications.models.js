(function (models, undefined) {
    models.NotificationMethod = function (notificationMethodFromServer) {

        var self = this;

        if (notificationMethodFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.providerKey = "";
            self.description = "";
            self.serviceCode = "";
        } else {
            self.key = notificationMethodFromServer.key;
            self.name = notificationMethodFromServer.name;
            self.providerKey = notificationMethodFromServer.providerKey;
            self.description = notificationMethodFromServer.description;
            self.serviceCode = notificationMethodFromServer.serviceCode;
        }

        self.displayEditor = function () {
            return self.dialogEditorView.editorView;
        };

    };

    models.NotificationMessage = function (emailTemplateFromServer) {

        var self = this;

        if (emailTemplateFromServer == undefined) {
            self.Key = "";
            self.Name = "";
            self.Description = "";
            self.FromAddress = "";
            self.ReplyTo = "";
            self.BodyText = "";
            self.MaxLength = "";
            self.BodyTextIsFilePath = "";
            self.TriggerKey = "";
            self.MethodKey = "";
            self.Recipients = "";
            self.SendToCustomer = "";
            self.Disabled = "";
        } else {
            self.Key = emailTemplateFromServer.key;
            self.Name = emailTemplateFromServer.name;
            self.Description = emailTemplateFromServer.description;
            self.FromAddress = emailTemplateFromServer.fromAddress;
            self.ReplyTo = emailTemplateFromServer.replyTo;
            self.BodyText = emailTemplateFromServer.bodyText;
            self.MaxLength = emailTemplateFromServer.maxLength;
            self.BodyTextIsFilePath = emailTemplateFromServer.bodyTextIsFilePath;
            self.TriggerKey = emailTemplateFromServer.triggerKey;
            self.MethodKey = emailTemplateFromServer.methodKey;
            self.Recipients = emailTemplateFromServer.recipients;
            self.SendToCustomer = emailTemplateFromServer.sendToCustomer;
            self.Disabled = emailTemplateFromServer.disabled;
        }

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