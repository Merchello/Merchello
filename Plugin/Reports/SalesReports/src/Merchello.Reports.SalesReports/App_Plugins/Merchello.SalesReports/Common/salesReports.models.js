(function(models, undefined) {

    models.SaleByItemResult = function(data) {

        var self = this;

        if (data == undefined) {
            self.productVariant = {};
            self.quantity = 0;
            self.total = 0;
        } else {
            self.productVariant = new merchello.Models.ProductVariant(data.productVariant, true);
            self.quantity = data.quantity;
            self.total = data.total;
        }
    }

    models.SaleOverTimeResult = function (data) {

        var self = this;

        if (data == undefined) {
            self.date = {};
            self.salestotal = 0;
            self.salescount = 0;
        } else {
            self.date = data.date;
            self.salestotal = data.salestotal;
            self.salescount = data.salescount;
        }
    }


}(window.merchello.Models = window.merchello.Models || {}));