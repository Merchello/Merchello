    /**
     * @ngdoc models
     * @name productAttributeDisplayBuilder
     *
     * @description
     * A utility service that builds ProductAttributeDisplay models
     */
    angular.module('merchello.models').factory('productAttributeDisplayBuilder',
        ['genericModelBuilder', 'ProductAttributeDisplay',
        function(genericModelBuilder, ProductAttributeDisplay) {

            var Constructor = ProductAttributeDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
    }]);
