(function (models, undefined) {

    models.EmailTemplate = function (emailTemplateFromServer) {

        var self = this;

        if (emailTemplateFromServer == undefined) {
            self.pk = "";
            self.name = "";
            self.description = "";
            self.header = "";
            self.footer = "";
        } else {
            self.pk = emailTemplateFromServer.pk;
            self.name = emailTemplateFromServer.name;
            self.description = emailTemplateFromServer.description;
            self.header = emailTemplateFromServer.header;
            self.footer = emailTemplateFromServer.footer;
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