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
        self.price = '';
    };

    angular.module('merchello.models').constant('BulkVariantChangePricesDialogData', BulkVariantChangePricesDialogData);