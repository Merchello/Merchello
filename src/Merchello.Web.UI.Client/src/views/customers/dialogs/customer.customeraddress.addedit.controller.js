    /**
     * @ngdoc controller
     * @name Merchello.Customer.Dialogs.CustomerAddressAddEditController
     * @function
     *
     * @description
     * The controller for adding and editing customer addresses
     */
    angular.module('merchello').controller('Merchello.Customer.Dialogs.CustomerAddressAddEditController',
        ['$scope',
        function($scope) {

            $scope.wasFormSubmitted = false;

            // exposed methods
            $scope.updateProvinceList = updateProvinceList;
            $scope.toTitleCase = toTitleCase;
            $scope.save = save;

            function updateProvinceList() {
                // try to find the province matching the province code of the customer address
                var provinceCode = $scope.dialogData.customerAddress.region;
                if($scope.dialogData.selectedCountry.provinces.length > 0) {
                    var province = _.find($scope.dialogData.selectedCountry.provinces, function(p) {
                        return p.code === provinceCode;
                    });
                    if (province === null || province === undefined) {
                        $scope.dialogData.selectedProvince = $scope.dialogData.selectedCountry.provinces[0];
                    } else {
                        $scope.dialogData.selectedProvince = province;
                    }
                }
            }

            function save() {
                $scope.wasFormSubmitted = true;
                if($scope.editAddressForm.address1.$valid && $scope.editAddressForm.locality.$valid && $scope.editAddressForm.postalCode.$valid) {
                    if($scope.dialogData.selectedCountry.hasProvinces()) {
                        $scope.dialogData.customerAddress.region = $scope.dialogData.selectedProvince.code;
                    }
                    $scope.dialogData.customerAddress.countryCode = $scope.dialogData.selectedCountry.countryCode;
                    $scope.submit($scope.dialogData);
                }
            }

            function toTitleCase(str) {
                return str.charAt(0).toUpperCase() + str.slice(1);
            }

    }]);
