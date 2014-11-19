(function(models, undefined) {

    models.SaleByItemResult = function(data) {

        var self = this;

        if (data == undefined) {
            self.product = {};
            self.quantity = 0;
            self.total = 0;
        } else {
            self.product = new merchello.Models.ProductVariant(data.productVariant, true);
            self.quantity = data.quantity;
            self.total = data.total;
        }
    }

}(window.merchello.Models = window.merchello.Models || {}));