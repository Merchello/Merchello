    /**
     * @ngdoc model
     * @name PaymentMethodDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's PaymentMethodDisplay object
     */
    var PaymentMethodDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.providerKey = '';
        self.description = '';
        self.paymentCode = '';
        self.dialogEditorView = {};
        self.authorizeCapturePaymentEditorView = {};
        self.voidPaymentEditorView = {};
        self.refundPaymentEditorView = {};
    };

    angular.module('merchello.models').constant('PaymentMethodDisplay', PaymentMethodDisplay);