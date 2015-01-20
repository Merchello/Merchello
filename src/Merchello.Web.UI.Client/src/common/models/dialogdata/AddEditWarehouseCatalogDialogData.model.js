    /**
     * @ngdoc model
     * @name AddEditWarehouseCatalogDialogData
     * @function
     *
     * @description
     * Represents a JS version of Merchello's AddEditWarehouseCatalogDialogData object
     */
    var AddEditWarehouseCatalogDialogData = function() {
        var self = this;
        self.warehouse = {};
        self.warehouseCatalog = {};
    };

    angular.module('merchello.models').constant('AddEditWarehouseCatalogDialogData', AddEditWarehouseCatalogDialogData);