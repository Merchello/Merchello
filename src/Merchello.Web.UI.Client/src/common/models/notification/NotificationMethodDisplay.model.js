    /**
     * @ngdoc model
     * @name NotificationMethodDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's NotificationMethodDisplay object
     */
    var NotificationMethodDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.providerKey = '';
        self.description = '';
        self.serviceCode = '';
        self.notificationMessages = [];
        self.dialogEditorView = {};
    };

    angular.module('merchello.models').constant('NotificationMethodDisplay', NotificationMethodDisplay);