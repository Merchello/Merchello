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

    DailyAuditLogDisplay.prototype = (function() {

        //// TODO this is not working as expected so we are keeping it internal
        function dayToDate() {
            //// 0: YYYY
            //// 1: MM
            //// 2: DD
            var dateParts = toDateString.call(this).split('-');
            var timeParts = toTimeString.call(this).split(':');
            return Date.parse(dateParts[0], dateParts[1] - 1, dateParts[2], timeParts[0], timeParts[1], 0, 0);
        }

        function toDateString() {
            return this.day.split('T')[0];
        }

        function toTimeString() {
            var time = this.day.split('T')[1];
            return time.split(':')[0] + ':' + time.split(':')[1];
        }

        return {
            toDateString: toDateString,
            toTimeString: toTimeString
        };

    }());

    angular.module('merchello.models').constant('DailyAuditLogDisplay', DailyAuditLogDisplay);