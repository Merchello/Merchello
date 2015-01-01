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

        function getPaymentStatus() {
            return this.invoiceStatus.name;
        }
        // TODO this may need to be refactored in Umbraco 8 when '_' becomes a module
        function getFulfillmentStatus () {
            if (!_.isEmpty(self.orders)) {
                return self.orders[0].orderStatus.name;
            }
            return '';
        }

        function getProductLineItems() {
            return _.filter(this.items, function (item) { return item.lineItemTypeField.alias === 'Product'; });
        }

        function getTaxLineItem() {
            return _.find(this.items, function (item) { return item.lineItemTypeField.alias === 'Tax'; });
        }

        function getShippingLineItems() {
            return _.find(this.items, function (item) { return item.lineItemTypeField.alias === 'Shipping'; });
        }

        return {
            getPaymentStatus: getPaymentStatus,
            getFulfillmentStatus: getFulfillmentStatus,
            getProductLineItems: getProductLineItems,
            getTaxLineItem: getTaxLineItem,
            getShippingLineItem: getShippingLineItems
        };

    }());

    angular.module('merchello.models').constant('InvoiceDisplay', InvoiceDisplay);