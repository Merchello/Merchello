    /**
     * @ngdoc service
     * @name merchello.models.salesHistoryDisplayBuilder
     *
     * @description
     * A utility service that builds salesHistoryDisplayBuilder models
     */
    angular.module('merchello.models')
        .factory('salesHistoryDisplayBuilder',
            ['genericModelBuilder', 'dailyAuditLogDisplayBuilder', 'SalesHistoryDisplay',
            function(genericModelBuilder, dailyAuditLogDisplayBuilder, SalesHistoryDisplay) {

                var Constructor = SalesHistoryDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var history = this.createDefault();
                        angular.forEach(jsonResult.dailyLogs, function(result) {
                            history.addDailyLog(dailyAuditLogDisplayBuilder.transform(result));
                        });
                        return history;
                    }
                };
        }]);