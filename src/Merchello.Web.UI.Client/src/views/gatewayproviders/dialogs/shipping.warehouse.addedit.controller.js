angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.WarehouseAddEditController',
    ['$scope',
    function($scope) {

        // exposed methods
        $scope.save = save;

        function save() {
            if($scope.dialogData.selectedCountry.provinces.length > 0) {
                $scope.dialogData.warehouse.region = $scope.dialogData.selectedProvince.code;
            }
            $scope.dialogData.warehouse.countryCode = $scope.dialogData.selectedCountry.countryCode;
            $scope.submit($scope.dialogData);
        }
}]);
