(function () {

    var fedexProcessorSettings = function () {
        var self = this;
        self.FedExKey = '';
        self.FedExPassword = '';
        self.FedExPickupTypeCode = '';
        self.FedExCustomerClassification = '';
        self.FedExPackagingTypeCode = '';
        self.FedExAdditionalHandlingCharge = '';
        self.ApiVersion = '';
        self.UseSandbox = '';
    };

    angular.module('merchello.plugins.fedex').constant('FedexProcessorSettings', fedexProcessorSettings);

    angular.module('merchello.plugins.fedex').factory('fedexProviderSettingsBuilder',
        ['genericModelBuilder', 'FedexProcessorSettings',
        function (genericModelBuilder, FedexProcessorSettings) {

            var Constructor = FedexProcessorSettings;

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
