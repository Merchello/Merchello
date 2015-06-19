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
        self.authorizePaymentEditorView = {};
        self.authorizeCapturePaymentEditorView = {};
        self.voidPaymentEditorView = {};
        self.refundPaymentEditorView = {};
        self.capturePaymentEditorView = {};
        self.includeInPaymentSelection = true;
        self.requiresCustomer = false;
    };

    angular.module('merchello.models').constant('PaymentMethodDisplay', PaymentMethodDisplay);