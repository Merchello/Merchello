(function () {

    var chaseProcessorSettings = function () {
        var self = this;
        self.MerchantId = '';
        self.Bin = '';
        self.Username = '';
        self.Password = '';
        self.Method = '';
        self.DelimitedData = '';
        self.DelimitedChar = '';
        self.EncapChar = '';
        self.RelayResponse = '';
        self.ApiVersion = '';
        self.UseSandbox = '';
    };

    angular.module('merchello.plugins.chase').constant('ChaseProcessorSettings', chaseProcessorSettings);

    angular.module('merchello.plugins.chase').factory('chaseProviderSettingsBuilder',
        ['genericModelBuilder', 'ChaseProcessorSettings',
        function (genericModelBuilder, ChaseProcessorSettings) {

            var Constructor = ChaseProcessorSettings;

            return {
                createDefault: function () {
                    var settings = new Constructor();
                    settings.useSandbox = '1';
                    return settings;
                },
                transform: function (jsonResult) {
                    var settings = genericModelBuilder.transform(jsonResult, Constructor);
                    return settings;
                }
            };
        }]);
}());
