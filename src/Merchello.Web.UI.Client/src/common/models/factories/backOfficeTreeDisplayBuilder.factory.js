/**
 * @ngdoc service
 * @name backOfficeTreeDisplayBuilder
 *
 * @description
 * A utility service that builds backOfficeTreeDisplay models
 */
angular.module('merchello.models').factory('backOfficeTreeDisplayBuilder',
    ['genericModelBuilder', 'BackOfficeTreeDisplay',
    function(genericModelBuilder, BackOfficeTreeDisplay) {
        var Constructor = BackOfficeTreeDisplay;

        return {
            createDefault: function() {
                return new Constructor();
            },
            transform: function(jsonResult) {
                return genericModelBuilder.transform(jsonResult, Constructor);
            }
        };
    }]);
