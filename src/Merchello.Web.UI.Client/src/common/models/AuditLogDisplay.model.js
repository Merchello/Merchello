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
        self.extendedData = [];
        self.isError = false;
        self.key = '';
        self.message = {};
        self.recordDate = '';
        self.verbosity = '';
    }

    angular.module('merchello.models').constant('AuditLogDisplay', AuditLogDisplay);