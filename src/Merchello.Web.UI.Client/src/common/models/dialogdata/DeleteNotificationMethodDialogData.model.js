    /**
     * @ngdoc model
     * @name DeleteNotificationMethodDialogData
     * @function
     *
     * @description
     * Represents a JS version of Merchello's DeleteNotificationMethodDialogData object
     */
    var DeleteNotificationMethodDialogData = function() {
        var self = this;
        self.notificationMethod = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteNotificationMethodDialogData', DeleteNotificationMethodDialogData);
