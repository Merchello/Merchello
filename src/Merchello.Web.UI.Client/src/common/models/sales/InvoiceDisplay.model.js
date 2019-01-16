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
        self.billToCountryName = '';
        self.billToEmail = '';
        self.billToPhone = '';
        self.billToCompany = '';
        self.currencyCode = '';
        self.exported = '';
        self.archived = '';
        self.total = 0.0;
        self.currency = {};
        self.items = [];
        self.orders = [];
        self.notes = [];
    };

    InvoiceDisplay.prototype = (function() {

        function ensureArray(items) {
            var collection = [];
            if (items === undefined || items === null) {
                return collection;
            }

            if (!angular.isArray(items)) {
                collection.push(items);
            } else {
                angular.forEach(items, function(item) {
                   collection.push(item);
                });
            }

            return collection;
        }

        function getBillingAddress() {
            var adr = new AddressDisplay();
            adr.address1 = this.billToAddress1;
            adr.address2 = this.billToAddress2;
            adr.locality = this.billToLocality;
            adr.region = this.billToRegion;
            adr.countryCode = this.billToCountryCode;
            adr.countryName = this.billToCountryName;
            adr.postalCode = this.billToPostalCode;
            adr.name = this.billToName;
            adr.phone = this.billToPhone;
            adr.email = this.billToEmail;
            adr.addressType = 'Billing';
            adr.organization = this.billToCompany;
            return adr;
        }

        function setBillingAddress(adr) {
            this.billToAddress1 = adr.address1;
            this.billToAddress2 = adr.address2;
            this.billToLocality = adr.locality;
            this.billToRegion = adr.region;
            this.billToCountryCode = adr.countryCode;
            this.billToPostalCode = adr.postalCode;
            this.billToName = adr.name;
            this.billToEmail = adr.email;
            this.billToCompany = adr.organization;
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

        function getFulfillmentStatus() {
            var keepFindingOrder = true;
            var statusToReturn = 'Fulfilled';

            if (!_.isEmpty(this.orders)) {
                angular.forEach(this.orders,
                    function(order) {
                        if (keepFindingOrder) {
                            if (order.orderStatus.name !== statusToReturn) {

                                statusToReturn = order.orderStatus.name;

                                keepFindingOrder = false;
                            }
                        }
                    });
            } else {
                statusToReturn = 'Not Fulfilled';
            }

            return statusToReturn;
        }

        // gets the currency code for the invoice
        function getCurrencyCode() {

            if(this.currencyCode !== '') {
                return this.currencyCode;
            }

            if (this.currency.currencyCode === '') {
                var first = this.items[0];
                var currencyCode = first.extendedData.getValue('merchCurrencyCode');
                return currencyCode;
            } else {
                return this.currency.currencyCode;
            }
        }

        // gets the product line items
        function getProductLineItems() {
            return ensureArray( _.filter(this.items, function (item) { return item.lineItemTypeField.alias === 'Product'; }));
        }

        // gets the tax line items
        function getTaxLineItem() {
            return _.find(this.items, function (item) { return item.lineItemTypeField.alias === 'Tax'; });
        }

        // gets the shipping line items
        function getShippingLineItems() {
            return ensureArray(_.filter(this.items, function (item) {
                return item.lineItemTypeField.alias === 'Shipping';
            }));
        }

        function getAdjustmentLineItems() {
            return ensureArray(_.filter(this.items, function (item) {
                var adjustmentExtendedData = item.extendedData.getValue('merchAdjustment');
                if (adjustmentExtendedData !== "") {
                    return true;
                }
               return item.lineItemTypeField.alias === 'Adjustment';
            }));
        }

        // gets the custom line items
        function getCustomLineItems() {
            var custom =  ensureArray(_.filter(this.items, function(item) {
                return item.lineItemType === 'Custom';
            }));
            if (custom === undefined) {
                custom = [];
            }
            return custom;
        }

        // gets a collection of discount line items
        function getDiscountLineItems() {
            var discounts = _.filter(this.items, function(item) {
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

        function prefixedInvoiceNumber() {
            if (this.invoiceNumberPrefix === '') {
                return this.invoiceNumber;
            } else {
                return this.invoiceNumberPrefix + '-' + this.invoiceNumber;
            }
        }

        function isAnonymous() {
            return this.customerKey === '00000000-0000-0000-0000-000000000000';
        }

        return {
            getPaymentStatus: getPaymentStatus,
            getFulfillmentStatus: getFulfillmentStatus,
            getCurrencyCode: getCurrencyCode,
            getProductLineItems: getProductLineItems,
            getDiscountLineItems: getDiscountLineItems,
            getAdjustmentLineItems: getAdjustmentLineItems,
            getTaxLineItem: getTaxLineItem,
            getShippingLineItems: getShippingLineItems,
            getCustomLineItems: getCustomLineItems,
            hasOrder: hasOrder,
            isPaid: isPaid,
            getBillToAddress: getBillingAddress,
            setBillingAddress: setBillingAddress,
            remainingBalance: remainingBalance,
            invoiceDateString: invoiceDateString,
            shippingTotal: shippingTotal,
            isAnonymous:  isAnonymous,
            prefixedInvoiceNumber: prefixedInvoiceNumber
        };
    }());

    angular.module('merchello.models').constant('InvoiceDisplay', InvoiceDisplay);