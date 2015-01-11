angular.module('merchello').controller('Merchello.Dashboards.InvoicePaymentsController',
    ['$scope', '$routeParams', function($scope, $routeParams) {

        $scope.invoice = {
            key: $routeParams.id
        };
        $scope.loaded = true;

}]);
