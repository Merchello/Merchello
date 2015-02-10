    /**
     * @ngdoc models
     * @name catalogInventoryDisplayBuilder
     *
     * @description
     * A utility service that builds ProductAttributeDisplay models
     */
    angular.module('merchello.models').factory('catalogInventoryDisplayBuilder',
        ['genericModelBuilder', 'CatalogInventoryDisplay',
        function(genericModelBuilder, CatalogInventoryDisplay) {

            var Constructor = CatalogInventoryDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };

    }]);
