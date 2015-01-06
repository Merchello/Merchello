(function (models, undefined) {

    models.DailyLog = function(data) {

        var self = this;

        if (data === undefined) {
            self.day = '';
            self.logs = [];
        } else {
            self.day = data.day;
            self.logs = _.map(data.logs, function(log) {
                return new merchello.Models.LogEntry(log);
            });
        }
    };

    models.LogEntry = function(data) {

        var self = this;

        if (data === undefined) {
            self.entityKey = '';
            self.entityTfKey = '';
            self.entityType = '';
            self.extendedData = [];
            self.isError = false;
            self.key = '';
            self.message = {};
            self.recordDate = '';
            self.verbosity = '';
        } else {
            self.entityKey = data.entityKey;
            self.entityTfKey = data.entityTfKey;
            self.entityType = data.entityType;
            self.extendedData = _.map(data.extendedData, function(item) {
                return item;
            });
            self.isError = data.isError;
            self.key = data.key;
            if (typeof data.message === "string") {
                self.message = new merchello.Models.LogMessage(JSON.parse(data.message));
            } else if (typeof data.message === "object") {
                self.message = new merchello.Models.LogMessage(data.message);
            } else {
                self.message = new merchello.Models.LogMessage();
            }
            self.recordDate = data.recordDate;
            self.verbosity = data.verbosity;
        }
    };

    models.LogMessage = function(data) {

        // LogMessage is a dynamic model that has conditional parameters depending on the type of message.

        // All LogMessages must have two parameters, however.
        // area: this is the area in the language files that the specific lang key will be found
        // key: this is the lang key for localization

        // Additionally, it has zero or more parameters specific to the message as values that get plugged in dynamically to the message.

        var self = this;

        if (data === undefined) {
            self.area = '';
            self.key = '';
        } else {
            self.area = data.area;
            self.key = data.key;
            switch (self.key) {
                case 'invoiceCreated':
                case 'invoiceDeleted':
                case 'orderCreated':
                    self.invoiceNumber = data.invoiceNumber;
                    break;
                case 'orderDeleted':
                    self.orderNumber = data.orderNumber;
                    break;
                case 'shipmentCreated':
                    self.itemCount = data.itemCount;
                    break;
                case 'paymentAuthorize':
                case 'paymentCaptured':
                    self.invoiceTotal = data.invoiceTotal;
                    self.currencyCode = data.currencyCode;
                    break;
                case 'paymentRefunded':
                    self.refundAmont = data.refundAmount;
                    self.currencyCode = data.currencyCode;
                    break;
                default:
                    break;
            }
        }

    };


}(window.merchello.Models = window.merchello.Models || {}));