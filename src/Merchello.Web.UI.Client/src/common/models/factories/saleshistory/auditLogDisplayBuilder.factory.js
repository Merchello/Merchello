    /**
     * @ngdoc service
     * @name merchello.models.auditLogDisplayBuilder
     *
     * @description
     * A utility service that builds auditLogDisplayBuilder models
     */
    angular.module('merchello.models')
        .factory('auditLogDisplayBuilder',
            ['genericModelBuilder', 'salesHistoryMessageDisplayBuilder', 'extendedDataDisplayBuilder', 'AuditLogDisplay',
            function(genericModelBuilder, salesHistoryMessageDisplayBuilder, extendedDataDisplayBuilder, AuditLogDisplay) {

                var Constructor = AuditLogDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var auditLogDisplay = genericModelBuilder.transform(jsonResult, Constructor);
                        auditLogDisplay.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);

                        // this is a bit brittle - and we should look at the construction of this in the ApiController
                        var message = JSON.parse(jsonResult.message);
                        auditLogDisplay.message = salesHistoryMessageDisplayBuilder.transform(message);
                        return auditLogDisplay;
                    }
                };
        }]);