angular.module('merchello.providers').controller('Merchello.Providers.Dialogs.PayPalProviderSettingsController',
    ['$scope', 'payPalProviderSettingsBuilder',
        function($scope, braintreeProviderSettingsBuilder) {

            $scope.providerSettings = {};


            function init() {
                var json = JSON.parse($scope.dialogData.provider.extendedData.getValue('payPalProviderSettings'));
                $scope.providerSettings = braintreeProviderSettingsBuilder.transform(json);

                $scope.$watch(function () {
                    return $scope.providerSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData.setValue('payPalProviderSettings', angular.toJson(newValue));
                }, true);

            }

            // initialize the controller
            init();

        }]);