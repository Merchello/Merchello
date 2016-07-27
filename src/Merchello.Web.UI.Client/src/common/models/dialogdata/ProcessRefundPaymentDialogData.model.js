    /**
     * @ngdoc model
     * @name ProcessRefundPaymentDialogData
     * @function
     *
     * @description
     * Dialog data model for refunding payments
     */
    var ProcessRefundPaymentDialogData = function() {
        var self = this;
        self.invoiceKey = '';
        self.paymentMethodKey = '';
        self.paymentKey = '';
        self.amount = 0;
        self.currencySymbol = '';
        self.paymentMethodName = '';
        self.appliedAmount = 0;
        self.processorArgumentCollectionDisplay = new ProcessorArgumentCollectionDisplay();
        self.warning = '';
    };

    ProcessRefundPaymentDialogData.prototype = (function() {
        function toPaymentRequestDisplay() {
            var paymentRequest = angular.extend(this, PaymentRequestDisplay);
            paymentRequest.processorArgs = this.processorArgumentCollectionDisplay.toArray();
            return paymentRequest;
        }

        return {
            toPaymentRequestDisplay: toPaymentRequestDisplay
        };
    }());

    angular.module('merchello.models').constant('ProcessRefundPaymentDialogData', ProcessRefundPaymentDialogData);