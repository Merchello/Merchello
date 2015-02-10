    /**
     * @ngdoc model
     * @name WarehouseCatalogDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's WarehouseCatalogDisplay object
     */
    var WarehouseCatalogDisplay = function() {
        var self = this;
        self.key = '';
        self.warehouseKey = '';
        self.name = '';
        self.description = '';
        self.isDefault = true;
    };

    angular.module('merchello.models').constant('WarehouseCatalogDisplay', WarehouseCatalogDisplay);