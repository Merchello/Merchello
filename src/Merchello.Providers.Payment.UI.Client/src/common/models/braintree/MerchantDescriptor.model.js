var MerchantDescriptor = function() {
    var self = this;
    self.name = '';
    self.url = '';
    self.phone = '';
};


angular.module('merchello.payments.models').constant('MerchantDescriptor', MerchantDescriptor);
