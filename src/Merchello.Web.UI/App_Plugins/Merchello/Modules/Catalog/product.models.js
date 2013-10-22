(function (models, undefined) {

    models.ProductVariant = function (productVariantFromServer) {

        var self = this;
        self.Name = productVariantFromServer.Name;
        self.Sku = productVariantFromServer.Sku;
        self.Price = productVariantFromServer.Price;

    };

    models.Product = function (productFromServer) {

        var self = this;
        self.Name = productFromServer.Name;
        self.Sku = productFromServer.Sku;
        self.Price = productFromServer.Price;
        self.CostOfGoods = productFromServer.CostOfGoods;
        self.SalePrice = productFromServer.SalePrice;
        self.OnSale = productFromServer.OnSale;
        self.Weight = productFromServer.Weight;
        self.Length = productFromServer.Length;
        self.Width = productFromServer.Width;
        self.Height = productFromServer.Height;
        self.Barcode = productFromServer.Barcode;
        self.Available = productFromServer.Available;
        self.TrackInventory = productFromServer.TrackInventory;
        self.OutOfStockPurchase = productFromServer.OutOfStockPurchase;
        self.Taxable = productFromServer.Taxable;
        self.Shippable = productFromServer.Shippable;
        self.Download = productFromServer.Download;
        self.DownloadMediaId = productFromServer.DownloadMediaId;
        self.productOptions = loadOptions(productFromServer.productOptions);
        self.productVariants = loadVariants(productFromServer.productVariants);

        // private helper to create product options and choices
        function loadOptions(productOptionsArray) {
            return [];
        }

        // private helper to create product variants
        function loadVariants(productVariantsArray) {
            return [];
        }

    };


}(window.merchello.Models = window.merchello.Models || {}));