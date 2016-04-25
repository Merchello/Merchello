var PayPalProviderSettings = function() {
    var self = this;
    self.clientId = '';
    self.clientSecret = '';
    self.mode = 'Sandbox';
};

angular.module('merchello.providers.models').constant('PayPalProviderSettings', PayPalProviderSettings);
