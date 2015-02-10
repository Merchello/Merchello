    /**
     * @ngdoc model
     * @name AddEditNotificationMessageDialogData
     * @function
     *
     * @description
     * Represents a JS version of Merchello's AddEditNotificationMessageDialogData object
     */
    var AddEditNotificationMessageDialogData = function() {
        var self = this;
        self.notificationMessage = {};
        self.notificationMonitors = [];
        self.selectedMonitor = {};
    };

    angular.module('merchello.models').constant('AddEditNotificationMessageDialogData', AddEditNotificationMessageDialogData);