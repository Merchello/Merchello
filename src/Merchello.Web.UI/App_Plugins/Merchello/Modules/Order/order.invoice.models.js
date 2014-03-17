(function (models, undefined) {

    models.OrderLineItem = function (data) {

        var self = this;

        if (data == undefined) {
            self.key = "";
            self.containerKey = "";
            self.shipmentKey = "";
            self.lineItemTfKey = "";
            self.sku = "";
            self.name = "";
            self.quantity = "";
            self.price = "";
            self.exported = "";
            self.backOrder = "";
        } else {
            self.key = data.key;
            self.containerKey = data.containerKey;
            self.shipmentKey = data.shipmentKey;
            self.lineItemTfKey = data.lineItemTfKey;
            self.sku = data.sku;
            self.name = data.name;
            self.quantity = data.quantity;
            self.price = data.price;
            self.exported = data.exported;
            self.backOrder = data.backOrder;
        }
    };

    models.OrderStatus = function (data) {

        var self = this;

        if (data == undefined) {
            self.key = "";
            self.name = "";
            self.alias = "";
            self.reportable = "";
            self.active = "";
            self.sortOrder = "";
        } else {
            self.key = data.key;
            self.name = data.name;
            self.alias = data.alias;
            self.reportable = data.reportable;
            self.active = data.active;
            self.sortOrder = data.sortOrder;
        }
    };

    models.Order = function (data) {

        var self = this;

        if (data == undefined) {
            self.key = "";
            self.versionKey = "";
            self.invoiceKey = "";
            self.orderNumberPrefix = "";
            self.orderNumber = "";
            self.orderDate = "";
            self.orderStatusKey = "";
            self.orderStatus = new merchello.Models.OrderStatus();
            self.exported = "";
            self.items = [];
        } else {
            self.key = data.key;
            self.versionKey = data.versionKey;
            self.invoiceKey = data.invoiceKey;
            self.orderNumberPrefix = data.orderNumberPrefix;
            self.orderNumber = data.orderNumber;
            self.orderDate = data.orderDate;
            self.orderStatusKey = data.orderStatusKey;
            self.orderStatus = new merchello.Models.OrderStatus(data.orderStatus);
            self.exported = data.exported;
            self.items = _.map(data.items, function (lineitem) {
                return new merchello.Models.OrderLineItem(lineitem);
            });
        }
    };

    models.InvoiceLineItem = function (data) {

        var self = this;

        if (data == undefined) {
            self.key = "";
            self.containerKey = "";
            self.lineItemTfKey = "";
            self.sku = "";
            self.name = "";
            self.quantity = "";
            self.price = "";
            self.exported = "";
        } else {
            self.key = data.key;
            self.containerKey = data.containerKey;
            self.lineItemTfKey = data.lineItemTfKey;
            self.sku = data.sku;
            self.name = data.name;
            self.quantity = data.quantity;
            self.price = data.price;
            self.exported = data.exported;
        }
    };

    models.InvoiceStatus = function (data) {

        var self = this;

        if (data == undefined) {
            self.key = "";
            self.name = "";
            self.alias = "";
            self.reportable = "";
            self.active = "";
            self.sortOrder = "";
        } else {
            self.key = data.key;
            self.name = data.name;
            self.alias = data.alias;
            self.reportable = data.reportable;
            self.active = data.active;
            self.sortOrder = data.sortOrder;
        }
    };

    models.Invoice = function (data) {

        var self = this;

        if (data == undefined) {
            self.key = "";
            self.versionKey = "";
            self.customerKey = "";
            self.invoiceNumberPrefix = "";
            self.invoiceNumber = "";
            self.invoiceDate = "";
            self.invoiceStatusKey = "";
            self.invoiceStatus = new merchello.Models.InvoiceStatus();
            self.billToName = "";
            self.billToAddress1 = "";
            self.billToAddress2 = "";
            self.billToLocality = "";
            self.billToRegion = "";
            self.billToPostalCode = "";
            self.billToCountryCode = "";
            self.billToEmail = "";
            self.billToPhone = "";
            self.billToCompany = "";
            self.exported = "";
            self.archived = "";
            self.total = 0.0;
            self.items = [];
            self.orders = [];
        } else {
            self.key = data.key;
            self.versionKey = data.versionKey;
            self.customerKey = data.customerKey;
            self.invoiceNumberPrefix = data.invoiceNumberPrefix;
            self.invoiceNumber = data.invoiceNumber;
            self.invoiceDate = data.invoiceDate;
            self.invoiceStatusKey = data.invoiceStatusKey;
            self.invoiceStatus = new merchello.Models.InvoiceStatus(data.invoiceStatus);
            self.billToName = data.billToName;
            self.billToAddress1 = data.billToAddress1;
            self.billToAddress2 = data.billToAddress2;
            self.billToLocality = data.billToLocality;
            self.billToRegion = data.billToRegion;
            self.billToPostalCode = data.billToPostalCode;
            self.billToCountryCode = data.billToCountryCode;
            self.billToEmail = data.billToEmail;
            self.billToPhone = data.billToPhone;
            self.billToCompany = data.billToCompany;
            self.exported = data.exported;
            self.archived = data.archived;
            self.total = data.total;
            self.items = _.map(data.items, function (lineitem) {
                return new merchello.Models.InvoiceLineItem(lineitem);
            });
            self.orders = _.map(data.orders, function (order) {
                return new merchello.Models.Order(order);
            });
        }

        self.getPaymentStatus = function () {
            return self.invoiceStatus.name;
        };

        self.getFulfillmentStatus = function () {

            if(!_.isEmpty(self.orders)) {
                return self.orders[0].orderStatus.name;
            }

            return "";
        };

    };


}(window.merchello.Models = window.merchello.Models || {}));