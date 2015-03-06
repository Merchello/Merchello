(function () {

    var purchaseOrderProviderSettings = function () {
        var self = this;
        self.PurchaseOrderNumberPrefix = '';
    };

    angular.module('merchello.plugins.purchaseorder').constant('PurchaseOrderProviderSettings', purchaseOrderProviderSettings);

    angular.module('merchello.plugins.purchaseorder').factory('purchaseorderProviderSettingsBuilder',
        ['genericModelBuilder', 'PurchaseOrderProviderSettings',
        function (genericModelBuilder, PurchaseOrderProviderSettings) {

            var Constructor = PurchaseOrderProviderSettings;

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