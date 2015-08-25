/**
 * @ngdoc service
 * @name umbContentTypeDisplayBuilder
 *
 * @description
 * A utility service that builds UmbContentTypeDisplay models
 */
angular.module('merchello.models').factory('umbContentTypeDisplayBuilder',
    ['genericModelBuilder', 'UmbContentTypeDisplay',
    function(genericModelBuilder, UmbContentTypeDisplay) {

        var Constructor = UmbContentTypeDisplay;

        return {
            createDefault: function() {
                return new Constructor();
            },
            transform: function(jsonResult) {
                return genericModelBuilder.transform(jsonResult, Constructor);
            }
        };

}]);
