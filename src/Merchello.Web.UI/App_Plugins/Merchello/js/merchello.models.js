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
        self.extendedData = [];
        self.isError = false;
        self.key = '';
        self.message = {};
        self.recordDate = '';
        self.verbosity = '';
    }

    angular.module('merchello.models').constant('AuditLogDisplay', AuditLogDisplay);
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

})();