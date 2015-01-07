    /**
     * @ngdoc model
     * @name DailyAuditLogDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's DailyAuditLogDisplay object
     */
    var DailyAuditLogDisplay = function() {
        var self = this;
        this.day = '';
        this.logs = [];
    };

    angular.module('merchello.models').constant('DailyAuditLogDisplay', DailyAuditLogDisplay);