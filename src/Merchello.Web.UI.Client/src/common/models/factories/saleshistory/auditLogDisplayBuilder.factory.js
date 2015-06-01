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

                        // this checks to see if the message in the result is a JSON object
                        if (genericModelBuilder.isStringifyJson(jsonResult.message)) {
                            // if so, this is going to be something we can localize later (get from the lang files)
                            var message = JSON.parse(jsonResult.message);
                            auditLogDisplay.message = salesHistoryMessageDisplayBuilder.transform(message);
                        } else {
                            // otherwise we assume the developer simply put a note into the audit logs and thus
                            // we can't localize.
                            auditLogDisplay.message = salesHistoryMessageDisplayBuilder.createDefault();
                            auditLogDisplay.message.formattedMessage = jsonResult.message;
                        }
                        return auditLogDisplay;
                    }
                };
        }]);