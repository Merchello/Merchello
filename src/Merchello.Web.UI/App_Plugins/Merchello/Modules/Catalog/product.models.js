(function (models, undefined) {


    models.ProductAttribute = function (productAttributeFromServer) {

        var self = this;

        if (productAttributeFromServer == undefined)
        {
            self.optionId = "";
            self.name = "";
            self.sku = "";
            self.sortOrder = "";
        }
        else
        {
            self.optionId = productAttributeFromServer.optionId;
            self.name = productAttributeFromServer.name;
            self.sku = productAttributeFromServer.sku;
            self.sortOrder = productAttributeFromServer.sortOrder;
        }

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
                return new merchello.Models.ProductAttribute(attribute);
            });
        }

        // Helper to add a choice to this option
        self.addChoice = function (name) {

            var newChoice = new merchello.Models.ProductAttribute();
            newChoice.name = name;
            newChoice.sortOrder = self.choices.length + 1;
            newChoice.sku = newChoice.sortOrder;

            for (var i = 0; i < self.choices.length; i++)
            {
                if (newChoice.sku == self.choices[i].sku)
                {
                    newChoice.sku = newChoice.sku + 1;
                    i = -1;
                }
            }

            self.choices.push(newChoice);
        };

        // Helper to remove a choice to this option
        self.removeChoice = function (idx) {

            self.choices.splice(idx, 1);
        };
    };

    models.ProductVariant = function (productVariantFromServer) {

        var self = this;

        if (productVariantFromServer == undefined)
        {
            self.id = -1;
            self.productKey = "";
            self.name = "";
            self.sku = "";
            self.price = 0.00;
            self.costOfGoods = 0.00;
            self.salePrice = 0.00;
            self.onSale = false;
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
        }
        else {
            self.id = productVariantFromServer.id;
            self.productKey = productVariantFromServer.productKey;
            self.name = productVariantFromServer.name;
            self.sku = productVariantFromServer.sku;
            self.price = productVariantFromServer.price;
            self.costOfGoods = productVariantFromServer.costOfGoods;
            self.salePrice = productVariantFromServer.salePrice;
            self.onSale = productVariantFromServer.onSale;
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
        }

        // Helper to copy from product to create a master variant
        self.copyFromProduct = function (product) {

            self.name = product.name;
            self.sku = product.sku;
            self.price = product.price;
            self.costOfGoods = product.costOfGoods;
            self.salePrice = product.salePrice;
            self.onSale = product.onSale;
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
        };
    };

    models.Product = function (productFromServer) {

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

            self.productOptions = _.map(productFromServer.productOptions, function (option) {
                return new merchello.Models.ProductOption(option);
            });

            if (self.productOptions.length > 0) {
                self.hasOptions = true;
            }

            self.productVariants = _.map(productFromServer.productVariants, function (variant) {
                return new merchello.Models.ProductVariant(variant);
            });

            if (self.productVariants.length > 0) {
                self.hasVariants = true;
            }

        }

        // Helper to copy from master variant
        self.copyFromVariant = function (productVariant) {

            self.name = productVariant.name;
            self.sku = productVariant.sku;
            self.price = productVariant.price;
            self.costOfGoods = productVariant.costOfGoods;
            self.salePrice = productVariant.salePrice;
            self.onSale = productVariant.onSale;
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
        };

        // Helper to add a variant to this product
        self.addBlankOption = function () {

            var newOption = new merchello.Models.ProductOption();
            newOption.sortOrder = self.productOptions.length + 1;

            self.productOptions.push(newOption);
            self.hasOptions = true;

        };


        // Helper to add a variant to this product
        self.addVariant = function (attributes) {

            var newVariant = new merchello.Models.ProductVariant();
            newVariant.copyFromProduct(self);
            newVariant.attributes = attributes.slice(0);

            self.productVariants.push(newVariant);
            self.hasVariants = true;
        };

        // Helper to remove a variant from this product
        self.removeVariant = function (idx) {

            //self.choices.splice(idx, 1);
        };

    };


}(window.merchello.Models = window.merchello.Models || {}));