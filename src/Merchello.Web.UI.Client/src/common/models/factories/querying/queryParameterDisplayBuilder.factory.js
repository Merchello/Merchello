    /**
     * @ngdoc service
     * @name merchello.services.queryParameterDisplayBuilder
     *
     * @description
     * A utility service that builds QueryParameterDisplayModels models
     */
    angular.module('merchello.models')
        .factory('queryParameterDisplayBuilder',
        [function() {
            var Constructor = QueryParameterDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                }
            };
        }]);
