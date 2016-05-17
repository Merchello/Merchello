    /**
     * @ngdoc model
     * @name NotificationMessageDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's NotificationMessageDisplay object
     */
    var NotificationMessageDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.description = '';
        self.fromAddress = '';
        self.replyTo = '';
        self.bodyText = '';
        self.maxLength = '';
        self.bodyTextIsFilePath = false;
        self.monitorKey = '';
        self.methodKey = '';
        self.recipients = '';
        self.sendToCustomer = true;
        self.disabled = false;
    };

    angular.module('merchello.models').constant('NotificationMessageDisplay', NotificationMessageDisplay);