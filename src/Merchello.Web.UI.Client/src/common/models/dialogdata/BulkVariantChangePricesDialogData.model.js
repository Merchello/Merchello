    /**
     * @ngdoc model
     * @name BulkVariantChangePricesDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for bulk changes to product variant prices.
     */
    var BulkVariantChangePricesDialogData = function() {
        var self = this;
        self.productVariants = [];
        self.currencySymbol = '';
        self.price = 0;
        self.costOfGoods = 0;
        self.includeSalePrice = false;
        self.includeCostOfGoods = false;
        self.salePrice = 0;
    };

    angular.module('merchello.models').constant('BulkVariantChangePricesDialogData', BulkVariantChangePricesDialogData);