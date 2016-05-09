MUI.AddItem.ProductDataTableRow = function() {
    var self = this;
    self.productKey = '',
    self.productVariantKey = '',
    self.sku = '';
    self.isForVariant = false;
    self.onSale = false;
    self.salePrice = 0;
    self.salePriceIncVat = 0;
    self.price = 0;
    self.priceIncVat = 0;
    self.isAvailable = true;
    self.matchKeys = [];
    self.inventoryCount = 0;
    self.outOfStockPurchase = false;
};
