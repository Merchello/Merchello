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
    var PaymentRequestDisplay = function() {
        var self = this;
        self.invoiceKey = '';
        self.paymentKey = '';
        self.paymentMethodKey = '';
        self.amount = 0.0;
        self.processorArgs = [];
    };

    angular.module('merchello.models').constant('PaymentRequestDisplay', PaymentRequestDisplay);
