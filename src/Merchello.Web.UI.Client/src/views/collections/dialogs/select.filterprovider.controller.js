angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.SelectFilterProviderController',
    ['$scope',
    function($scope) {

        $scope.loaded = true;

        $scope.setSelection = function(provider) {
            $scope.submit(provider);
        }

}]);
