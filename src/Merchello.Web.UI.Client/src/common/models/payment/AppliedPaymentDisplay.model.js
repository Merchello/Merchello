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
