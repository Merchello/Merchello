(function (models, undefined) {


    models.ProductAttribute = function (productAttributeFromServer) {

        var self = this;
        self.optionId = productAttributeFromServer.Name;
        self.name = productAttributeFromServer.Name;
        self.sku = productAttributeFromServer.Sku;
        self.sortOrder = productAttributeFromServer.SortOrder;

    };

    models.ProductOption = function (productOptionFromServer) {

        var self = this;
        self.key = productOptionFromServer.Key;
        self.name = productOptionFromServer.Name;
        self.required = productOptionFromServer.Required;
        self.sortOrder = productOptionFromServer.SortOrder;

        self.choices = _.map(productOptionFromServer.Choices, function (attribute) {
            return new ProductAttribute(attribute);
        });
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
            self.id = productVariantFromServer.Id;
            self.productKey = productVariantFromServer.ProductKey;
            self.name = productVariantFromServer.Name;
            self.sku = productVariantFromServer.Sku;
            self.price = productVariantFromServer.Price;
            self.costOfGoods = productVariantFromServer.CostOfGoods;
            self.salePrice = productVariantFromServer.SalePrice;
            self.onSale = productVariantFromServer.OnSale;
            self.weight = productVariantFromServer.Weight;
            self.length = productVariantFromServer.Length;
            self.width = productVariantFromServer.Width;
            self.height = productVariantFromServer.Height;
            self.barcode = productVariantFromServer.Barcode;
            self.available = productVariantFromServer.Available;
            self.trackInventory = productVariantFromServer.TrackInventory;
            self.outOfStockPurchase = productVariantFromServer.OutOfStockPurchase;
            self.taxable = productVariantFromServer.Taxable;
            self.shippable = productVariantFromServer.Shippable;
            self.download = productVariantFromServer.Download;
            self.downloadMediaId = productVariantFromServer.DownloadMediaId;
            self.totalInventoryCount = productVariantFromServer.TotalInventoryCount;

            self.attributes = _.map(productVariantFromServer.Attributes, function (attribute) {
                return new ProductAttribute(attribute);
            });
        }

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

            self.productOptions = [];

            self.productVariants = [];
        }
        else
        {
            self.key = productFromServer.Key;
            self.name = productFromServer.Name;
            self.sku = productFromServer.Sku;
            self.price = productFromServer.Price;
            self.costOfGoods = productFromServer.CostOfGoods;
            self.salePrice = productFromServer.SalePrice;
            self.onSale = productFromServer.OnSale;
            self.weight = productFromServer.Weight;
            self.length = productFromServer.Length;
            self.width = productFromServer.Width;
            self.height = productFromServer.Height;
            self.barcode = productFromServer.Barcode;
            self.available = productFromServer.Available;
            self.trackInventory = productFromServer.TrackInventory;
            self.outOfStockPurchase = productFromServer.OutOfStockPurchase;
            self.taxable = productFromServer.Taxable;
            self.shippable = productFromServer.Shippable;
            self.download = productFromServer.Download;
            self.downloadMediaId = productFromServer.DownloadMediaId;

            self.productOptions = _.map(productFromServer.ProductOptions, function (option) {
                return new ProductOption(option);
            });

            self.productVariants = _.map(productFromServer.ProductVariants, function (variant) {
                return new ProductVariant(variant);
            });

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
        }

    };


}(window.merchello.Models = window.merchello.Models || {}));