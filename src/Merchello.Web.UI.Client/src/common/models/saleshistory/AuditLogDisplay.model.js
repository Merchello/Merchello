    /**
     * @ngdoc model
     * @name AuditLogDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's AuditLogDisplay object
     */
    var AuditLogDisplay = function() {
        var self = this;

        self.entityKey = '';
        self.entityTfKey = '';
        self.entityType = '';
        self.extendedData = {};
        self.isError = false;
        self.key = '';
        self.message = {};
        self.recordDate = '';
        self.verbosity = '';
    };

    AuditLogDisplay.prototype = (function() {

        function toDateString() {
            return this.recordDate.split('T')[0];
        }

        function toTimeString() {
            var time = this.recordDate.split('T')[1];
            return time.split(':')[0] + ':' + time.split(':')[1];
        }

        return {
            toDateString: toDateString,
            toTimeString: toTimeString
        };

    }());

    angular.module('merchello.models').constant('AuditLogDisplay', AuditLogDisplay);