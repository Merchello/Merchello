angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.SpecAttributeCollectionAddEditController',
['$scope',
    function($scope) {

    $scope.wasFormSubmitted = false;

    $scope.save = function() {
        $scope.wasFormSubmitted = true;
        if ($scope.collectionForm.name.$valid) {

        }
    }

}]);
