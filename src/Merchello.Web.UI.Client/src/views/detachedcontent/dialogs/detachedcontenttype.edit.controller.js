angular.module('merchello').controller('Merchello.DetachedContentType.Dialogs.EditDetachedContentTypeController',
    ['$scope', function($scope) {
        $scope.wasFormSubmitted = false;
        $scope.save = save;

        function save() {
            $scope.wasFormSubmitted = true;
            if ($scope.productContentTypeForm.name.$valid) {
                $scope.submit($scope.dialogData);
            }
        }
    }]);
