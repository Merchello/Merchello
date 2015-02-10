    /**
     * @ngdoc service
     * @name merchello.models.typeFieldDisplayBuilder
     *
     * @description
     * A utility service that builds TypeFieldDisplay models
     */
    angular.module('merchello.models')
    .factory('typeFieldDisplayBuilder',
    ['genericModelBuilder', 'TypeFieldDisplay',
        function(genericModelBuilder, TypeFieldDisplay) {

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
