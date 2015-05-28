angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferProviderSelectionController',
    ['$scope', function($scope) {
        $scope.loaded = true;

        $scope.setSelection = function(provider) {
            if (provider === undefined) {
                return;
            }
            $scope.dialogData.selectedProvider = provider;
            $scope.submit($scope.dialogData);
        };

}]);
