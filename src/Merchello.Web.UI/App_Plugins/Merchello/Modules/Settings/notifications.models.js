(function (models, undefined) {

    models.EmailTemplate = function (emailTemplateFromServer) {

        var self = this;

        if (emailTemplateFromServer == undefined) {
            self.pk = "",
            self.name = "",
            self.description = "",
            self.header = "",
            self.footer = ""
        } else {
            self.pk = emailTemplateFromServer.pk,
            self.name = emailTemplateFromServer.name,
            self.description = emailTemplateFromServer.description,
            self.header = emailTemplateFromServer.header,
            self.footer = emailTemplateFromServer.footer
        }

    };

}(window.merchello.Models = window.merchello.Models || {}));