(function () {
    angular.module('controllers').controller('DogController', ['$scope', 'Dog', function ($scope, Dog) {
        $scope.pageTitle = "Dogs";
        $scope.dogs = Dog.query();
    }]);
}());