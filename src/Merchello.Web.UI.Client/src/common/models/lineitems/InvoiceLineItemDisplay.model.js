    /**
     * @ngdoc model
     * @name InvoiceLineItemDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's InvoiceLIneItemDisplay object
     */
    var InvoiceLineItemDisplay = function() {
        var self = this;

        self.key = '';
        self.containerKey = '';
        self.lineItemTfKey = '';
        self.lineItemType = '';
        self.lineItemTypeField = {};
        self.sku = '';
        self.name = '';
        self.quantity = '';
        self.price = '';
        self.exported = false;
        self.extendedData = {};
    };

    InvoiceLineItemDisplay.prototype = (function() {

        function absPrice() {
            return Math.abs(this.price);
        }

        return {
            absPrice : absPrice
        };

    }());

    angular.module('merchello.models').constant('InvoiceLineItemDisplay', InvoiceLineItemDisplay);