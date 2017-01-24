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
        self.master = false;
        self.downloadMediaId = -1;
        self.totalInventoryCount = 0;
        self.attributes = [];
        self.catalogInventories = [];
        self.detachedContents = [];
    };

    ProductVariantDisplay.prototype = (function() {

        function getProductForMasterVariant() {
            var product = new ProductDisplay();
            product = angular.extend(product, this);
            // do some corrections
            var pvk = product.key;
            product.key = product.productKey;
            product.productVariantKey = pvk;
            delete product['productKey'];
            delete product['attributes'];
            // remove catalog inventories that are not active
            product.catalogInventories = _.reject(product.catalogInventories, function (ci) {
                return ci.active === false;
            });
            return product;
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

        // removes inactive catalog inventories from a variant before save
        function removeInActiveInventories() {
            this.catalogInventories = _.reject(this.catalogInventories, function (ci) {
                return ci.active === false;
            });
        }

        // updates all inventory counts to the count passed as a parameter
        function setAllInventoryCount(count) {
            angular.forEach(this.catalogInventories, function (ci) {
                ci.count = count;
            });
        }

        // updates all inventory low count to the low count passed as a parameter
        function setAllInventoryLowCount(lowCount) {
            angular.forEach(this.catalogInventories, function (ci) {
                ci.lowCount = lowCount;
            });
        }

        // TODO something like this could be used to copy productOptions from one product to another.
        // TODO: this method is not used
        function deepClone() {
            var variant = new ProductVariantDisplay();
            variant = angular.extend(variant, this);
            variant.attributes = [];
            angular.forEach(this.attributes, function (att) {
                var attribute = new ProductAttributeDisplay();
                angular.extend(attribute, att);
                variant.attributes.push(attribute);
            });
            variant.catalogInventories = [];
            angular.forEach(this.catalogInventories, function (ci) {
                var inv = new CatalogInventoryDisplay();
                angular.extend(inv, ci);
                variant.catalogInventories.push(ci);
            });
            return variant;
        }

        function hasDetachedContent() {
            return this.detachedContents.length > 0;
        }

        function getDetachedContent(isoCode) {
            if (!this.hasDetachedContent.call(this)) {
                return null;
            } else {
                return _.find(this.detachedContents, function(dc) {
                  return dc.cultureName === isoCode;
                });
            }
        }

        function detachedContentType() {
            if (!this.hasDetachedContent.call(this)) {
                return null;
            } else {
                return this.detachedContents[0].detachedContentType;
            }
        }

        function assertLanguageContent(isoCodes) {
            var self = this;
            var missing = [];
            var removers = [];
            _.each(self.detachedContents, function(dc) {
              var found = _.find(isoCodes, function(code) {
                return code === dc.cultureName;
                });
                if (found === undefined) {
                    removers.push(dc.cultureName);
                }
            });

            angular.forEach(removers, function(ic) {
                this.detachedContents = _.reject(self.detachedContents, function(oust) {
                    return oust.cultureName === ic;
                });
            });

            missing = _.filter(isoCodes, function(check) {
                var fnd = _.find(self.detachedContents, function(dc) {
                  return dc.cultureName === check;
                });
                return fnd === undefined;
            });
            return missing;
        }


        function prepForSave() {
            angular.forEach(this.detachedContents, function(dc) {
                dc.detachedDataValues = dc.detachedDataValues.asDetachedValueArray();
            });

            angular.forEach(this.attributes, function(a) {
                if(!angular.isArray(a.detachedDataValues)) {
                 a.detachedDataValues = a.detachedDataValues.toArray();
                }
            });
        }

        return {
            getProductForMasterVariant: getProductForMasterVariant,
            ensureCatalogInventory: ensureCatalogInventory,
            removeInActiveInventories: removeInActiveInventories,
            setAllInventoryCount: setAllInventoryCount,
            setAllInventoryLowCount: setAllInventoryLowCount,
            hasDetachedContent: hasDetachedContent,
            assertLanguageContent: assertLanguageContent,
            detachedContentType: detachedContentType,
            getDetachedContent: getDetachedContent,
            prepForSave: prepForSave
        };
    }());

    angular.module('merchello.models').constant('ProductVariantDisplay', ProductVariantDisplay);
