angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.SpecFilterCollectionAddController',
['$scope',
    function($scope) {

    $scope.wasFormSubmitted = false;

    $scope.save = function() {
        $scope.wasFormSubmitted = true;
        if ($scope.collectionForm.name.$valid) {
            $scope.submit($scope.dialogData)
        }
    }

}]);
