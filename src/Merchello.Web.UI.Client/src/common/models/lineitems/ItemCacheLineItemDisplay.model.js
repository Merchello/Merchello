    /**
     * @ngdoc model
     * @name ItemCacheLineItemDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ItemCacheLineItemDisplay object
     */
    var ItemCacheLineItemDisplay = function() {
        var self = this;

        self.key = '';
        self.containerKey = '';
        self.lineItemTfKey = '';
        self.lineItemType = '';
        self.lineItemTypeField = {};
        self.sku = '';
        self.name = '';
        self.quantity = '';
        self.price = '';
        self.exported = false;
        self.extendedData = {};
    };

    angular.module('merchello.models').constant('ItemCacheLineItemDisplay', ItemCacheLineItemDisplay);