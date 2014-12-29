(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Plugin.GatewayProviders.Taxation.Dialogs.AvaTaxGatewayProviderController
     * @function
     * 
     * @description
     * The controller for the editing of Avalara tax provider info on the Gateway Providers page.
     */
    controllers.AvalaraGatewayProviderController = function ($scope, $q, merchelloSettingsService) {

        /**
        * @ngdoc method 
        * @name buildAvataxSettingsFromString
        * @function
        * 
        * @description
        * On initial load extendedData will be empty but we need to populate with key values.
        */
        $scope.buildAvataxSettingsFromString = function () {
            var settingsString = $scope.dialogData.provider.extendedData[0].value;
            $scope.avataxSettings = JSON.parse(settingsString);
            $scope.populateInitialCountryDropdownState($scope.avataxSettings.DefaultStoreAddress.Country);
            $scope.populateInitialProvinceDropdownState($scope.avataxSettings.DefaultStoreAddress.Region);
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
        * Method called on intial page load.  Loads in data from server and sets up scope.
        */
        $scope.init = function () {
            $scope.setVariables();
            var gotCountries = $scope.loadCountries();
            gotCountries.then(function() {
                $scope.buildAvataxSettingsFromString();
            });
        };

        /**
         * @ngdoc method
         * @name loadCountries
         * @function
         * 
         * @description
         * Load a list of countries and provinces from the API. 
         */
        $scope.loadCountries = function () {
            var deferred = $q.defer();
            $scope.countries = [];
            var promiseCountries = merchelloSettingsService.getAllCountries();
            promiseCountries.then(function(countriesResponse) {
                for (var i = 0; i < countriesResponse.length; i++) {
                    var country = countriesResponse[i];
                    var newCountry = {
                        id: i,
                        countryCode: country.countryCode,
                        name: country.name,
                        provinces: country.provinces,
                        provinceLabel: country.provinceLabel
                    };
                    $scope.countries.push(newCountry);
                }
                $scope.countries.sort($scope.sortCountries);
                $scope.countries.unshift({ id: -1, name: 'Select Country', countryCode: '00', provinces: {}, provinceLabel: '' });
                $scope.filters.country = $scope.countries[0];
                deferred.resolve($scope.countries);
            });
            return deferred.promise;
        };

        /**
         * @ngdoc method
         * @name populateInitialCountryDropdownState
         * @function
         * 
         * @description
         * Gets the initial country dropdown state based on the provided country. 
         */
        $scope.populateInitialCountryDropdownState = function (countryCode) {
            for (var i = 0; i < $scope.countries.length; i++) {
                if ($scope.countries[i].countryCode == countryCode) {
                    $scope.filters.country = $scope.countries[i];
                    $scope.provinces = _.map($scope.countries[i].provinces, function (province) {
                        return province;
                    });
                    $scope.provinces.unshift({ code: '00', name: 'Select State/Province' });
                }
            }
        };

        /**
         * @ngdoc method
         * @name populateInitialProvinceDropdownState
         * @function
         * 
         * @description
         * Gets the initial country province state based on the provided province. 
         */
        $scope.populateInitialProvinceDropdownState = function (province) {
            if ($scope.doesCountryHaveProvinces($scope.filters.country)) {
                for (var i = 0; i < $scope.filters.country.provinces.length; i++) {
                    if ($scope.filters.country.provinces[i].code == province) {
                        $scope.filters.province = $scope.filters.country.provinces[i];
                    }
                }
            }
        };

        /**
         * @ngdoc method
         * @name saveSettings
         * @function
         * 
         * @description
         * If the form is valid, acquire the settings and close the dialog, submitting the collected data.
         */
        $scope.saveSettings = function () {
            if ($scope.editProviderForm.address1.$valid && $scope.editProviderForm.locality.$valid && $scope.editProviderForm.postalCode && $scope.filters.country.id !== -1) {
                $scope.dialogData.provider.extendedData[0].value = angular.toJson($scope.avataxSettings);
                $scope.submit($scope.dialogData);
            } else {
                $scope.wasFormSubmitted = true;
            }
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
            $scope.countries = [];
            $scope.filters = {
                country: {},
                provinces: {}
            };
            $scope.wasFormSubmitted = false;
        };

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
                $scope.avataxSettings.DefaultStoreAddress.Country = selectedCountry.countryCode;
                if (selectedCountry.provinces.length > 0) {
                    $scope.provinces = _.map(selectedCountry.provinces, function (province) {
                        return province;
                    });
                    $scope.provinces.unshift({ code: '00', name: 'Select State/Province' });
                    $scope.filters.province = $scope.provinces[0];
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
                $scope.avataxSettings.DefaultStoreAddress.Region = selectedProvince.code;
            }
        };

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Plugin.GatewayProviders.Taxation.Dialogs.AvaTaxGatewayProviderController", ['$scope', '$q', 'merchelloSettingsService', merchello.Controllers.AvalaraGatewayProviderController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
