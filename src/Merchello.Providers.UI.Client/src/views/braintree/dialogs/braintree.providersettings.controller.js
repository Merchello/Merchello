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

                    // environmentType is missed for some reason, so we need to set here
                    // It's the same value as environment, not sure why we need two?
                    newValue.environmentType = newValue.environment;

                    $scope.dialogData.provider.extendedData.setValue('braintreeProviderSettings', angular.toJson(newValue));
                }, true);

            }

            // initialize the controller
            init();

        }]);
