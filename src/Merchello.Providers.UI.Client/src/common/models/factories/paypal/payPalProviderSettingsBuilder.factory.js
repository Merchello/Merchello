angular.module('merchello.providers.models').factory('payPalProviderSettingsBuilder',
    ['genericModelBuilder', 'merchantDescriptorBuilder', 'PayPalProviderSettings',
        function(genericModelBuilder, merchantDescriptorBuilder, PayPalProviderSettings) {

            var Constructor = PayPalProviderSettings;

            return {
                createDefault: function () {
                    var settings = new Constructor();
                    settings.mode = 'Sandbox';
                    return settings;
                },
                transform: function (jsonResult) {
                    var settings = genericModelBuilder.transform(jsonResult, Constructor);
                    return settings;
                }
            };
        }]);
