(function (models, undefined) {

    models.emptyGuid = "00000000-0000-0000-0000-000000000000";


    models.ProductAttribute = function (productAttributeFromServer) {

        var self = this;

        if (productAttributeFromServer == undefined)
        {
            self.key = "";
            self.optionKey = "";
            self.name = "";
            self.sku = "";
            self.sortOrder = 0;
            self.optionOrder = 0;
            self.isRemoved = false;
        }
        else
        {
            self.key = productAttributeFromServer.key;
            self.optionKey = productAttributeFromServer.optionKey;
            self.name = productAttributeFromServer.name;
            self.sku = productAttributeFromServer.sku;
            self.sortOrder = productAttributeFromServer.sortOrder;
            self.optionOrder = self.sortOrder;
            self.isRemoved = false;
        }

        self.findMyOption = function (options) {
            return _.find(options, function (option) { return option.key == self.optionKey; });
        };

    };

    models.ProductOption = function (productOptionFromServer) {

        var self = this;

        if (productOptionFromServer == undefined)
        {
            self.key = "";
            self.name = "";
            self.required = "";
            self.sortOrder = 1;

            self.choices = [];
        }
        else
        {
            self.key = productOptionFromServer.key;
            self.name = productOptionFromServer.name;
            self.required = productOptionFromServer.required;
            self.sortOrder = productOptionFromServer.sortOrder;

            self.choices = _.map(productOptionFromServer.choices, function (attribute) {
                var attr = new merchello.Models.ProductAttribute(attribute);
                attr.optionOrder = self.sortOrder;
                return attr;
            });

            self.choices = _.sortBy(self.choices, function (choice) { return choice.sortOrder; });
        }

        // Helper to add a choice to this option
        self.addChoice = function (name) {

            var newChoice = new merchello.Models.ProductAttribute();
            newChoice.name = name;
            newChoice.sortOrder = self.choices.length + 1;
            //newChoice.sku = newChoice.sortOrder;
            newChoice.optionKey = self.key;

            //for (var i = 0; i < self.choices.length; i++)
            //{
            //    if (newChoice.sku == self.choices[i].sku)
            //    {
            //        newChoice.sku = newChoice.sku + 1;
            //        i = -1;
            //    }
            //}

            self.choices.push(newChoice);
        };

        // Helper to remove a choice to this option
        self.removeChoice = function (idx) {

            var removedItems = self.choices.splice(idx, 1);

            for (var i = 0; i < removedItems.length; i++) {
                removedItems[i].isRemoved = true;
            }

            return removedItems;
        };

        // Helper to remove a choice to this option
        self.setSortOrder = function (neworder) {

            self.sortOrder = neworder;

            for (var i = 0; i < self.choices.length; i++) {
                self.choices[i].optionOrder = neworder;
            }
        };

        // Helper to make the sortOrder sync with the order in the choices array
        self.resetChoiceSortOrder = function () {

            for (var i = 0; i < self.choices.length; i++) {
                self.choices[i].sortOrder = i + 1;
            }
        };

        // Helper to check if this choice exists in my choices array
        self.choiceExists = function (choice) {

            var choiceInArray = _.find(self.choices, function (c) { return c.key == choice.key; });
            if (choiceInArray == undefined)
            {
                return false;
            }
            else
            {
                return true;
            }
        };

        // Helper to find a choice by name and return it.  undefined if not found
        self.findChoiceByName = function (name) {

            var choiceInArray = _.find(self.choices, function (c) { return c.name == name; });
            return choiceInArray;
        };
    };

    models.ProductVariant = function (productVariantFromServer) {

        var self = this;

        if (productVariantFromServer == undefined)
        {
            self.key = ""; //merchello.Models.emptyGuid;
            self.productKey = "";
            self.name = "";
            self.sku = "";
            self.price = 0.00;
            self.costOfGoods = 0.00;
            self.salePrice = 0.00;
            self.onSale = false;
            self.manufacturer = "";
            self.manufacturerModelNumber = "";
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
            self.totalInventoryCount = 0;

            self.attributes = [];

            self.catalogInventories = [];

            self.selected = false;
        }
        else {
            self.key = productVariantFromServer.key;
            self.productKey = productVariantFromServer.productKey;
            self.name = productVariantFromServer.name;
            self.sku = productVariantFromServer.sku;
            self.price = productVariantFromServer.price;
            self.costOfGoods = productVariantFromServer.costOfGoods;
            self.salePrice = productVariantFromServer.salePrice;
            self.onSale = productVariantFromServer.onSale;
            self.manufacturer = productVariantFromServer.manufacturer;
            self.manufacturerModelNumber = productVariantFromServer.manufacturerModelNumber;
            self.weight = productVariantFromServer.weight;
            self.length = productVariantFromServer.length;
            self.width = productVariantFromServer.width;
            self.height = productVariantFromServer.height;
            self.barcode = productVariantFromServer.barcode;
            self.available = productVariantFromServer.available;
            self.trackInventory = productVariantFromServer.trackInventory;
            self.outOfStockPurchase = productVariantFromServer.outOfStockPurchase;
            self.taxable = productVariantFromServer.taxable;
            self.shippable = productVariantFromServer.shippable;
            self.download = productVariantFromServer.download;
            self.downloadMediaId = productVariantFromServer.downloadMediaId;
            self.totalInventoryCount = productVariantFromServer.totalInventoryCount;

            self.attributes = _.map(productVariantFromServer.attributes, function (attribute) {
                return new merchello.Models.ProductAttribute(attribute);
            });

            self.attributes = _.sortBy(self.attributes, function (attr) { return attr.sortOrder; });

            self.attributeKeys = _.pluck(self.attributes, 'key');

            self.catalogInventories = _.map(productVariantFromServer.catalogInventories, function (catalogInventory) {
                return new merchello.Models.CatalogInventory(catalogInventory);
            });

            self.selected = false;
        }

        // Helper to copy from product to create a master variant
        self.copyFromProduct = function (product) {

            self.productKey = product.key;
            self.name = product.name;
            self.sku = product.sku;
            self.price = product.price;
            self.costOfGoods = product.costOfGoods;
            self.salePrice = product.salePrice;
            self.onSale = product.onSale;
            self.manufacturer = product.manufacturer;
            self.manufacturerModelNumber = product.manufacturerModelNumber;
            self.weight = product.weight;
            self.length = product.length;
            self.width = product.width;
            self.height = product.height;
            self.barcode = product.barcode;
            self.available = product.available;
            self.trackInventory = product.trackInventory;
            self.outOfStockPurchase = product.outOfStockPurchase;
            self.taxable = product.taxable;
            self.shippable = product.shippable;
            self.download = product.download;
            self.downloadMediaId = product.downloadMediaId;

            self.attributes = [];

            for (var i = 0; i < product.catalogInventories.length; i++) {
                var foundInventory = _.where(self.catalogInventories, { "catalogKey": product.catalogInventories[i].catalogKey });
                if (foundInventory.length == 0) {
                    self.catalogInventories.push(new merchello.Models.CatalogInventory(product.catalogInventories[i]));                  
                }
            }
        };

        self.fixAttributeSortOrders = function (options) {
            for (var i = 0; i < self.attributes.length; i++)
            {
                var attr = self.attributes[i];
                var option = attr.findMyOption(options);
                if (option) {
                    attr.optionOrder = option.sortOrder;              
                }
            }
            self.attributes = _.sortBy(self.attributes, function (attrInList) { return attrInList.optionOrder; });
        };

        // for sorting in a table
        self.addAttributesAsProperties = function (options) {
            for (var i = 0; i < self.attributes.length; i++) {
                var attr = self.attributes[i];
                var option = attr.findMyOption(options);
                if (option) {
                    self[option.name] = attr.name;
                }
            }
        };

        self.isComposedFromAttribute = function (attribute) {
            var composedOf = false;
            for (var i = 0; i < self.attributes.length; i++) {
                var attr = self.attributes[i];
                if (attr.key == attribute.key) {
                    composedOf = true;
                    break;
                }
            }
            return composedOf;
        };

        self.findCatalogInventory = function (catalog) {
            return _.find(self.catalogInventories, function(inv) {
                 return (inv.catalogKey == catalog.key);
            });
        };

        self.ensureAllCatalogInventoriesForWarehouse = function (warehouse) {
            var isInCatalog = false;
            for (var i = 0; i < warehouse.warehouseCatalogs.length; i++) {
                var catalog = warehouse.warehouseCatalogs[i];
                var catalogInventory = self.findCatalogInventory(catalog);
                if (!catalogInventory) {
                    self.addCatalogInventory(warehouse, catalog);
                } else {
                    catalogInventory.setWarehouse(warehouse);
                    catalogInventory.setCatalog(catalog);
                    catalogInventory.productInCatalog = true;
                    isInCatalog = true;
                }
            }
            if (!isInCatalog) {
                var defaultCatalog = warehouse.findDefaultCatalog();
                var defaultCatalogInventory = self.findCatalogInventory(defaultCatalog);
                defaultCatalogInventory.productInCatalog = true;
            }
        };

        self.ensureCatalogInventory = function(defaultWarehouse) {
            if (self.catalogInventories.length == 0) {
                self.addCatalogInventory(defaultWarehouse, defaultWarehouse.findDefaultCatalog());
            }
        };

        // Helper to add a catalog inventory to this product
        self.addCatalogInventory = function (warehouse, catalog) {

            var newCatalogInventory = new merchello.Models.CatalogInventory();
            newCatalogInventory.productVariantKey = self.key;
            newCatalogInventory.warehouseKey = warehouse.key;
            newCatalogInventory.catalogKey = catalog.key;
            newCatalogInventory.catalogName = catalog.name;
            newCatalogInventory.warehouse = warehouse;
            newCatalogInventory.catalog = catalog;

            self.catalogInventories.push(newCatalogInventory);

            return newCatalogInventory;
        };

        // Helper to remove catalog inventories that aren't used / checked
        self.removeUnusedCatalogInventories = function (catalogInventories) {

            return _.reject(catalogInventories, function (inventory) { return !inventory.productInCatalog; });

        };


        self.globalInventoryChanged = function(newVal) {
            if (newVal) {
                var newValInt = parseInt(newVal);
                var totalAcrossCatalogs = 0;
                for (var i = 0; i < self.catalogInventories.length; i++) {
                    self.catalogInventories[i].count = newValInt;
                    totalAcrossCatalogs = totalAcrossCatalogs + newValInt;
                }
                self.totalInventoryCount = totalAcrossCatalogs;
            }
        };
    };

    models.Product = function (productFromServer, dontMapChildren) {

        var self = this;

        if (productFromServer == undefined)
        {
            self.key = "";
            self.name = "";
            self.sku = "";
            self.price = 0.00;
            self.costOfGoods = 0.00;
            self.salePrice = 0.00;
            self.onSale = false;
            self.manufacturer = "";
            self.manufacturerModelNumber = "";
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
        }
        else
        {
            self.key = productFromServer.key;
            self.name = productFromServer.name;
            self.sku = productFromServer.sku;
            self.price = productFromServer.price;
            self.costOfGoods = productFromServer.costOfGoods;
            self.salePrice = productFromServer.salePrice;
            self.onSale = productFromServer.onSale;
            self.manufacturer = productFromServer.manufacturer;
            self.manufacturerModelNumber = productFromServer.manufacturerModelNumber;
            self.weight = productFromServer.weight;
            self.length = productFromServer.length;
            self.width = productFromServer.width;
            self.height = productFromServer.height;
            self.barcode = productFromServer.barcode;
            self.available = productFromServer.available;
            self.trackInventory = productFromServer.trackInventory;
            self.outOfStockPurchase = productFromServer.outOfStockPurchase;
            self.taxable = productFromServer.taxable;
            self.shippable = productFromServer.shippable;
            self.download = productFromServer.download;
            self.downloadMediaId = productFromServer.downloadMediaId;
            self.hasOptions = false;
            self.hasVariants = false;

            if (!dontMapChildren)
            {
                self.productOptions = _.map(productFromServer.productOptions, function (option) {
                    return new merchello.Models.ProductOption(option);
                });

                self.productOptions = _.sortBy(self.productOptions, function (option) { return option.sortOrder; });

                if (self.productOptions.length > 0) {
                    self.hasOptions = true;
                }

                self.productVariants = _.map(productFromServer.productVariants, function (variant) {
                    var jsvariant = new merchello.Models.ProductVariant(variant);
                    jsvariant.fixAttributeSortOrders(self.productOptions);
                    jsvariant.addAttributesAsProperties(self.productOptions);
                    return jsvariant;
                });

                if (self.productVariants.length > 0) {
                    self.hasVariants = true;
                }

                self.catalogInventories = _.map(productFromServer.catalogInventories, function (catalogInventory) {
                    return new merchello.Models.CatalogInventory(catalogInventory);
                });
            }
            else
            {
                if (productFromServer.productOptions.length > 0) {
                    self.hasOptions = true;
                }

                if (productFromServer.productVariants.length > 0) {
                    self.hasVariants = true;
                    self.minPrice = _.min(productFromServer.productVariants, function (v) { return v.price; }).price;
                    self.maxPrice = _.max(productFromServer.productVariants, function (v) { return v.price; }).price;
                }
            }
        }

        // Helper to copy from master variant
        self.copyFromVariant = function (productVariant) {

            self.key = productVariant.productKey;
            self.name = productVariant.name;
            self.sku = productVariant.sku;
            self.price = productVariant.price;
            self.costOfGoods = productVariant.costOfGoods;
            self.salePrice = productVariant.salePrice;
            self.onSale = productVariant.onSale;
            self.manufacturer = productVariant.manufacturer;
            self.manufacturerModelNumber = productVariant.manufacturerModelNumber;
            self.weight = productVariant.weight;
            self.length = productVariant.length;
            self.width = productVariant.width;
            self.height = productVariant.height;
            self.barcode = productVariant.barcode;
            self.available = productVariant.available;
            self.trackInventory = productVariant.trackInventory;
            self.outOfStockPurchase = productVariant.outOfStockPurchase;
            self.taxable = productVariant.taxable;
            self.shippable = productVariant.shippable;
            self.download = productVariant.download;
            self.downloadMediaId = productVariant.downloadMediaId;

            self.catalogInventories = productVariant.catalogInventories.slice(0);
            self.catalogInventories = self.removeUnusedCatalogInventories(self.catalogInventories);
        };

        // Helper to remove catalog inventories that aren't used / checked
        self.removeUnusedCatalogInventories = function (catalogInventories) {

            return _.reject(catalogInventories, function (inventory) { return !inventory.productInCatalog; });

        };

        // Helper to add a variant to this product
        self.addBlankOption = function () {

            if (self.productOptions == undefined) {
                self.productOptions = [];
            }

            var newOption = new merchello.Models.ProductOption();
            newOption.sortOrder = self.productOptions.length + 1;

            self.productOptions.push(newOption);
            self.hasOptions = true;

        };

        // Helper to remove an option from this product
        self.removeOption = function (option) {

            self.productOptions = _.reject(self.productOptions, function (opt) { return _.isEqual(opt, option); });

        };

        // Create an array of all the Choices in a list
        self.flattened = function() {
            var flat = [];
            for (var o = 0; o < self.productOptions.length; o++) {
                var thisOption = self.productOptions[o];
                for (var a = 0; a < thisOption.choices.length; a++) {
                    flat.push(thisOption.choices[a]);
                }
            }

            return flat;
        };


        // Helper to add a variant to this product
        self.addVariant = function (attributes) {

            var newVariant = new merchello.Models.ProductVariant();
            newVariant.copyFromProduct(self);
            newVariant.attributes = attributes.slice(0);
            newVariant.attributeKeys = _.pluck(newVariant.attributes, 'key');
            newVariant.selected = true;
            newVariant.sku = "";
            newVariant.name = "";

            self.productVariants.push(newVariant);
            self.hasVariants = true;

            return newVariant;
        };

        // Helper to remove a variant from this product
        self.removeVariant = function (idx) {

            self.productVariants.splice(idx, 1);
        };

        self.fixAttributeSortOrders = function () {
            for (var i = 0; i < self.productVariants.length; i++) {
                self.productVariants[i].fixAttributeSortOrders(self.productOptions);
            }
        };

        // for sorting in a table
        self.addAttributesAsPropertiesToVariants = function () {
            for (var i = 0; i < self.productVariants.length; i++) {
                self.productVariants[i].addAttributesAsProperties(self.productOptions);
            }
        };

        self.hasAvailableVariantPermutiations = function() {

            var permutations = 1;

            for (var o = 0; o < self.productOptions.length; o++) {
                var thisOption = self.productOptions[o];

                permutations = permutations * thisOption.choices.length;
            }

            var availablePermutations = permutations - self.productVariants.length;

            return availablePermutations;
        };

        self.variantsMinimumPrice = function () {
            if (self.productVariants) {
                return _.min(self.productVariants, function(v) { return v.price; });
            } else {
                return self.minPrice;
            }
        };

        self.variantsMaximumPrice = function () {
            if (self.productVariants) {
                return _.max(self.productVariants, function(v) { return v.price; });
            } else {
                return self.maxPrice;
            }
        };

    };


}(window.merchello.Models = window.merchello.Models || {}));