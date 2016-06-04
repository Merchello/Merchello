var PayPalProviderSettings = function() {
    var self = this;
    self.clientId = '';
    self.clientSecret = '';
    self.mode = 'Sandbox';
    self.apiUsername = '';
    self.apiPassword = '';
    self.apiSignature = '';
    self.applicationId = '';
    self.successUrl = '';
    self.retryUrl = '';
    self.cancelUrl = '';
    self.deleteInvoiceOnCancel = false;
};

angular.module('merchello.providers.models').constant('PayPalProviderSettings', PayPalProviderSettings);
