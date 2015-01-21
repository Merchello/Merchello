    /**
     * @ngdoc model
     * @name AddEditNotificationMethodDialogData
     * @function
     *
     * @description
     * Represents a JS version of Merchello's AddEditNotificationMethodDialogData object
     */
    var AddEditNotificationMethodDialogData = function() {
        var self = this;
        self.notificationMethod = {};
    };

    angular.module('merchello.models').constant('AddEditNotificationMethodDialogData', AddEditNotificationMethodDialogData);