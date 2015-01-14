    /**
     * @ngdoc service
     * @name merchello.models.warehouseDisplayBuilder
     *
     * @description
     * A utility service that builds WarehouseDisplay models
     */
    angular.module('merchello.models')
        .factory('warehouseDisplayBuilder',
        ['genericModelBuilder', 'warehouseCatalogDisplayBuilder', 'WarehouseDisplay',
        function(genericModelBuilder, warehouseCatalogDisplayBuilder, WarehouseDisplay) {

            var Constructor = WarehouseDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var warehouse = genericModelBuilder.transform(jsonResult, Constructor);
                    warehouse.warehouseCatalogs = warehouseCatalogDisplayBuilder.transform(jsonResult.warehouseCatalogs);
                    return warehouse;
                }
            };

    }]);
