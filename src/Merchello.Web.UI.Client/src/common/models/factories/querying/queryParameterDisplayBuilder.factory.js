    /**
     * @ngdoc service
     * @name merchello.services.queryParameterDisplayBuilder
     *
     * @description
     * A utility service that builds QueryParameterDisplayModels models
     *
     */
    angular.module('merchello.models')
        .factory('queryParameterDisplayBuilder',
            ['genericModelBuilder', 'QueryParameterDisplay',
            function(genericModelBuilder, QueryParameterDisplay) {
            var Constructor = QueryParameterDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);
