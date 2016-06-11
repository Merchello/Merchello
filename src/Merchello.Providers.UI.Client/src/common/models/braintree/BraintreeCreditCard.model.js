var BraintreeCreditCard = function() {
    var self = this;
    self.cardholderName = '';
    self.number = '';
    self.cvv = '';
    self.expirationMonth = '';
    self.expirationYear = '';
    self.billingAddress = {
        postalCode: ''
    };
};

angular.module('merchello.providers.models').constant('BraintreeCreditCard', BraintreeCreditCard);
