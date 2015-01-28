    /**
     * @ngdoc model
     * @name CatalogInventoryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CatalogInventoryDisplay object
     */
    var CatalogInventoryDisplay = function() {
        var self = this;
        self.productVariantKey = '';
        self.catalogKey = '';
        self.count = 0;
        self.lowCount = 0;
        self.location = '';
        self.update = new Date();
        self.active = true;
    };

    angular.module('merchello.models').constant('CatalogInventoryDisplay', CatalogInventoryDisplay);