(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Customer.CustomerEditAddressController
     * @function
     * 
     * @description
     * The controller for adding or editing an address.
     */
    controllers.CustomerEditAddressController = function ($scope, dialogService) {

        /**
         * @ngdoc method
         * @name chooseAddress
         * @function
         * 
         * @description
         * Return the appropriate address depending on the filter.
         */
        $scope.chooseAddress = function (filter) {
            var address = $scope.dialogData.addresses[(filter.id * 1) + 1];
            $scope.currentAddress = address;
        };

        /**
         * @ngdoc method
         * @name doesCountryHaveProvinces
         * @function
         * 
         * @description
         * Returns true if the country provided has provinces. Otherwise returns false.
         */
        $scope.doesCountryHaveProvinces = function (country) {
            var result = false;
            if (country.provinces) {
                if (country.provinces.length > 0) {
                    result = true;
                }
            }
            return result;
        };

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Inits the scope.
         */
        $scope.init = function () {
            $scope.setVariables();
            $scope.setAsDefault();
        };

        /**
         * @ngdoc method
         * @name openDeleteAddressDialog
         * @function
         * 
         * @description
         * Open a dialog to confirm whether to delete a selected address.
         */
        $scope.openDeleteAddressDialog = function () {
            var dialogData = {};
            dialogData.name = $scope.currentAddress.label;
            dialogService.open({
                template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
                show: true,
                callback: $scope.processDeleteAddressDialog,
                dialogData: dialogData
            });
        };

        // Remove any blank addresses and fix multiple defaults.
        $scope.prepareAddressesForSave = function () {
            var addresses = $scope.dialogData.addresses;
            addresses = _.map(addresses, function (address) {
                if (address.key == $scope.currentAddress.key) {
                    address = new merchello.Models.CustomerAddress($scope.currentAddress);
                }
                return address;
            });
            addresses = _.reject(addresses, function (address) {
                // Reject an address if it is blank (and remove from the array).
                return address.address1 == '';
            });
            if ($scope.dialogData.addressToReturn.isDefault) {
                _.each(addresses, function(address) {
                    if (address.isDefault) {
                        if (address.key !== $scope.dialogData.addressToReturn.key) {
                            address.isDefault = false;
                        }
                    }
                });
            }
            $scope.dialogData.addresses = _.map(addresses, function(address) {
                return new merchello.Models.CustomerAddress(address);
            });
        };

        /**
         * @ngdoc method
         * @name prepareCountryDropdown
         * @function
         * 
         * @description
         * This function sets the country dropdown to match the proper country.
         */
        $scope.prepareCountryDropdown = function() {
            for (var i = 0; i < $scope.countries.length; i++) {
                if ($scope.countries[i].countryCode == $scope.currentAddress.countryCode) {
                    $scope.dialogData.filters.country = $scope.countries[i];
                }
            }
            $scope.updateCountry($scope.dialogData.filters.country);
            $scope.prepareProvinceDropdown();
        };

        /**
         * @ngdoc method
         * @name prepareProvinceDropdown
         * @function
         * 
         * @description
         * This function sets the province dropdown to match the proper country.
         */
        $scope.prepareProvinceDropdown = function () {
            for (var i = 0; i < $scope.provinces.length; i++) {
                if ($scope.provinces[i].name == $scope.currentAddress.region) {
                    $scope.filters.province = $scope.provinces[i];
                }
            }
            $scope.updateProvince($scope.filters.province);
        };

        /**
         * @ngdoc method
         * @name processDeleteAddressDialog
         * @function
         * 
         * @description
         * Get response from a dialog on whether the selected address is to be deleted.
         */
        $scope.processDeleteAddressDialog = function (dialogData) {
            if ($scope.currentAddress.key === '') {
                $scope.currentAddress = new merchello.Models.CustomerAddress();
            } else {
                $scope.dialogData.shouldDelete = true;
                $scope.saveAddress();
            }
        };

        /**
         * @ngdoc method
         * @name saveAddress
         * @function
         * 
         * @description
         * Acquire the address and close the dialog, submitting collected data.
         */
        $scope.saveAddress = function() {
            if ($scope.editAddressForm.address1.$valid && $scope.editAddressForm.locality.$valid && $scope.editAddressForm.postalCode && $scope.dialogData.filters.country.id != -1) {
                $scope.dialogData.addressToReturn = $scope.currentAddress;
                $scope.prepareAddressesForSave();
                $scope.submit($scope.dialogData);
            } else {
                $scope.wasFormSubmitted = true;
            }
        };

        /**
         * @ngdoc method
         * @name setAsDefault
         * @function
         * 
         * @description
         * Set the default address as the first one to be edited.
         */
        $scope.setAsDefault = function() {
            var addresses = $scope.dialogData.addresses;
            for (var i = 0; i < addresses.length; i++) {
                if (addresses[i].isDefault) {
                    $scope.currentAddress = new merchello.Models.CustomerAddress(addresses[i]);
                    for (var j = 0; j < $scope.dialogData.addressAliases.length; j++) {
                        if ($scope.dialogData.addressAliases[j].id == (i - 1)) {
                            $scope.dialogData.filters.address = $scope.dialogData.addressAliases[j];
                        }
                    }
                }
            }
            $scope.prepareCountryDropdown();
        };

        /**
         * @ngdoc method
         * @name setVariables
         * @function
         * 
         * @description
         * Set the $scope variables.
         */
        $scope.setVariables = function() {
            $scope.countries = $scope.dialogData.countries;
            $scope.currentAddress = new merchello.Models.CustomerAddress();
            $scope.filters = {
                province: {}
            };
            $scope.provinces = [];
            $scope.wasFormSubmitted = false;
        }

        /**
         * @ngdoc method
         * @name updateCountry
         * @function
         * 
         * @description
         * Update the selected country for the applicable address type, and prepare the provinces for selection.
         */
        $scope.updateCountry = function (selectedCountry) {
            if (selectedCountry.id > -1) {
                $scope.currentAddress.countryCode = selectedCountry.countryCode;
                if (selectedCountry.provinces.length > 0) {
                    $scope.provinces = _.map(selectedCountry.provinces, function (province) {
                        return province;
                    });
                    $scope.provinces.unshift({ code: '00', name: 'Select State/Province' });
                    $scope.dialogData.filters.province = $scope.provinces[0];
                }
            } else {
                $scope.provinces = [];
            }
        };

        /**
         * @ngdoc method
         * @name updateProvince
         * @function
         * 
         * @description
         * Update the selected province for the applicable address type.
         */
        $scope.updateProvince = function (selectedProvince) {
            if (selectedProvince.code !== '00') {
                $scope.currentAddress.region = selectedProvince.code;
                _.each($scope.dialogData.addresses, function(address) {
                    if (address.key == $scope.currentAddress.key) {
                        address.region = selectedProvince.code;
                    }
                });
            }
        };

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Customer.Dialogs.CustomerEditAddressController", ['$scope', 'dialogService', merchello.Controllers.CustomerEditAddressController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
