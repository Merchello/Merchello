    /**
     * @ngdoc model
     * @name AddPaymentDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for adding payments to a sale.
     */
    var AddPaymentDialogData = function() {
        var self = this;
        self.paymentMethod = {};
        self.paymentMethodName = '';
        self.invoice = {};
        self.authorizePaymentOnly = false;
        self.invoiceBalance = 0;
        self.amount = 0;
        self.currencySymbol = '';
        self.showSpinner = function() { return true; };
        self.processorArgs = new ProcessorArgumentCollectionDisplay();
    };

    AddPaymentDialogData.prototype = (function() {

        function asPaymentRequestDisplay() {
            var request = new PaymentRequestDisplay();
            request.invoiceKey = this.invoice.key;
            request.paymentMethodKey = this.paymentMethod.key;
            request.amount = this.amount;
            request.processorArgs = this.processorArgs.toArray();
            return request;
        }

        return {
            asPaymentRequestDisplay: asPaymentRequestDisplay
        };
    }());

    angular.module('merchello.models').constant('AddPaymentDialogData', AddPaymentDialogData);
