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
