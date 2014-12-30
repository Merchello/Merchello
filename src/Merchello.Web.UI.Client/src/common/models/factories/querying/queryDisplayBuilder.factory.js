    /**
     * @ngdoc service
     * @name merchello.services.queryDisplayBuilder
     *
     * @description
     * A utility service that builds QueryDisplayModels models
     */
    angular.module('merchello.models')
        .factory('queryDisplayBuilder',
        [function() {
            var Constructor = QueryDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                }
            };
        }]);

