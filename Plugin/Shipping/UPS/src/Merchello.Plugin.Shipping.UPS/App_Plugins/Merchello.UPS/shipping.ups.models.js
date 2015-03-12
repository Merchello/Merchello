(function () {

    var upsProcessorSettings = function () {
        var self = this;
        self.UpsAccessKey = '';
        self.UpsUserName = '';
        self.UpsPassword = '';
        self.UpsPickupTypeCode = '';
        self.UpsCustomerClassification = '';
        self.UpsPackagingTypeCode = '';
        self.UpsAdditionalHandlingCharge = '';
        self.UpsFreeGroundShipping = '';
        self.ApiVersion = '';
        self.UseSandbox = '';
    };

    angular.module('merchello.plugins.ups').constant('UpsProcessorSettings', upsProcessorSettings);

    angular.module('merchello.plugins.ups').factory('upsProviderSettingsBuilder',
        ['genericModelBuilder', 'UpsProcessorSettings',
        function (genericModelBuilder, UpsProcessorSettings) {

            var Constructor = UpsProcessorSettings;

            return {
                createDefault: function () {
                    var settings = new Constructor();
                    return settings;
                },
                transform: function (jsonResult) {
                    var settings = genericModelBuilder.transform(jsonResult, Constructor);
                    return settings;
                }
            };
        }]);
}());
