    /**
     * @ngdoc model
     * @name PaymentRequestDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's PaymentRequestDisplay object
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