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
        //self.master = true;
        self.downloadMediaId = -1;
        self.productOptions = [];
        self.productVariants = [];
        self.catalogInventories = [];
        self.detachedContents = [];
    };

    ProductDisplay.prototype = (function() {

        // returns a product variant with the associated key
        function getProductVariant(productVariantKey) {
            return _.find(this.productVariants, function(v) { return v.key === productVariantKey; });
        }

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
            variant.master = true;
            delete variant['productOptions'];
            delete variant['productVariants'];
            return variant;
        }

        // returns a count of total inventory. if product has variants sums all inventory otherwise uses
        // the product inventory count
        function totalInventory() {
            var inventoryCount = 0;
            if (hasVariants.call(this)) {
                var anyTracksInventory = false;
                angular.forEach(this.productVariants, function(pv) {
                    if(pv.trackInventory) {
                        anyTracksInventory = true;
                        angular.forEach(pv.catalogInventories, function(ci) {
                            inventoryCount += ci.count;
                        });
                    }
                });
                if(!anyTracksInventory) {
                    inventoryCount = "n/a";
                }
            } else {
                if(this.trackInventory) {
                    angular.forEach(this.catalogInventories, function(ci) {
                        inventoryCount += ci.count;
                    });
                } else {
                    inventoryCount = "n/a";
                }
            }
            return inventoryCount;
        }

        // adds an empty options
        function addEmptyOption() {
            var option = new ProductOptionDisplay();
            this.productOptions.push(option);
        }

        // removes an option
        function removeOption(option) {
            this.productOptions = _.reject(this.productOptions, function(opt) { return _.isEqual(opt, option); });
        }


        // finds the minimum variant price or sales price
        function variantsMinimumPrice(salePrice) {
            if (this.productVariants.length > 0) {
                if (salePrice === undefined) {
                    return _.min(this.productVariants, function(v) { return v.price; }).price;
                } else {
                    var onSaleVariants = _.filter(this.productVariants, function(osv) { return osv.onSale; });
                    if(onSaleVariants.length > 0) {
                        salePrice = _.min(onSaleVariants,
                            function(v) { return v.salePrice; }
                        ).salePrice;
                        return salePrice;
                    } else {
                        return 0;
                    }
                }
            } else {
                return 0;
            }
        }

        // finds the maximum variant price or sales price
        function variantsMaximumPrice(salePrice) {
            if (this.productVariants.length > 0) {
                if(salePrice === undefined) {
                    return _.max(this.productVariants, function(v) { return v.price; }).price;
                } else {
                    var onSaleVariants = _.filter(this.productVariants, function(osv) { return osv.onSale; });
                    if(onSaleVariants.length > 0) {
                        return _.max(
                            onSaleVariants,
                            function (v) {
                                return v.salePrice;
                            }
                        ).salePrice;
                    } else {
                        return 0;
                    }
                }
            } else {
                return 0;
            }
        }

        // returns a value indicating whether or not any variants are on sale
        function anyVariantsOnSale() {
            var variant = _.find(this.productVariants, function(v) { return v.onSale; });
            return variant === undefined ? false : true;
        }

        // returns a collection of shippable variants
        function shippableVariants() {
            return _.filter(this.productVariants, function(v) { return v.shippable; });
        }

        // returns a collection of taxable variants
        function taxableVariants() {
            return _.filter(this.productVariants, function(v) { return v.taxable; });
        }

        // returns a value indicating whether or not the product has a detached content that can be rendered.
        function canBeRendered() {
            if (!this.available) {
                return false;
            }
            if (this.detachedContents.length === 0) {
                return false;
            }
            if (_.filter(this.detachedContents, function(dc) { return dc.canBeRendered; }).length === 0) {
                return false;
            }
            return true;
        }

        // ensures a catalog is selected if the variant is marked shippable
        function ensureCatalogInventory(defaultCatalogKey) {
            if (!this.shippable && !this.trackInventory) {
                return;
            }
            // if this product is not associated with any catalogs we need to add the default catalog
            // so that we can associate shipping information
            if (this.catalogInventories.length === 0) {
                var inv = new CatalogInventoryDisplay();
                inv.productVariantKey = this.key;
                inv.catalogKey = defaultCatalogKey;
                inv.active = true;
                this.catalogInventories.push(inv);
            } else {
                // if there are catalogs and none are selected we need to force the default catalog to be selected.
                var activeInventories = _.filter(this.catalogInventories, function (ci) {
                    return ci.active;
                });
                if (activeInventories.length === 0) {
                    var defaultInv = _.find(this.catalogInventories, function (dci) {
                        return dci.catalogKey === defaultCatalogKey;
                    });
                    if (defaultInv !== undefined) {
                        defaultInv.active = true;
                    }
                }
            }
        }

        function prepForSave() {
            angular.forEach(this.detachedContents, function(dc) {
                dc.detachedDataValues = dc.detachedDataValues.asDetachedValueArray();
            });

            angular.forEach(this.productVariants, function(pv) {
                if (pv.detachedContents.length > 0) {
                    angular.forEach(pv.detachedContents, function(pvdc) {
                        pvdc.detachedDataValues = pvdc.detachedDataValues.toArray();
                    });
                }

                angular.forEach(pv.attributes, function(a) {
                    a.detachedDataValues = a.detachedDataValues.toArray();
                });
            });

            angular.forEach(this.productOptions, function(po) {
                angular.forEach(po.choices, function(c) {
                    c.detachedDataValues = c.detachedDataValues.toArray();
                });
            });
        }

        return {
            hasVariants: hasVariants,
            totalInventory: totalInventory,
            getMasterVariant: getMasterVariant,
            addEmptyOption: addEmptyOption,
            removeOption: removeOption,
            variantsMinimumPrice: variantsMinimumPrice,
            variantsMaximumPrice: variantsMaximumPrice,
            anyVariantsOnSale: anyVariantsOnSale,
            shippableVariants: shippableVariants,
            getProductVariant: getProductVariant,
            taxableVariants: taxableVariants,
            canBeRendered: canBeRendered,
            ensureCatalogInventory: ensureCatalogInventory,
            prepForSave: prepForSave
        };
    }());

    angular.module('merchello.models').constant('ProductDisplay', ProductDisplay);