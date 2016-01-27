angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloMultiProductPickerController',
    ['$scope', 'dialogService',
    function($scope, dialogService) {

        $scope.preValuesLoaded = false;
        $scope.products = [];
        $scope.remove = remove;
        $scope.openPickerDialog = openPickerDialog;


        function init() {

        }


        function remove(product) {

        }

        function openPickerDialog() {

        }

        init();
        
    }]);
