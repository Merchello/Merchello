    /**
     * @ngdoc model
     * @name PaymentGatewayProviderDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's PaymentGatewayProviderDisplay object
     */
    var PaymentGatewayProviderDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.providerTfKey = '';
        self.description = '';
        self.extendedData = {};
        self.gatewayResource = {};
        self.encryptExtendedData = false;
        self.activated = false;
        self.gatewayResources = [];
        self.paymentMethods = [];
        self.dialogEditorView = {};
    };

    angular.module('merchello.models').constant('PaymentGatewayProviderDisplay', PaymentGatewayProviderDisplay);
