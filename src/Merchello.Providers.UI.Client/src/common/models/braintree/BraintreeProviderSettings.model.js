var BraintreeProviderSettings = function() {
    var self = this;
    self.environment = '';
    self.publicKey = '';
    self.privateKey = '';
    self.merchantId = '';
    self.merchantDescriptor = {};
    self.defaultTransactionOption = '';
};

angular.module('merchello.providers.models').constant('BraintreeProviderSettings', BraintreeProviderSettings);
