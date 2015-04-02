angular.module('merchello.plugins.sagepay').controller('Merchello.Plugin.Payment.SagePayPaymentProviderController',
    ['$scope',
    function ($scope) {

        var extendedDataKey = 'sagePayProviderSettings';
        var settingsString = $scope.dialogData.provider.extendedData.getValue(extendedDataKey);
        $scope.providerSettings = angular.fromJson(settingsString);
        console.info($scope.providerSettings);

        // Watch with object equality to convert back to a string for the submit() call on the Save button
        $scope.$watch(function () {
            return $scope.providerSettings;
        }, function (newValue, oldValue) {
            console.info(newValue);
            $scope.dialogData.provider.extendedData.setValue(extendedDataKey, angular.toJson(newValue));
        }, true);
        
    }
]);