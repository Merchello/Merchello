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
        self.productVariantKey = '';
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
        self.barcode = "";
        self.available = true;
        self.trackInventory = false;
        self.outOfStockPurchase = false;
        self.taxable = false;
        self.shippable = false;
        self.download = false;
        self.downloadMediaId = -1;
        self.productOptions = [];
        self.productVariants = [];
        self.catalogInventories = [];
    };

    ProductDisplay.prototype = (function() {

        // returns a value indicating whether or not the product has variants
        function hasVariants() {
            return this.productVariants.length > 0;
        }

        // gets the master variant that represents a product without variants
        function getMasterVariant() {
            var variant = new ProductVariantDisplay();
            angular.extend(variant, this);
            // clean up
            variant.key = this.productVariantKey;
            variant.productKey = this.key;
            delete variant['productOptions'];
            delete variant['productVariants'];
            return variant;
        }

        // returns a count of total inventory. if product has variants sums all inventory otherwise uses
        // the product inventory count
        function totalInventory() {
            var inventoryCount = 0;
            if (hasVariants.call(this)) {
                angular.forEach(this.productVariants, function(pv) {
                    angular.forEach(pv.catalogInventories, function(ci) {
                        inventoryCount += ci.count;
                    });
                });
            } else {
                angular.forEach(this.catalogInventories, function(ci) {
                  inventoryCount += ci.count;
                });
            }
            return inventoryCount;
        }

        return {
            hasVariants: hasVariants,
            totalInventory: totalInventory,
            getMasterVariant: getMasterVariant
        };
    }());

    angular.module('merchello.models').constant('ProductDisplay', ProductDisplay);