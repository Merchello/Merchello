/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */


(function() { 

    /**
    * @ngdoc model
    * @name AddressDisplay
    * @function
    * 
    * @description
    * Represents a JS version of Merchello's AddressDisplay object
    */
    var AddressDisplay = function () {

        var self = this;

        self.name = '';
        self.address1 = '';
        self.address2 = '';
        self.locality = '';
        self.region = '';
        self.postalCode = '';
        self.countryCode = '';
        self.addressType = '';
        self.organization = '';
        self.phone = '';
        self.email = '';
        self.isCommercial = false;
    };
    
    angular.module('merchello.models').constant('AddressDisplay', AddressDisplay);

    /**
     * @ngdoc model
     * @name CountryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CountryDisplay object
     */
    var CountryDisplay = function() {
        var self = this;

        self.key = '';
        self.countryCode = '';
        self.name = '';
        self.provinceLabel = '';
        self.provinces = [];
    };

    angular.module('merchello.models').constant('CountryDisplay', CountryDisplay);
    /**
     * @ngdoc model
     * @name CurrencyDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CurrencyDisplay object
     */
    var CurrencyDisplay = function() {
        var self = this;
        self.name = '';
        self.currencyCode = '';
        self.symbol = '';
    };

    angular.module('merchello.models').constant('CurrencyDisplay', CurrencyDisplay);

    /**
     * @ngdoc model
     * @name ExtendedDataDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ExtendedDataDisplay object
     */
    var ExtendedDataDisplay = function() {
        var items = [];
    };

    ExtendedDataDisplay.prototype = (function() {

        function isEmpty() {
            return items.length === 0;
        }

        function getValue(key) {
            return _.where(this.items, { key: key });
        }

        function setValue(key, value) {
            var found = false;
            var i = 0;
            while(i < this.items.length && !found) {
                if (this.items[0].key === key) {
                    found = true;
                    this.items[ i ].value = value;
                }
                i++;
            }
            if (found) {
                return;
            }
            this.items.push({ key: key, value: value });
        }

        return {
            isEmpty: isEmpty,
            getValue: getValue,
            setValue: setValue
        };
    }());

    angular.module('merchello.models').constant('ExtendedDataDisplay', ExtendedDataDisplay);
    /**
     * @ngdoc model
     * @name ProvinceDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProvinceDisplay object
     */
     var ProvinceDisplay = function()
     {
         var self = this;
         self.name = '';
         self.code = '';
     };

    angular.module('merchello.models').constant('ProvinceDisplay', ProvinceDisplay);
    /**
     * @ngdoc model
     * @name Merchello.Models.Province
     * @function
     *
     * @description
     * Represents a JS version of Merchello's SettingDisplay object
     */
    var SettingDisplay = function() {
        self.currencyCode = '';
        self.nextOrderNumber = 0;
        self.nextInvoiceNumber = 0;
        self.nextShipmentNumber = 0;
        self.dateFormat = '';
        self.timeFormat = '';
        self.unitSystem = '';
        self.globalShippable = false;
        self.globalTaxable = false;
        self.globalTrackInventory = false;
        self.globalShippingIsTaxable = false;
    };

    angular.module('merchello.models').constant('SettingDisplay', SettingDisplay);
    /**
     * @ngdoc model
     * @name Merchello.Models.TypeField
     * @function
     *
     * @description
     * Represents a JS version of Merchello's TypeFieldDisplay object
     */
    var TypeFieldDisplay = function() {
        var self = this;
        self.alias = 'test';
        self.name = 'test';
        self.typeKey = '';
    };

    angular.module('merchello.models').constant('TypeFieldDisplay', TypeFieldDisplay);

    /**
     * @ngdoc model
     * @name PaymentRequest
     * @function
     *
     * @description
     * A back office model used for making payment requests to a payment provider
     *
     * @note
     * Presently there is not a corresponding Merchello.Web model
     */
    var CapturePaymentDialogData = function() {
        var self = this;
        self.currencySymbol = '';
        self.invoiceKey = '';
        self.paymentKey = '';
        self.paymentMethodKey = '';
        self.invoiceBalance = 0.0;
        self.amount = 0.0;
        self.processorArgs = [];
    };

    CapturePaymentDialogData.prototype = (function() {

        function setup(payments, invoice, currencySymbol) {
            if (payments.length > 0) {
                var payment = payments[0];
                this.paymentMethodKey = payment.paymentMethodKey;
                this.paymentMethodName = payment.paymentMethodName;
            }
            if (invoice !== undefined) {
                this.invoiceBalance = invoice.remainingBalance(payments);
            }
            if (currencySymbol !== undefined) {
                this.currencySymbol = currencySymbol;
            }
        }

        return {
            setup: setup
        };

    }());

    angular.module('merchello.models').constant('CapturePaymentDialogData', CapturePaymentDialogData);

    /**
     * @ngdoc model
     * @name GatewayResourceDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's GatewayResourceDisplay object
     */
    var GatewayResourceDisplay = function() {
        self.name = '';
        self.serviceCode = '';
    };

    angular.module('merchello.models').constant('GatewayResourceDisplay', GatewayResourceDisplay);
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
        self.lineItemTypeField = {};  // TODO why is this here
        self.sku = '';
        self.name = '';
        self.quantity = '';
        self.price = '';
        self.exported = false;
        self.extendedData = {};
    };

    angular.module('merchello.models').constant('InvoiceLineItemDisplay', InvoiceLineItemDisplay);
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
    /**
     * @ngdoc model
     * @name AppliedPaymentDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's AppliedPaymentDisplay object
     */
    var AppliedPaymentDisplay = function() {
        var self = this;
        self.key = '';
        self.paymentKey = '';
        self.invoiceKey = '';
        self.appliedPaymentTfKey = '';
        self.description = '';
        self.amount = 0.0;
        self.exported = false;
        self.createDate = '';
    };

    angular.module('merchello.models').constant('AppliedPaymentDisplay', AppliedPaymentDisplay);

    /**
     * @ngdoc model
     * @name PaymentDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's PaymentDisplay object
     */
    var PaymentDisplay = function() {
        var self = this;
        self.key = '';
        self.customerKey = '';
        self.paymentMethodKey = '';
        self.paymentTypeFieldKey = '';
        self.paymentMethodName = '';
        self.referenceNumber = '';
        self.amount = 0.0;
        self.authorized = false;
        self.collected = false;
        self.exported = false;
        self.extendedData = {};
        self.appliedPayments = [];
    };

    PaymentDisplay.prototype = (function() {

        // private
        var getStatus = function() {
                var statusArr = [];
                if (this.authorized) {
                    statusArr.push("Authorized");
                }
                if (this.collected) {
                    statusArr.push("Captured");
                }
                if (this.exported) {
                    statusArr.push("Exported");
                }

                return statusArr.join("/");
            },

            hasAmount = function() {
                return this.amount > 0;
            };

        // public
        return {
            getStatus: getStatus,
            hasAmount: hasAmount
        };
    }());

    angular.module('merchello.models').constant('PaymentDisplay', PaymentDisplay);
    /**
     * @ngdoc model
     * @name QueryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's QueryDisplay object
     *
     * @remark
     * PetaPoco Page<T> uses a 1 based page rather than a 0 based to represent the first page.
     * We do the conversion in the WebApiController - so the JS QueryDisplay should assume this is 0 based.
     */
    var QueryDisplay = function() {
        var self = this;
        self.currentPage = 0;
        self.itemsPerPage = 25;
        self.parameters = [];
        self.sortBy = '';
        self.sortDirection = 'Ascending'; // valid options are 'Ascending' and 'Descending'
    };

    QueryDisplay.prototype = (function() {
        // private
        function addParameter(queryParameter) {
            this.parameters.push(queryParameter);
        }

        function addCustomerKeyParam(customerKey) {
            var param = new QueryParameterDisplay();
            param.fieldName = 'customerKey';
            param.value = customerKey;
            addParameter.call(this, param);
        }

        function addInvoiceDateParam(dateString, startOrEnd) {
            var param = new QueryParameterDisplay();
            param.fieldName = startOrEnd === 'start' ? 'invoiceDateStart' : 'invoiceEndDate';
            param.value = dateString;
            addParameter.call(this, param);
        }


        function addFilterTermParam(term) {
            if(term.length <= 0) {
                return;
            }
            var param = new QueryParameterDisplay();
            param.fieldName = 'term';
            param.value = term;
            addParameter.call(this, param);
        }

        function applyInvoiceQueryDefaults() {
            this.sortBy = 'invoiceNumber';
            this.sortDirection = 'Descending';
        }

        // public
        return {
            addParameter: addParameter,
            addCustomerKeyParam: addCustomerKeyParam,
            applyInvoiceQueryDefaults: applyInvoiceQueryDefaults,
            addInvoiceDateParam: addInvoiceDateParam,
            addFilterTermParam: addFilterTermParam
        };
    }());

    angular.module('merchello.models').constant('QueryDisplay', QueryDisplay);
    /**
     * @ngdoc model
     * @name QueryParameterDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ListQueryParameterDisplay object
     */
    var QueryParameterDisplay = function()
    {
        var self = this;
        self.fieldName = '';
        self.value = '';
    };

    angular.module('merchello.models').constant('QueryParameterDisplay', QueryParameterDisplay);

    /**
     * @ngdoc model
     * @name QueryResultDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's QueryResultDisplay object
     */
    var QueryResultDisplay = function() {
        var self = this;
        self.currentPage = 0;
        self.items = [];
        self.itemsPerPage = 0;
        self.totalItems = 0;
        self.totalPages = 0;
    };

    QueryResultDisplay.prototype = (function() {
        function addItem(item) {
            this.items.push(item);
        }

        return {
            addItem: addItem
        };
    }());

    angular.module('merchello.models').constant('QueryResultDisplay', QueryResultDisplay);
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

        // gets the invoice status name
        // TODO this is incorrectly named
        function getPaymentStatus() {
            return this.invoiceStatus.name;
        }

        function getFulfillmentStatus () {
            if (!_.isEmpty(self.orders)) {
                return self.orders[0].orderStatus.name;
            }
            // TODO this should be localized
            return 'Not Fulfilled';
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

        function remainingBalance(payments) {
            var amountPaid = 0;
            //angular.forEach(payments, function(payment) {
            //  amountPaid += payment.amount;
            //});
            return this.total - amountPaid;
        }

        return {
            getPaymentStatus: getPaymentStatus,
            getFulfillmentStatus: getFulfillmentStatus,
            getProductLineItems: getProductLineItems,
            getTaxLineItem: getTaxLineItem,
            getShippingLineItem: getShippingLineItems,
            hasOrder: hasOrder,
            isPaid: isPaid,
            getBillToAddress: getBillingAddress,
            remainingBalance: remainingBalance
        };
    }());

    angular.module('merchello.models').constant('InvoiceDisplay', InvoiceDisplay);
    /**
     * @ngdoc model
     * @name InvoiceStatusDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's InvoiceStatusDisplay object
     */
    var InvoiceStatusDisplay = function() {
        var self = this;

        self.key = '';
        self.name = '';
        self.alias = '';
        self.reportable = '';
        self.active = '';
        self.sortOrder = '';
    };

    angular.module('merchello.models').constant('InvoiceStatusDisplay', InvoiceStatusDisplay);
    /**
     * @ngdoc model
     * @name OrderDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's OrderDisplay object
     */
    var OrderDisplay = function() {
        var self = this;
        self.key = '';
        self.versionKey = '';
        self.invoiceKey = '';
        self.orderNumberPrefix = '';
        self.orderNumber = '';
        self.orderDate = '';
        self.orderStatusKey = ''; // to do this is not needed since we have the OrderStatusDisplay
        self.orderStatus = {};
        self.exported = false;
        self.items = [];
    };

    OrderDisplay.prototype = (function() {
        // private
        // TODO this could address the issue in current merchello.view.controller
        // $scope.processFulfillShipmentDialog
        function createShipment(shipmentStatus, origin, destination, lineItems) {
            if (shipmentStatus === undefined) {
                return;
            }
            var shipment = new ShipmentDisplay();
            shipment.setOriginAddress(origin);
            shipment.setDestinationAddress(destination);
            shipment.shipmentStatus = shipmentStatus;
            if (lineItems === undefined) {
                shipment.items = this.items;
            }
            else {
                shipment.items = lineItems;
            }
            return shipment;
        }

        // public
        return {
            createShipment: createShipment
        };

    }());

    angular.module('merchello.models').constant('OrderDisplay', OrderDisplay);
    /**
     * @ngdoc model
     * @name OrderStatusDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's OrderStatusDisplay object
     */
    var OrderStatusDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.alias = '';
        self.reportable = '';
        self.active = '';
        self.sortOrder = '';
    };

    angular.module('merchello.models').constant('OrderStatusDisplay', OrderStatusDisplay);
    /**
    * @ngdoc model
    * @name ShipmentDisplay
    * @function
    * 
    * @description
    * Represents a JS version of Merchello's ShipmentDisplay object
    */
    var ShipmentDisplay = function () {

        var self = this;

        self.key = '';
        self.shipmentNumber = '';
        self.shipmentNumberPrefix = '';
        self.versionKey = '';
        self.fromOrganization = '';
        self.fromName = '';
        self.fromAddress1 = '';
        self.fromAddress2 = '';
        self.fromLocality = '';
        self.fromRegion = '';
        self.fromPostalCode = '';
        self.fromCountryCode = '';
        self.fromIsCommercial = '';
        self.toOrganization = '';
        self.toName = '';
        self.toAddress1 = '';
        self.toAddress2 = '';
        self.toLocality = '';
        self.toRegion = '';
        self.toPostalCode = '';
        self.toCountryCode = '';
        self.toIsCommercial = '';
        self.shipMethodKey = '';
        self.phone = '';
        self.email = '';
        self.carrier = '';
        self.trackingCode = '';
        self.shippedDate = '';
        self.items = [];
        self.shipmentStatus = {};
    };

    // Shipment Prototype
    // ------------------------------------------------
    ShipmentDisplay.prototype = (function () {

        //// Private members

            // returns the shipment destination as an Address
        var getDestinationAddress = function() {
                return buildAddress.call(this, this.toName, this.toAddress1, this.toAddress2, this.toLocality, this.toRegion,
                    this.toPostalCode, this.toCountryCode, this.toOrganization, this.toIsCommercial, '', '', 'shipping');
            },

            // returns the shipment origin as an Address
            getOriginAddress = function() {
                return buildAddress.call(this, this.fromName, this.fromAddress1, this.fromAddress2, this.fromLocality,
                    this.fromRegion, this.fromPostalCode, this.fromCountryCode, this.fromOrganization,
                    this.fromIsCommercial, '', '', 'shipping');
            },

            setDestinationAddress = function(address)
            {
                this.toName = address.name;
                this.toAddress1 = address.address1;
                this.toAddress2 = address.address2;
                this.toLocality = address.locality;
                this.toRegion = address.region;
                this.toPostalCode = address.postalCode;
                this.toCountryCode = address.countryCode;
                this.toOrganization = address.organization;
                this.toIsCommercial = address.isCommercial;
                this.phone = address.phone;
                this.email = address.email;
            },

            setOriginAddress = function(address) {
                this.fromName = address.name;
                this.fromAddress1 = address.address1;
                this.fromAddress2 = address.address2;
                this.fromLocality = address.locality;
                this.fromRegion = address.region;
                this.fromPostalCode = address.postalCode;
                this.fromCountryCode = address.countryCode;
                this.fromOrganization = address.organization;
                this.fromIsCommercial = address.isCommercial;
            },

            // Utility to build an address
            buildAddress = function(name, address1, address2, locality, region, postalCode, countryCode, organization,
                                    isCommercial, phone, email, addressType) {
                var adr = new AddressDisplay();
                adr.name = name;
                adr.address1 = address1;
                adr.address2 = address2;
                adr.locality = locality;
                adr.region = region;
                adr.postalCode = postalCode;
                adr.countryCode = countryCode;
                adr.organization = organization;
                adr.isCommercial = isCommercial;
                adr.phone = phone;
                adr.email = email;
                adr.addressType = addressType;
                return adr;
            };

        // public members
        return {
            /**
            * @ngdoc method
            * @name merchello.models.Shipment.getDestinationAddress
            * @function
            *
            * @description
            * Returns a merchello.models.Address representing the shipment destination
            */
            getDestinationAddress: getDestinationAddress,

            /**
            * @ngdoc method
            * @name merchello.models.Shipment.getOriginAddress
            * @function
            *
            * @description
            * Returns a merchello.models.Address representing the shipment origin
            */
            getOriginAddress: getOriginAddress,

            /**
             * @ngdoc method
             * @name merchello.models.Shipment.setDestinationAddress
             * @function
             *
             * @description
             * Sets the destination address for a shipment
             */
            setDestinationAddress: setDestinationAddress,

            /**
             * @ngdoc method
             * @name merchello.models.Shipment.setOriginAddress
             * @function
             *
             * @description
             * Sets the origin address for a shipment
             */
            setOriginAddress: setOriginAddress
        };
    }());

    angular.module('merchello.models').constant('ShipmentDisplay', ShipmentDisplay);

    /**
    * @ngdoc model
    * @name merchello.models.shipmentStatus
    * @function
    * 
    * @description
    * Represents a JS version of Merchello's ShipmentStatusDisplay object
    */
    var ShipmentStatusDisplay = function () {
        var self = this;
        self.key = '';
        self.name = '';
        self.alias = '';
        self.reportable = '';
        self.active = '';
        self.sortOrder = '';
    };

    angular.module('merchello.models').constant('ShipmentStatusDisplay', ShipmentStatusDisplay);
    /**
     * @ngdoc model
     * @name AuditLogDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's AuditLogDisplay object
     */
    var AuditLogDisplay = function() {
        var self = this;

        self.entityKey = '';
        self.entityTfKey = '';
        self.entityType = '';
        self.extendedData = {};
        self.isError = false;
        self.key = '';
        self.message = {};
        self.recordDate = '';
        self.verbosity = '';
    };

    AuditLogDisplay.prototype = (function() {

        function toDateString() {
            return this.recordDate.split('T')[0];
        }

        function toTimeString() {
            var time = this.recordDate.split('T')[1];
            return time.split(':')[0] + ':' + time.split(':')[1];
        }

        return {
            toDateString: toDateString,
            toTimeString: toTimeString
        };

    }());

    angular.module('merchello.models').constant('AuditLogDisplay', AuditLogDisplay);
    /**
     * @ngdoc model
     * @name DailyAuditLogDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's DailyAuditLogDisplay object
     */
    var DailyAuditLogDisplay = function() {
        var self = this;
        this.day = '';
        this.logs = [];
    };

    DailyAuditLogDisplay.prototype = (function() {

        //// TODO this is not working as expected so we are keeping it internal
        function dayToDate() {
            //// 0: YYYY
            //// 1: MM
            //// 2: DD
            var dateParts = toDateString.call(this).split('-');
            var timeParts = toTimeString.call(this).split(':');
            console.info(dateParts);
            return Date.parse(dateParts[0], dateParts[1] - 1, dateParts[2], timeParts[0], timeParts[1], 0, 0);
        }

        function toDateString() {
            return this.day.split('T')[0];
        }

        function toTimeString() {
            var time = this.day.split('T')[1];
            return time.split(':')[0] + ':' + time.split(':')[1];
        }

        return {
            toDateString: toDateString,
            toTimeString: toTimeString
        };

    }());

    angular.module('merchello.models').constant('DailyAuditLogDisplay', DailyAuditLogDisplay);
    /**
     * @ngdoc model
     * @name SalesHistoryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's SalesHistoryDisplay object
     */
    var SalesHistoryDisplay = function() {
        var self = this;
        self.dailyLogs = [];
    };

    SalesHistoryDisplay.prototype = (function() {

        // utility method to push a daily log
        function addDailyLog(dailyLog) {
            this.dailyLogs.push(dailyLog);
        }

        return {
            addDailyLog: addDailyLog
        };

    }());

    angular.module('merchello.models').constant('SalesHistoryDisplay', SalesHistoryDisplay);
    /**
     * @ngdoc model
     * @name SalesHistoryMessageDisplay
     * @function
     *
     * @description
     * Represents a sales history message object
     */
    var SalesHistoryMessageDisplay = function() {
        var self = this;
        self.area = '';
        self.key = '';
    };

    SalesHistoryMessageDisplay.prototype = (function() {

        // constructs a localization key
        function localizationKey() {
            return this.area + '_' + this.key;
        }

        // any extra properties on this object are assumed to be tokens used in the localized
        // message
        function localizationTokens() {
            var allKeys = Object.keys(this);
            var tokens = [];
            for(var i = 0; i < allKeys.length; i++) {
                if (allKeys[i] !== 'area' && allKeys[i] !== 'key')
                {
                    tokens.push(this[allKeys[i]]);
                }
            }
            return tokens;
        }

        return {
            localizationKey: localizationKey,
            localizationTokens: localizationTokens
        };
    }());

    angular.module('merchello.models').constant('SalesHistoryMessageDisplay', SalesHistoryMessageDisplay);
    /**
   * @ngdoc service
   * @name merchello.models.genericModelBuilder
   * 
   * @description
   * A utility service that builds local models for API query results
   *  http://odetocode.com/blogs/scott/archive/2014/03/17/building-better-models-for-angularjs.aspx
   */
    angular.module('merchello.models')
        .factory('genericModelBuilder', [
            function() {
        
        // private
        // transforms json object into a local model
        function transformObject(jsonResult, Constructor) {
            var model = new Constructor();
            angular.extend(model, jsonResult);
            return model;
        }

        function transform(jsonResult, Constructor) {
            if (angular.isArray(jsonResult)) {
                var models = [];
                angular.forEach(jsonResult, function (object) {
                    models.push(transformObject(object, Constructor));
                });
                return models;
            } else {
                return transformObject(jsonResult, Constructor);
            }
        }

        // public
        return {
            transform : transform
        };
    }]);
    /**
     * @ngdoc service
     * @name merchello.models.addressDisplayBuilder
     *
     * @description
     * A utility service that builds AddressDisplay models
     */
    angular.module('merchello.models')
        .factory('addressDisplayBuilder',
            ['genericModelBuilder', 'AddressDisplay',
                function(genericModelBuilder, AddressDisplay) {

                var Constructor = AddressDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
        }]);

    /**
     * @ngdoc service
     * @name merchello.models.countryDisplayBuilder
     *
     * @description
     * A utility service that builds CountryDisplay models
     */
    angular.module('merchello.models')
        .factory('countryDisplayBuilder',
        ['genericModelBuilder', 'provinceDisplayBuilder', 'CountryDisplay',
            function(genericModelBuilder, provinceDisplayBuilder, CountryDisplay) {

                var Constructor = CountryDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var countries = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < countries.length; i++) {
                            countries[i].provinces = provinceDisplayBuilder.transform(jsonResult[ i ].provinces);
                        }
                        return countries;
                    }
                };

        }]);

    /**
     * @ngdoc service
     * @name merchello.models.currencyDisplayBuilder
     *
     * @description
     * A utility service that builds CurrencyDisplay models
     */
    angular.module('merchello.models')
        .factory('currencyDisplayBuilder',
        ['genericModelBuilder', 'CurrencyDisplay',
            function(genericModelBuilder, CurrencyDisplay) {

                var Constructor = CurrencyDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

/**
 * @ngdoc service
 * @name merchello.models.dialogDataFactory
 *
 * @description
 * A utility service that builds dialogData models
 */
angular.module('merchello.models').factory('dialogDataFactory',
    ['CapturePaymentDialogData',
    function(CapturePaymentDialogData) {

        function getCapturePaymentDialogData() {
            return new CapturePaymentDialogData();
        }

        return {
            getCapturePaymentDialogData: getCapturePaymentDialogData
        };
}]);

    /**
     * @ngdoc service
     * @name merchello.models.extendedDataDisplayBuilder
     *
     * @description
     * A utility service that builds ExtendedDataBuilder models
     */
    angular.module('merchello.models')
        .factory('extendedDataDisplayBuilder',
        ['genericModelBuilder', 'ExtendedDataDisplay',
            function(genericModelBuilder, ExtendedDataDisplay) {

                var Constructor = ExtendedDataDisplay;
                var extendedDataItem = function() {
                    this.key = '';
                    this.value = '';
                };

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var extendedData = new Constructor();
                        extendedData.items = genericModelBuilder.transform(jsonResult, extendedDataItem);
                        return extendedData;
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.gatewayResourceDisplayBuilder
     *
     * @description
     * A utility service that builds GatewayResourceDisplay models
     */
    angular.module('merchello.models')
        .factory('gatewayResourceDisplayBuilder',
        ['genericModelBuilder', 'GatewayResourceDisplay',
            function(genericModelBuilder, GatewayResourceDisplay) {
                var Constructor = GatewayResourceDisplay;
                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);


    /**
     * @ngdoc service
     * @name merchello.models.appliedPaymentDisplayBuilder
     *
     * @description
     * A utility service that builds applieddPaymentDisplaybuilder
     */
    angular.module('merchello.models')
        .factory('appliedPaymentDisplayBuilder',
        ['genericModelBuilder', 'AppliedPaymentDisplay',
            function(genericModelBuilder, AppliedPaymentDisplay) {

                var Constructor = AppliedPaymentDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.paymentDisplayBuilder
     *
     * @description
     * A utility service that builds PaymentDisplay models
     */
    angular.module('merchello.models')
        .factory('paymentDisplayBuilder',
        ['genericModelBuilder', 'appliedPaymentDisplayBuilder', 'extendedDataDisplayBuilder', 'PaymentDisplay',
            function(genericModelBuilder, appliedPaymentDisplayBuilder, extendedDataDisplayBuilder, PaymentDisplay) {

                var Constructor = PaymentDisplay;

                return {
                    createDefault: function() {
                        var payment = new Constructor();
                        payment.extendedData = extendedDataDisplayBuilder.createDefault();
                        return payment;
                    },
                    transform: function(jsonResult) {
                        var payment = genericModelBuilder.transform(jsonResult, Constructor);
                        payment.appliedPayments = appliedPaymentDisplayBuilder.transform(jsonResult.appliedPayments);
                        payment.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                        return payment;
                    }
                };
            }]);
    /**
     * @ngdoc service
     * @name merchello.models.provinceDisplayBuilder
     *
     * @description
     * A utility service that builds ProvinceDisplay models
     */
    angular.module('merchello.models')
        .factory('provinceDisplayBuilder',
        ['genericModelBuilder', 'ProvinceDisplay',
            function(genericModelBuilder, ProvinceDisplay) {
                var Constructor = ProvinceDisplay;
                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.services.queryDisplayBuilder
     *
     * @description
     * A utility service that builds QueryDisplayModels models
     *
     */
    angular.module('merchello.models')
        .factory('queryDisplayBuilder',
        ['genericModelBuilder', 'QueryDisplay',
            function(genericModelBuilder, QueryDisplay) {
            var Constructor = QueryDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);


    /**
     * @ngdoc service
     * @name merchello.services.queryParameterDisplayBuilder
     *
     * @description
     * A utility service that builds QueryParameterDisplayModels models
     *
     */
    angular.module('merchello.models')
        .factory('queryParameterDisplayBuilder',
            ['genericModelBuilder', 'QueryParameterDisplay',
            function(genericModelBuilder, QueryParameterDisplay) {
            var Constructor = QueryParameterDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);

    /**
     * @ngdoc service
     * @name merchello.services.queryResultDisplayBuilder
     *
     * @description
     * A utility service that builds QueryResultDisplayModels models
     */
    angular.module('merchello.models')
        .factory('queryResultDisplayBuilder',
        ['genericModelBuilder', 'QueryResultDisplay',
            function(genericModelBuilder, QueryResultDisplay) {
            var Constructor = QueryResultDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function (jsonResult, itemBuilder) {
                    // this is slightly different than other builders in that there can only ever be a single
                    // QueryResult returned from the WebApiController, so we iterate through the items
                    var result = genericModelBuilder.transform(jsonResult, Constructor);
                    if (itemBuilder !== undefined)
                    {
                        result.items = [];
                        result.items = itemBuilder.transform(jsonResult.items);
                    }
                    return result;
                }
            };
        }]);

    /**
     * @ngdoc service
     * @name merchello.models.invoiceDisplayBuilder
     *
     * @description
     * A utility service that builds InvoiceDisplay models
     */
    angular.module('merchello.models')
        .factory('invoiceDisplayBuilder',
        ['genericModelBuilder', 'invoiceStatusDisplayBuilder', 'invoiceLineItemDisplayBuilder',
            'orderDisplayBuilder', 'InvoiceDisplay',
            function(genericModelBuilder, invoiceStatusDisplayBuilder, invoiceLineItemDisplayBuilder,
                     orderDisplayBuilder, InvoiceDisplay) {
                var Constructor = InvoiceDisplay;

                return {
                    createDefault: function() {
                        var invoice = new Constructor();
                        invoice.invoiceStatus = invoiceStatusDisplayBuilder.createDefault();
                        return invoice;
                    },
                    transform: function(jsonResult) {
                        var invoices = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < invoices.length; i++) {
                            invoices[ i ].invoiceStatus = invoiceStatusDisplayBuilder.transform(jsonResult[ i ].invoiceStatus);
                            invoices[ i ].items = invoiceLineItemDisplayBuilder.transform(jsonResult[ i ].items);
                            invoices[ i ].orders = orderDisplayBuilder.transform(jsonResult[ i ].orders);
                        }
                        return invoices;
                    }
                };
            }]);
    /**
     * @ngdoc service
     * @name merchello.models.invoiceLineItemDisplayBuilder
     *
     * @description
     * A utility service that builds InvoiceLineItemDisplay models
     */
    angular.module('merchello.models')
        .factory('invoiceLineItemDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'typeFieldDisplayBuilder', 'InvoiceLineItemDisplay',
            function(genericModelBuilder, extendedDataDisplayBuilder, typeFieldDisplayBuilder, InvoiceLineItemDisplay) {
                var Constructor = InvoiceLineItemDisplay;
                return {
                    createDefault: function() {
                        var invoiceLineItem = new Constructor();
                        invoiceLineItem.lineItemTypeField = typeFieldDisplayBuilder.createDefault();
                        invoiceLineItem.extendedData = extendedDataDisplayBuilder.createDefault();
                        return invoiceLineItem;
                    },
                    transform: function(jsonResult) {
                        var invoiceLineItems = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < invoiceLineItems.length; i++) {
                            invoiceLineItems[ i ].extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                            invoiceLineItems[ i ].lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].lineItemTypeField);
                        }
                        return invoiceLineItems;
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.invoiceStatusDisplayBuilder
     *
     * @description
     * A utility service that builds InvoiceStatusDisplay models
     */
    angular.module('merchello.models')
        .factory('invoiceStatusDisplayBuilder',
        ['genericModelBuilder', 'InvoiceStatusDisplay',
            function(genericModelBuilder, InvoiceStatusDisplay) {
                var Constructor = InvoiceStatusDisplay;
                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.gatewayResourceDisplayBuilder
     *
     * @description
     * A utility service that builds GatewayResourceDisplay models
     */
    angular.module('merchello.models')
        .factory('orderDisplayBuilder',
        ['genericModelBuilder', 'orderStatusDisplayBuilder', 'orderLineItemDisplayBuilder', 'OrderDisplay',
            function(genericModelBuilder, orderStatusDisplayBuilder, orderLineItemDisplayBuilder, OrderDisplay) {
                var Constructor = OrderDisplay;

                return {
                    createDefault: function() {
                        var order = new Constructor();
                        order.orderStatus = orderStatusDisplayBuilder.createDefault();
                        return order;
                    },
                    transform: function(jsonResult) {
                        var orders = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < orders.length; i++) {
                            orders[ i ].orderStatus = orderStatusDisplayBuilder.transform(jsonResult[ i ].orderStatus);
                            orders[ i ].items = orderLineItemDisplayBuilder.transform(jsonResult[ i ].items);
                        }
                        return orders;
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.orderLineItemDisplayBuilder
     *
     * @description
     * A utility service that builds OrderLineItemDisplay models
     */
    angular.module('merchello.models')
        .factory('orderLineItemDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'typeFieldDisplayBuilder', 'OrderLineItemDisplay',
            function(genericModelBuilder, extendedDataDisplayBuilder, typeFieldDisplayBuilder, OrderLineItemDisplay) {
                var Constructor = OrderLineItemDisplay;
                return {
                    createDefault: function() {
                        var orderLineItem = new Constructor();
                        orderLineItem.extendedData = extendedDataDisplayBuilder.createDefault();
                        orderLineItem.lineItemTypeField = typeFieldDisplayBuilder.createDefault();
                        return orderLineItem;
                    },
                    transform: function(jsonResult) {
                        var orderLineItems = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < orderLineItems.length; i++) {
                            orderLineItems[ i ].extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                            orderLineItems[ i ].lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].lineItemTypeField);
                        }
                        return orderLineItems;
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.orderStatusDisplayBuilder
     *
     * @description
     * A utility service that builds OrderStatusDisplay models
     */
    angular.module('merchello.models')
        .factory('orderStatusDisplayBuilder',
        ['genericModelBuilder', 'OrderStatusDisplay',
            function(genericModelBuilder, OrderStatusDisplay) {

                var Constructor = OrderStatusDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.salesHistoryMessageDisplayBuilder
     *
     * @description
     * A utility service that builds salesHistoryMessageDisplayBuilder models
     */
    angular.module('merchello.models')
        .factory('salesHistoryMessageDisplayBuilder',
        ['genericModelBuilder', 'SalesHistoryMessageDisplay',
            function(genericModelBuilder, SalesHistoryMessageDisplay) {

                var Constructor = SalesHistoryMessageDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.auditLogDisplayBuilder
     *
     * @description
     * A utility service that builds auditLogDisplayBuilder models
     */
    angular.module('merchello.models')
        .factory('auditLogDisplayBuilder',
            ['genericModelBuilder', 'salesHistoryMessageDisplayBuilder', 'extendedDataDisplayBuilder', 'AuditLogDisplay',
            function(genericModelBuilder, salesHistoryMessageDisplayBuilder, extendedDataDisplayBuilder, AuditLogDisplay) {

                var Constructor = AuditLogDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var auditLogDisplay = genericModelBuilder.transform(jsonResult, Constructor);
                        auditLogDisplay.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);

                        // this is a bit brittle - and we should look at the construction of this in the ApiController
                        var message = JSON.parse(jsonResult.message);
                        auditLogDisplay.message = salesHistoryMessageDisplayBuilder.transform(message);
                        return auditLogDisplay;
                    }
                };
        }]);
    /**
     * @ngdoc service
     * @name merchello.models.auditLogDisplayBuilder
     *
     * @description
     * A utility service that builds auditLogDisplayBuilder models
     */
    angular.module('merchello.models')
        .factory('dailyAuditLogDisplayBuilder',
            ['genericModelBuilder', 'auditLogDisplayBuilder', 'DailyAuditLogDisplay',
            function(genericModelBuilder, auditLogDisplayBuilder, DailyAuditLogDisplay) {

                var Constructor = DailyAuditLogDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var dailyLog = genericModelBuilder.transform(jsonResult, Constructor);
                        var logs = [];
                        angular.forEach(dailyLog.logs, function(log) {
                            logs.push(auditLogDisplayBuilder.transform(log));
                        });
                        dailyLog.logs = logs;
                        return dailyLog;
                    }
                };
        }]);
    /**
     * @ngdoc service
     * @name merchello.models.salesHistoryDisplayBuilder
     *
     * @description
     * A utility service that builds salesHistoryDisplayBuilder models
     */
    angular.module('merchello.models')
        .factory('salesHistoryDisplayBuilder',
            ['genericModelBuilder', 'dailyAuditLogDisplayBuilder', 'SalesHistoryDisplay',
            function(genericModelBuilder, dailyAuditLogDisplayBuilder, SalesHistoryDisplay) {

                var Constructor = SalesHistoryDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var history = this.createDefault();
                        angular.forEach(jsonResult.dailyLogs, function(result) {
                            history.addDailyLog(dailyAuditLogDisplayBuilder.transform(result));
                        });
                        return history;
                    }
                };
        }]);
    /**
     * @ngdoc service
     * @name merchello.models.settingDisplayBuilder
     *
     * @description
     * A utility service that builds SettingDisplay models
     */
    angular.module('merchello.models')
        .factory('settingDisplayBuilder',
        ['genericModelBuilder', 'SettingDisplay',
            function(genericModelBuilder, SettingDisplay) {

                var Constructor = SettingDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.shipmentDisplayBuilder
     *
     * @description
     * A utility service that builds ShipmentDisplay models
     */
    angular.module('merchello.models')
        .factory('shipmentDisplayBuilder',
        ['genericModelBuilder',  'shipmentStatusDisplayBuilder', 'orderLineItemDisplayBuilder', 'ShipmentDisplay', 'ShipmentStatusDisplay',
            function(genericModelBuilder, shipmentStatusBuilder, orderLineItemBuilder, ShipmentDisplay, ShipmentStatusDisplay) {

                var Constructor = ShipmentDisplay;

                return {
                    // TODO the default warehouse address (AddressDisplay) could be saved as a config value
                    // and then added to the shipment origin address
                    createDefault: function() {
                        var shipment = new Constructor();
                        shipment.shipmentStatus = shipmentStatusBuilder.createDefault();
                        return shipment;
                    },
                    transform: function(jsonResult) {
                        // the possible list of shipments
                        var shipments = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < jsonResult.length; i++) {
                            // each shipment has a ShipmentStatusDisplay
                            shipments[ i ].shipmentStatus = shipmentStatusBuilder.transform(jsonResult[ i ].shipmentStatus, ShipmentStatusDisplay);
                            // add the OrderLineItemDisplay(s) associated with the shipment
                            shipments[ i ].items = orderLineItemBuilder.transform(jsonResult[ i ].items);
                        }
                        return shipments;
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.shipmentStatusDisplayBuilder
     *
     * @description
     * A utility service that builds ShipmentStatusDisplay models
     */
    angular.module('merchello.models')
    .factory('shipmentStatusDisplayBuilder',
    ['genericModelBuilder', 'ShipmentStatusDisplay',
        function(genericModelBuilder, ShipmentStatusDisplay) {

            var Constructor = ShipmentStatusDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);

    /**
     * @ngdoc service
     * @name merchello.models.typeFieldDisplayBuilder
     *
     * @description
     * A utility service that builds TypeFieldDisplay models
     */
    angular.module('merchello.models')
    .factory('typeFieldDisplayBuilder',
    ['genericModelBuilder', 'TypeFieldDisplay',
        function(genericModelBuilder, TypeFieldDisplay) {

            var Constructor = TypeFieldDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);


})();