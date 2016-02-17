/*! MerchelloPaymentProviders
 * https://github.com/Merchello/Merchello
 * Copyright (c) 2016 Across the Pond, LLC.
 * Licensed MIT
 */


(function() { 

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

var BraintreeProviderSettings = function() {
    var self = this;
    self.environment = '';
    self.publicKey = '';
    self.privateKey = '';
    self.merchantId = '';
    self.merchantDescriptor = {};
    self.defaultTransactionOption = '';
};

angular.module('merchello.payments.models').constant('BraintreeProviderSettings', BraintreeProviderSettings);

var MerchantDescriptor = function() {
    var self = this;
    self.name = '';
    self.url = '';
    self.phone = '';
};


angular.module('merchello.payments.models').constant('MerchantDescriptor', MerchantDescriptor);

angular.module('merchello.payments.models').factory('braintreeCreditCardBuilder',
    ['genericModelBuilder', 'BraintreeCreditCard',
        function(genericModelBuilder, BraintreeCreditCard) {

            var Constructor = BraintreeCreditCard;

            return {
                createDefault: function () {
                    return new Constructor();
                },
                transform: function (jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);

angular.module('merchello.payments.models').factory('braintreeProviderSettingsBuilder',
    ['genericModelBuilder', 'merchantDescriptorBuilder', 'BraintreeProviderSettings',
        function(genericModelBuilder, merchantDescriptorBuilder, BraintreeProviderSettings) {

            var Constructor = BraintreeProviderSettings;

            return {
                createDefault: function () {
                    var settings = new Constructor();
                    settings.environment = 'Sandbox';
                    settings.merchantDescriptor = merchantDescriptorBuilder.createDefault();
                    settings.defaultTransactionOption = 'SubmitForSettlement';
                    return settings;
                },
                transform: function (jsonResult) {
                    var settings = genericModelBuilder.transform(jsonResult, Constructor);
                    settings.merchantDescriptor = merchantDescriptorBuilder.transform(jsonResult.merchantDescriptor);
                    return settings;
                }
            };
        }]);

angular.module('merchello.payments.models').factory('merchantDescriptorBuilder',
    ['genericModelBuilder', 'MerchantDescriptor',
        function(genericModelBuilder, MerchantDescriptor) {

            var Constructor = MerchantDescriptor;

            return {
                createDefault: function () {
                    return new Constructor();
                },
                transform: function (jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);


})();