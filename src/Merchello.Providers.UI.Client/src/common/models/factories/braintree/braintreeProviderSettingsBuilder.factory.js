angular.module('merchello.providers.models').factory('braintreeProviderSettingsBuilder',
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
