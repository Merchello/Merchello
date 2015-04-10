(function () {
    
    var authorizeNetProcessorSettings = function () {
        var self = this;
        self.LoginId = '';
        self.TransactionKey = '';
        self.Method = '';
        self.DelimitedData = '';
        self.DelimitedChar = '';
        self.EncapChar = '';
        self.RelayResponse = '';
        self.ApiVersion = '';
        self.UseSandbox = '';
    };

    angular.module('merchello.plugins.authorizenet').constant('AuthorizeNetProcessorSettings', authorizeNetProcessorSettings);

    angular.module('merchello.plugins.authorizenet').factory('authorizenetProviderSettingsBuilder',
        ['genericModelBuilder', 'AuthorizeNetProcessorSettings',
        function (genericModelBuilder, AuthorizeNetProcessorSettings) {

            var Constructor = AuthorizeNetProcessorSettings;

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
