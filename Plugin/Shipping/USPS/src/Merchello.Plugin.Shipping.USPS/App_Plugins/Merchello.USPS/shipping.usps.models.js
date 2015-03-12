(function () {

    var uspsProcessorSettings = function () {
        var self = this;
        self.UspsUsername = '';
        self.UspsPassword = '';
        self.UspsAdditionalHandlingCharge = '';
        self.UseSandbox = '';
    };

    angular.module('merchello.plugins.usps').constant('UspsProcessorSettings', uspsProcessorSettings);

    angular.module('merchello.plugins.usps').factory('uspsProviderSettingsBuilder',
        ['genericModelBuilder', 'UspsProcessorSettings',
        function (genericModelBuilder, UspsProcessorSettings) {

            var Constructor = UspsProcessorSettings;

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
