(function (models, undefined) {

    models.LineItem = function (data) {

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
            self.lineItemType = "";
            self.totalPrice = "";
        } else {
            self.key = data.key;
            self.containerKey = data.containerKey;
            self.lineItemTfKey = data.lineItemTfKey;
            self.sku = data.sku;
            self.name = data.name;
            self.quantity = data.quantity;
            self.price = data.price;
            self.exported = data.exported;
            self.lineItemType = data.lineItemType;
            self.totalPrice = data.totalPrice;
        }
    };

    models.OrderLineItem = function (data) {

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
            self.lineItemType = "";
            self.totalPrice = "";
            self.shipmentKey = "";
            self.backOrder = "";
        } else {
            self.key = data.key;
            self.containerKey = data.containerKey;
            self.lineItemTfKey = data.lineItemTfKey;
            self.sku = data.sku;
            self.name = data.name;
            self.quantity = data.quantity;
            self.price = data.price;
            self.exported = data.exported;
            self.lineItemType = data.lineItemType;
            self.totalPrice = data.totalPrice;
            self.shipmentKey = data.shipmentKey;
            self.backOrder = data.backOrder;
        }
    };

    models.Order = function (data) {

        var self = this;

        if (data == undefined) {
            self.key = "";
            self.invoiceKey = "";
            self.orderNumberPrefix = "";
            self.orderNumber = "";
            self.orderDate = "";
            self.orderStatusKey = "";
            self.exported = "";
            self.items = [];
        } else {
            self.key = data.key;
            self.invoiceKey = data.invoiceKey;
            self.orderNumberPrefix = data.orderNumberPrefix;
            self.orderNumber = data.orderNumber;
            self.orderDate = data.orderDate;
            self.orderStatusKey = data.orderStatusKey;
            self.exported = data.exported;
            self.items = _.map(data.items, function (lineitem) {
                return new merchello.Models.OrderLineItem(lineitem);
            });
        }
    };

    models.Invoice = function (data) {

        var self = this;

        if (data == undefined) {
            self.key = "";
            self.customerKey = "";
            self.invoiceNumberPrefix = "";
            self.invoiceNumber = "";
            self.invoiceDate = "";
            self.invoiceStatusKey = "";
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
            self.total = "";
            self.items = [];
            self.orders = [];
        } else {
            self.key = data.key;
            self.customerKey = data.customerKey;
            self.invoiceNumberPrefix = data.invoiceNumberPrefix;
            self.invoiceNumber = data.invoiceNumber;
            self.invoiceDate = data.invoiceDate;
            self.invoiceStatusKey = data.invoiceStatusKey;
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
                return new merchello.Models.LineItem(lineitem);
            });
            self.orders = _.map(data.orders, function (order) {
                return new merchello.Models.Order(order);
            });
        }

        self.getPaymentStatus = function () {
            switch (self.invoiceStatusKey) {
                case 1:
                    return "Authorized";

                default:
                    return "Paid";
            }
        };

        self.getFulfillmentStatus = function () {

            if(!_.isEmpty(self.orders)) {
                switch (self.orders[0].orderStatusKey) {
                    case 1:
                        return "Not Fulfilled";
                    case 2:
                        return "Partial";
                    default:
                        return "Fulfilled";
                }                
            }

        };

    };


}(window.merchello.Models = window.merchello.Models || {}));