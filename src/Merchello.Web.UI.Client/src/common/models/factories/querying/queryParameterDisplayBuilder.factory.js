    /**
     * @ngdoc service
     * @name merchello.services.queryParameterDisplayBuilder
     *
     * @description
     * A utility service that builds QueryParameterDisplayModels models
     *
     * @note
     * This does not implement a transform method since there is no API end point.  This is
     * a client side generated model used to construct a parameter for a client side QueryDisplay model
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
