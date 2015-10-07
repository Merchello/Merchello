angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloMultiProductDialogController',
    ['$scope',
    function($scope) {

        $scope.loaded = false;

        function init() {
            $scope.loaded = true;
        }

        init();
}]);
