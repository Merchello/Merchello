angular.module('merchello').controller('Merchello.Product.Dialogs.ProductCopyController',
    ['$scope',
    function($scope) {

        $scope.wasFormSubmitted = false;
        $scope.save = save;


        function save() {
            $scope.wasFormSubmitted = true;
            if ($scope.copyProductForm.name.$valid && $scope.copyProductForm.sku.$valid) {
                $scope.submit($scope.dialogData);
            }
        }
}]);
