    /**
     * @ngdoc controller
     * @name Merchello.Common.Dialogs.EditAddressController
     * @function
     * 
     * @description
     * The controller for adding a country
     */
    angular.module('merchello')
        .controller('Merchello.Common.Dialogs.EditAddressController',
        function ($scope) {

            // public methods
            $scope.save = save;

            function init() {
                $scope.address = $scope.dialogData.address;
            };

            function save() {
                $scope.dialogData.address.countryCode = $scope.dialogData.selectedCountry.countryCode;
                if($scope.dialogData.selectedCountry.provinces.length > 0) {
                    $scope.dialogData.address.region = $scope.dialogData.selectedProvince.code;
                }
                $scope.submit($scope.dialogData);
            };

        init();
    });

