    /**
     * @ngdoc model
     * @name ChangeWarehouseCatalogDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for changes warehouse catalogs.
     */
    var ChangeWarehouseCatalogDialogData  = function() {
        var self = this;
        self.warehouse = {};
        self.selectedWarehouseCatalog = {};
    };

    angular.module('merchello.models').constant('ChangeWarehouseCatalogDialogData', ChangeWarehouseCatalogDialogData);