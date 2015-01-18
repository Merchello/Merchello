    /**
     * @ngdoc model
     * @name DeleteWarehouseCatalogDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for deleting a warehouse catalog.
     */
    var DeleteWarehouseCatalogDialogData = function() {
        var self = this;
        self.warehouseCatalog = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteWarehouseCatalogDialogData', DeleteWarehouseCatalogDialogData);
