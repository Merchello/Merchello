var MerchantDescriptor = function() {
    var self = this;
    self.name = '';
    self.url = '';
    self.phone = '';
};


angular.module('merchello.providers.models').constant('MerchantDescriptor', MerchantDescriptor);
