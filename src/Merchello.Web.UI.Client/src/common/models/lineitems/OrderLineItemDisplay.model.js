    /**
     * @ngdoc model
     * @name OrderLineItemDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's OrderLIneItemDisplay object
     */
    var OrderLineItemDisplay = function() {
        var self = this;

        self.key = '';
        self.containerKey = '';
        self.lineItemTfKey = '';
        self.lineItemTypeField = {};
        self.sku = '';
        self.name = '';
        self.quantity = 0;
        self.price = 0;
        self.exported = false;
        self.lineItemType = '';
        self.shipmentKey = '';
        self.backOrder = false;
        self.extendedData = [];
    };

    OrderLineItemDisplay.prototype = (function() {

        function getProductVariantKey() {
            var variantKey = '';
            if (this.extendedData.length > 0) {
                variantKey = _.find(self.extendedData, function(extDataItem) {
                    return extDataItem['key'] === "merchProductVariantKey";
                });
            }
            if (variantKey === undefined) {
                variantKey = '';
            }
            return variantKey;
        }

    }());

    angular.module('merchello.models').constant('OrderLineItemDisplay', OrderLineItemDisplay);