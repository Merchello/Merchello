angular.module('merchello.plugins.braintree').controller('Merchello.Plugins.GatewayProviders.Dialogs.PaymentMethodAddEditController',
    ['$scope', 'braintreeProviderSettingsBuilder',
        function($scope, braintreeProviderSettingsBuilder) {

            $scope.providerSettings = {};

            function init() {
                var json = JSON.parse($scope.dialogData.provider.extendedData.getValue('braintreeProviderSettings'));
                $scope.providerSettings = braintreeProviderSettingsBuilder.transform(json);
                console.info($scope.providerSettings);
            }

            // initialize the controller
            init();

        }]);