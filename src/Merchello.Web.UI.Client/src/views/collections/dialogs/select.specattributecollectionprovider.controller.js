angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.SelectSpecAttributeProviderController',
    ['$scope',
    function($scope) {

        $scope.loaded = true;

        $scope.setSelection = function(provider) {
            $scope.submit(provider);
        }

}]);
