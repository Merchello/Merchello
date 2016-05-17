angular.module('merchello.providers').controller('Merchello.Providers.Dialogs.PayPalProviderSettingsController',
    ['$scope', 'payPalProviderSettingsBuilder',
        function($scope, payPalProviderSettingsBuilder) {

            $scope.providerSettings = {};


            function init() {

                var json = JSON.parse($scope.dialogData.provider.extendedData.getValue('paypalprovidersettings'));
                $scope.providerSettings = payPalProviderSettingsBuilder.transform(json);

                console.info($scope.providerSettings);

                $scope.$watch(function () {
                    return $scope.providerSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData.setValue('paypalprovidersettings', angular.toJson(newValue));
                }, true);

            }

            // initialize the controller
            init();

        }]);