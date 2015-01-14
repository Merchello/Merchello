    /**
     * @ngdoc service
     * @name merchello.models.warehouseCatalogDisplayBuilder
     *
     * @description
     * A utility service that builds WarehouseCatalogDisplay models
     */
    angular.module('merchello.models')
        .factory('warehouseCatalogDisplayBuilder',
        ['genericModelBuilder', 'WarehouseCatalogDisplay',
            function(genericModelBuilder, WarehouseCatalogDisplay) {

                var Constructor = WarehouseCatalogDisplay;
                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };

            }]);

