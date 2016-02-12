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

angular.module('merchello.payments.models').constant('BraintreeCreditCard', BraintreeCreditCard);
