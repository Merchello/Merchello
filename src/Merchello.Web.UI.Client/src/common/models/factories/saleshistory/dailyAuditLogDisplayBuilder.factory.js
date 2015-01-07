    /**
     * @ngdoc service
     * @name merchello.models.auditLogDisplayBuilder
     *
     * @description
     * A utility service that builds auditLogDisplayBuilder models
     */
    angular.module('merchello.models')
        .factory('dailyAuditLogDisplayBuilder',
            ['genericModelBuilder', 'auditLogDisplayBuilder', 'DailyAuditLogDisplay',
            function(genericModelBuilder, auditLogDisplayBuilder, DailyAuditLogDisplay) {

                var Constructor = DailyAuditLogDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var dailyLog = genericModelBuilder.transform(jsonResult, Constructor);
                        dailyLog.logs = auditLogDisplayBuilder.transform(jsonResult.logs);
                        return dailyLog;
                    }
                };
        }]);