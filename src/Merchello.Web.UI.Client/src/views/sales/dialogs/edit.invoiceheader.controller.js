angular.module('merchello').controller('Merchello.Sales.Dialogs.InvoiceHeaderController',
    ['$scope',
    function($scope) {

        function init() {
            console.info($scope.dialogData);
           $scope.loaded = true;
        }

        init();
}]);
