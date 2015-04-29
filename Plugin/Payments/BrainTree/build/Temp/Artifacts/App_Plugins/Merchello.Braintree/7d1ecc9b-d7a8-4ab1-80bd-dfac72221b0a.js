(function() {

    var MerchantDescriptor = function() {
        var self = this;
        self.name = '';
        self.url = '';
        self.phone = '';
    };

    angular.module('merchello.plugins.braintree').constant('MerchantDescriptor', MerchantDescriptor);

    angular.module('merchello.plugins.braintree').factory('merchantDescriptorBuilder',
        ['genericModelBuilder', 'MerchantDescriptor',
            function(genericModelBuilder, BraintreeProviderSettings) {

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


    var BraintreeProviderSettings = function() {
        var self = this;
        self.environment = '';
        self.publicKey = '';
        self.privateKey = '';
        self.merchantId = '';
        self.merchantDescriptor = {};
        self.defaultTransactionOption = '';
    };

    angular.module('merchello.plugins.braintree').constant('BraintreeProviderSettings', BraintreeProviderSettings);

    angular.module('merchello.plugins.braintree').factory('braintreeProviderSettingsBuilder',
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
}());
