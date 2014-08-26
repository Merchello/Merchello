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
			self.quantity = 0;
			self.price = 0.0;
			self.exported = false;
			self.backOrder = false;
			self.extendedData = [];
			self.productVariant = {};
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
			self.extendedData = data.extendedData;
			self.productVariant = {};
		}

		self.getProductVariantKey = function () {
			var variantKey = '';
			if (self.extendedData.length > 0) {
				variantKey = _.find(self.extendedData, function(extDataItem) {
					return extDataItem['key'] == "merchProductVariantKey";
				});
			}
			if (variantKey == undefined) {
				variantKey = '';
			}
			return variantKey;
		};

		self.setProductVariant = function(variant) {
			self.productVariant = variant;
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
		self.lineItemType = new merchello.Models.TypeField();
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
			self.payments = [];
			self.appliedPayments = [];
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
			self.payments = [];
			self.appliedPayments = [];
		}

		self.getPaymentStatus = function () {
			return self.invoiceStatus.name;
		};

		self.getFulfillmentStatus = function () {

			if (!_.isEmpty(self.orders)) {
				return self.orders[0].orderStatus.name;
			}

			return "";
		};

		self.getProductLineItems = function () {
			return _.filter(self.items, function (item) { return item.lineItemType.alias == "Product"; });
		};

		self.getTaxLineItem = function () {
			return _.find(self.items, function (item) { return item.lineItemType.alias == "Tax"; });
		};

		self.getShippingLineItem = function () {
			return _.find(self.items, function (item) { return item.lineItemType.alias == "Shipping"; });
		};

	};

	models.Payment = function (data) {

		var self = this;

		if (data == undefined) {
			self.key = "";
			self.customerKey = "";
			self.paymentMethodKey = "";
			self.paymentTypeFieldKey = "";
			self.paymentMethodName = "";
			self.referenceNumber = "";
			self.amount = 0.0;
			self.authorized = false;
			self.collected = false;
			self.exported = false;
			self.appliedPayments = [];
		} else {
			self.key = data.key;
			self.customerKey = data.customerKey;
			self.paymentMethodKey = data.paymentMethodKey;
			self.paymentTypeFieldKey = data.paymentTypeFieldKey;
			self.paymentMethodName = data.paymentMethodName;
			self.referenceNumber = data.referenceNumber;
			self.amount = data.amount;
			self.authorized = data.authorized;
			self.collected = data.collected;
			self.exported = data.exported;
			self.appliedPayments = _.map(data.appliedPayments, function (appliedPayment) {
				return new merchello.Models.AppliedPayment(appliedPayment);
			});
		}
		self.paymentType = new merchello.Models.TypeField();

		self.getStatus = function () {
			var statusArr = [];
			if (self.authorized) {
				statusArr.push("Authorized");
			}
			if (self.collected) {
				statusArr.push("Captured");
			}
			if (self.exported) {
				statusArr.push("Exported");
			}

			return statusArr.join("/");
		};

		self.hasAmount = function () {
			return self.amount > 0;
		};
	};

	models.AppliedPayment = function (data) {

		var self = this;

		if (data == undefined) {
			self.key = "";
			self.paymentKey = "";
			self.invoiceKey = "";
			self.appliedPaymentTfKey = "";
			self.description = "";
			self.createDate = "";
			self.amount = 0.0;
			self.exported = false;
			self.invoice = {};
			self.payment = {};
		} else {
			self.key = data.key;
			self.paymentKey = data.paymentKey;
			self.invoiceKey = data.invoiceKey;
			self.appliedPaymentTfKey = data.appliedPaymentTfKey;
			self.description = data.description;
			self.createDate = data.createDate;
			self.amount = data.amount;
			self.exported = data.exported;
			self.invoice = new merchello.Models.Invoice(data.invoice);
			self.payment = new merchello.Models.Payment(data.payment);
		}

		self.hasAmount = function() {
			return self.amount > 0;
		};
	};

	models.PaymentRequest = function (data) {

		var self = this;

		if (data == undefined) {
			self.invoiceKey = "";
			self.paymentKey = "";
			self.paymentMethodKey = "";
			self.amount = 0.0;
			self.processorArgs = [];
		} else {
			self.invoiceKey = data.invoiceKey;
			self.paymentKey = data.paymentKey;
			self.paymentMethodKey = data.paymentMethodKey;
			self.amount = data.amount;
			self.processorArgs = data.processorArgs;
		}
	};

	models.Shipment = function (data) {

		var self = this;

		if (data == undefined) {
			self.key = "";
			self.versionKey = "";
			self.fromOrganization = "";
			self.fromName = "";
			self.fromAddress1 = "";
			self.fromAddress2 = "";
			self.fromLocality = "";
			self.fromRegion = "";
			self.fromPostalCode = "";
			self.fromCountryCode = "";
			self.fromIsCommercial = "";
			self.toOrganization = "";
			self.toName = "";
			self.toAddress1 = "";
			self.toAddress2 = "";
			self.toLocality = "";
			self.toRegion = "";
			self.toPostalCode = "";
			self.toCountryCode = "";
			self.toIsCommercial = "";
			self.shipMethodKey = "";
			self.phone = "";
			self.email = "";
			self.carrier = "";
			self.trackingCode = "";
			self.shippedDate = "";
			self.items = [];
		} else {
			self.key = data.key;
			self.versionKey = data.versionKey;
			self.fromOrganization = data.fromOrganization;
			self.fromName = data.fromName;
			self.fromAddress1 = data.fromAddress1;
			self.fromAddress2 = data.fromAddress2;
			self.fromLocality = data.fromLocality;
			self.fromRegion = data.fromRegion;
			self.fromPostalCode = data.fromPostalCode;
			self.fromCountryCode = data.fromCountryCode;
			self.fromIsCommercial = data.fromIsCommercial;
			self.toOrganization = data.toOrganization;
			self.toName = data.toName;
			self.toAddress1 = data.toAddress1;
			self.toAddress2 = data.toAddress2;
			self.toLocality = data.toLocality;
			self.toRegion = data.toRegion;
			self.toPostalCode = data.toPostalCode;
			self.toCountryCode = data.toCountryCode;
			self.toIsCommercial = data.toIsCommercial;
			self.shipMethodKey = data.shipMethodKey;
			self.phone = data.phone;
			self.email = data.email;
			self.carrier = data.carrier;
			self.trackingCode = data.trackingCode;
			self.shippedDate = data.shippedDate;
			self.items = _.map(data.items, function (lineitem) {
				return new merchello.Models.OrderLineItem(lineitem);
			});
		}
	};

	models.OrderSummary = function (data) {

	    var self = this;

	    if (data == undefined) {
	        self.itemTotal = 0;
	        self.invoiceTotal = 0;
	        self.shippingTotal = 0;
	        self.taxTotal = 0;
	        self.orderPrepComplete = false;
	    } else {
	        self.itemTotal = data.itemTotal;
	        self.invoiceTotal = data.invoiceTotal;
	        self.shippingTotal = data.shippingTotal;
	        self.taxTotal = data.taxTotal;
	        self.orderPrepComplete = data.orderPrepComplete;
	    }
	};


}(window.merchello.Models = window.merchello.Models || {}));