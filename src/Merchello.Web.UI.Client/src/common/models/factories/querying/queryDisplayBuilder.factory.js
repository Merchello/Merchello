    /**
     * @ngdoc service
     * @name merchello.services.queryDisplayBuilder
     *
     * @description
     * A utility service that builds QueryDisplayModels models
     *
     */
    angular.module('merchello.models')
        .factory('queryDisplayBuilder',
        ['genericModelBuilder', 'QueryDisplay',
            function(genericModelBuilder, QueryDisplay) {
            var Constructor = QueryDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);

