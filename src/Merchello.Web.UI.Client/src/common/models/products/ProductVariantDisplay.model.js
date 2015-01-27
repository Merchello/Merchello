    /**
     * @ngdoc model
     * @name ProductAttributeDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProductVariantDisplay object
     */
    var ProductVariantDisplay = function() {
        var self = this;
        self.key = '';
        self.productKey = '';
        self.versionKey = '';
        self.name = '';
        self.sku = '';
        self.price = 0.00;
        self.costOfGoods = 0.00;
        self.salePrice = 0.00;
        self.onSale = false;
        self.manufacturer = '';
        self.manufacturerModelNumber = '';
        self.weight = 0;
        self.length = 0;
        self.width = 0;
        self.height = 0;
        self.barcode = '';
        self.available = true;
        self.trackInventory = false;
        self.outOfStockPurchase = false;
        self.taxable = false;
        self.shippable = false;
        self.download = false;
        self.downloadMediaId = -1;
        self.totalInventoryCount = 0;
        self.attributes = [];
        self.catalogInventories = [];
    };

    angular.module('merchello.models').constant('ProductVariantDisplay', ProductVariantDisplay);
