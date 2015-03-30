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

        function getBillingAddress() {
            var adr = new AddressDisplay();
            adr.address1 = this.billToAddress1;
            adr.address2 = this.billToAddress2;
            adr.locality = this.billToLocality;
            adr.region = this.billToRegion;
            adr.countryCode = this.billToCountryCode;
            adr.postalCode = this.billToPostalCode;
            adr.name = this.billToName;
            adr.phone = this.billToPhone;
            adr.email = this.billToEmail;
            adr.organization = this.billToCompany;
            return adr;
        }

        // gets the invoice date as a date string
        function invoiceDateString() {
            return this.invoiceDate.split('T')[0];
        }

        // gets the invoice status name
        // TODO this is incorrectly named
        function getPaymentStatus() {
            return this.invoiceStatus.name;
        }

        function getFulfillmentStatus () {
            if (!_.isEmpty(this.orders)) {
                return this.orders[0].orderStatus.name;
            }
            // TODO this should be localized
            return 'Not Fulfilled';
        }

        // gets the currency code for the invoice
        function getCurrencyCode() {
            var first = this.items[0];
            var currencyCode = first.extendedData.getValue('merchCurrencyCode');
            return currencyCode;
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
            return _.find(this.items, function (item) {
                return item.lineItemTypeField.alias === 'Shipping';
            });
        }

        // gets the custom line items
        function getCustomLineItems() {
            var custom =  _.find(this.items, function(item) {
                return item.lineItemType === 'Custom';
            });
            if (custom === undefined) {
                custom = [];
            }
            return custom;
        }

        // gets a collection of discount line items
        function getDiscountLineItems() {
            var discounts = _.find(this.items, function(item) {
                return item.lineItemTypeField.alias === 'Discount';
            });
            if (discounts === undefined) {
                discounts = [];
            }
            return discounts;
        }

        function shippingTotal() {
            var shippingLineItems = getShippingLineItems.call(this);
            var total = 0;
            if (shippingLineItems) {
                if (shippingLineItems.length) {
                    angular.forEach(shippingLineItems, function(lineItem) {
                      total += lineItem.price;
                    });
                } else {
                    total += shippingLineItems.price;
                }
            }
            return total;
        }

        // gets a value indicating whether or not this invoice has an order
        function hasOrder() {
            return this.orders.length > 0;
        }

        // gets a value indicating whether or not this invoice has been paid
        function isPaid() {
            // http://issues.merchello.com/youtrack/issue/M-659
            var status = this.invoiceStatus.alias;
            return status === 'paid';
        }

        // calculates the unpaid balance of the invoice
        function remainingBalance(payments) {
            var amountPaid = 0;
            angular.forEach(payments, function(payment) {
                angular.forEach(payment.appliedPayments, function(applied) {
                    amountPaid += applied.amount;
                });
            });
            return this.total - amountPaid;
        }

        return {
            getPaymentStatus: getPaymentStatus,
            getFulfillmentStatus: getFulfillmentStatus,
            getCurrencyCode: getCurrencyCode,
            getProductLineItems: getProductLineItems,
            getDiscountLineItems: getDiscountLineItems,
            getTaxLineItem: getTaxLineItem,
            getShippingLineItems: getShippingLineItems,
            getCustomLineItems: getCustomLineItems,
            hasOrder: hasOrder,
            isPaid: isPaid,
            getBillToAddress: getBillingAddress,
            remainingBalance: remainingBalance,
            invoiceDateString: invoiceDateString,
            shippingTotal: shippingTotal
        };
    }());

    angular.module('merchello.models').constant('InvoiceDisplay', InvoiceDisplay);