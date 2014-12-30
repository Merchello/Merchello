    /**
     * @ngdoc service
     * @name merchello.services.queryDisplayBuilder
     *
     * @description
     * A utility service that builds QueryDisplayModels models
     *
     * @note
     * This does not implement a transform method since there is no API end point.  This is
     * a client side generated model used to construct a query in for the ApiController.
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

