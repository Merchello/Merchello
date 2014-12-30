    /**
     * @ngdoc service
     * @name merchello.services.typeFieldDisplayBuilder
     *
     * @description
     * A utility service that builds TypeFieldDisplay models
     */
    angular.module('merchello.models')
    .factory('typeFieldDisplayBuilder',
    ['genericModelBuilder',
        function(genericModelBuilder) {

            var Constructor = TypeFieldDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);
