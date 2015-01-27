    /**
     * @ngdoc model
     * @name ProductAttributeDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProductDisplay object
     */
    var ProductDisplay = function() {
        var self = this;
        self.key = '';
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
        self.barcode = "";
        self.available = true;
        self.trackInventory = false;
        self.outOfStockPurchase = false;
        self.taxable = false;
        self.shippable = false;
        self.download = false;
        self.downloadMediaId = -1;
        self.hasOptions = false;
        self.hasVariants = false;
        self.productOptions = [];
        self.productVariants = [];
        self.catalogInventories = [];
    };

    ProductDisplay.prototype = (function() {

        // returns a value indicating whether or not the product has variants
        function hasVariants() {
            return this.productVariants.length > 0;
        }

        function totalInventory() {

        }

        return {
            hasVariants: hasVariants
        };
    }());

    angular.module('merchello.models').constant('ProductDisplay', ProductDisplay);