    /**
     * @ngdoc model
     * @name DeleteNotificationMessageDialogData
     * @function
     *
     * @description
     * Represents a JS version of Merchello's DeleteNotificationMessageDialogData object
     */
    var DeleteNotificationMessageDialogData = function() {
        var self = this;
        self.notificationMessage = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteNotificationMessageDialogData', DeleteNotificationMessageDialogData);