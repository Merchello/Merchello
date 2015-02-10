    /**
     * @ngdoc service
     * @name merchello.models.salesHistoryMessageDisplayBuilder
     *
     * @description
     * A utility service that builds salesHistoryMessageDisplayBuilder models
     */
    angular.module('merchello.models')
        .factory('salesHistoryMessageDisplayBuilder',
        ['genericModelBuilder', 'SalesHistoryMessageDisplay',
            function(genericModelBuilder, SalesHistoryMessageDisplay) {

                var Constructor = SalesHistoryMessageDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);
