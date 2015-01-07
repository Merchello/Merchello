    /**
     * @ngdoc model
     * @name InvoiceDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's InvoiceDisplay object
     */
    var InvoiceDisplay = function() {
        var self = this;
        self.key = '';
        self.versionKey = '';
        self.customerKey = '';
        self.invoiceNumberPrefix = '';
        self.invoiceNumber = '';
        self.invoiceDate = '';
        self.invoiceStatusKey = '';
        self.invoiceStatus = {};
        self.billToName = '';
        self.billToAddress1 = '';
        self.billToAddress2 = '';
        self.billToLocality = '';
        self.billToRegion = '';
        self.billToPostalCode = '';
        self.billToCountryCode = '';
        self.billToEmail = '';
        self.billToPhone = '';
        self.billToCompany = '';
        self.exported = '';
        self.archived = '';
        self.total = 0.0;
        self.items = [];
        self.orders = [];
    };

    InvoiceDisplay.prototype = (function() {

        // gets the invoice status name
        // TODO this is incorrectly namedd
        function getPaymentStatus() {
            return this.invoiceStatus.name;
        }

        function getFulfillmentStatus () {
            if (!_.isEmpty(self.orders)) {
                return self.orders[0].orderStatus.name;
            }
            return '';
        }

        // gets the product line items
        function getProductLineItems() {
            return _.filter(this.items, function (item) { return item.lineItemTypeField.alias === 'Product'; });
        }

        // gets the tax line items
        function getTaxLineItem() {
            return _.find(this.items, function (item) { return item.lineItemTypeField.alias === 'Tax'; });
        }

        // gets the shipping line items
        function getShippingLineItems() {
            return _.find(this.items, function (item) { return item.lineItemTypeField.alias === 'Shipping'; });
        }

        // gets a value indicating whether or not this invoice has an order
        function hasOrder() {
            return this.orders.length > 0;
        }

        // gets a value indicating whether or not this invoice has been paid
        function isPaid() {
            var status = this.getPaymentStatus.call(this);
            return status === 'Paid';
        }

        return {
            getPaymentStatus: getPaymentStatus,
            getFulfillmentStatus: getFulfillmentStatus,
            getProductLineItems: getProductLineItems,
            getTaxLineItem: getTaxLineItem,
            getShippingLineItem: getShippingLineItems,
            hasOrder: hasOrder,
            isPaid: isPaid
        };
    }());

    angular.module('merchello.models').constant('InvoiceDisplay', InvoiceDisplay);