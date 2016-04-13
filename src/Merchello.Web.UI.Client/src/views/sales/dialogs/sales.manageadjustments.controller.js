angular.module('merchello').controller('Merchello.Sales.Dialogs.ManageAdjustmentsController',
    ['$scope', 'invoiceLineItemDisplayBuilder',
    function($scope, invoiceLineItemDisplayBuilder) {

        $scope.deleteAdjustment = deleteAdjustment;
        $scope.addAdjustment = addAdjustment;

        $scope.amount = 0.0;
        $scope.name = '';

        function init() {
            console.info($scope.dialogData);
        }

        function deleteAdjustment(item) {

        }

        function addAdjustment() {

        }

        init();

    }]);
