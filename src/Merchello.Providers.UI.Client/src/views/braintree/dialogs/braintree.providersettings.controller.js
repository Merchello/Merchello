angular.module('merchello.providers').controller('Merchello.Providers.Dialogs.BraintreeProviderSettingsController',
    ['$scope', 'braintreeProviderSettingsBuilder',
        function($scope, braintreeProviderSettingsBuilder) {

            $scope.providerSettings = {};


            function init() {
                var json = JSON.parse($scope.dialogData.provider.extendedData.getValue('braintreeProviderSettings'));
                $scope.providerSettings = braintreeProviderSettingsBuilder.transform(json);

                $scope.$watch(function () {
                    return $scope.providerSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData.setValue('braintreeProviderSettings', angular.toJson(newValue));
                }, true);

            }

            // initialize the controller
            init();

        }]);
