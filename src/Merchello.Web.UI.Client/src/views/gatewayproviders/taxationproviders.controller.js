angular.module('merchello').controller('Merchello.Backoffice.TaxationProvidersController',
    ['$scope',
    function($scope) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;
        $scope.allCountries = [];
        $scope.availableCountries = [];
        $scope.taxationGatewayProviders = [];

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------
        /**
         * @ngdoc method
         * @name init
         * @function
         *
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        function init() {

            //loadAllAvailableCountries();
            //loadAllTaxationGatewayProviders();

        }

        function loadAllAvailableCountries() {

            var promiseAllCountries = merchelloSettingsService.getAllCountries();
            promiseAllCountries.then(function (allCountries) {

                $scope.allCountries = _.map(allCountries, function (country) {
                    return new merchello.Models.Country(country);
                });

            }, function (reason) {

                notificationsService.error("Available Countries Load Failed", reason.message);

            });

        }

        function loadAllTaxationGatewayProviders() {

            var promiseAllProviders = merchelloTaxationGatewayService.getAllGatewayProviders();
            promiseAllProviders.then(function (allProviders) {

                $scope.taxationGatewayProviders = _.map(allProviders, function (providerFromServer) {
                    return new merchello.Models.GatewayProvider(providerFromServer);
                });

                var noTaxProvider = new merchello.Models.GatewayProvider({ key: "-1", name: "Not Taxed", description: "" });
                $scope.taxationGatewayProviders.push(noTaxProvider);

                if ($scope.taxationGatewayProviders.length > 0) {
                    $scope.loadAvailableCountriesWithoutMethod($scope.taxationGatewayProviders[0]);
                    $scope.loadTaxMethods($scope.taxationGatewayProviders[0]);
                }

            }, function (reason) {

                notificationsService.error("Available Taxation Providers Load Failed", reason.message);

            });

        }

        function loadAvailableCountriesWithoutMethod(taxationGatewayProvider) {

            var promiseGwResources = merchelloTaxationGatewayService.getGatewayResources(taxationGatewayProvider.key);
            promiseGwResources.then(function (resources) {

                var newAvailableCountries = _.map(resources, function (resourceFromServer) {
                    var taxCountry = new merchello.Models.TaxCountry(resourceFromServer);

                    taxCountry.country = _.find($scope.allCountries, function (c) { return c.countryCode == taxCountry.serviceCode; });

                    if (taxCountry.country) {
                        taxCountry.setCountryName(taxCountry.country.name);
                    } else {
                        if (taxCountry.name == 'ELSE-FixedRate') {
                            taxCountry.setCountryName('Everywhere Else');
                        } else {
                            taxCountry.setCountryName(taxCountry.name);
                        }
                    }

                    return taxCountry;
                });

                _.each(newAvailableCountries, function (country) {
                    $scope.availableCountries.push(country);
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {

                notificationsService.error("Available Gateway Resources Load Failed", reason.message);

            });

        }

        function loadTaxMethods(taxationGatewayProvider) {

            var promiseGwResources = merchelloTaxationGatewayService.getTaxationProviderTaxMethods(taxationGatewayProvider.key);
            promiseGwResources.then(function (methods) {

                var newCountries = _.map(methods, function(methodFromServer) {
                    var taxMethod = new merchello.Models.TaxMethod(methodFromServer);
                    taxMethod.provider = taxationGatewayProvider;

                    var taxCountry = new merchello.Models.TaxCountry();

                    taxCountry.method = taxMethod;
                    taxCountry.country = _.find($scope.allCountries, function(c) { return c.countryCode == taxMethod.countryCode; });

                    if (taxCountry.country) {
                        taxCountry.setCountryName(taxCountry.country.name);
                    } else {
                        taxCountry.setCountryName(taxMethod.name);
                    }

                    return taxCountry;
                });

                _.each(newCountries, function(country) {
                    $scope.availableCountries.push(country);
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {

                notificationsService.error("Available Gateway Resources Load Failed", reason.message);

            });

        }


        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------

        function providerChange(method, providerSelected) {
            if (providerSelected.key.length > 0 && providerSelected.key != '-1') {
                method.providerKey = providerSelected.key;
            } else {
                method.providerKey = "";
            }
        };

        function save() {

            _.each($scope.availableCountries, function(taxCountry) {

                if (taxCountry.method.key.length > 0) {

                    if (taxCountry.method.providerKey.length > 0) {     // Already exists, just save it

                        var promiseTaxMethodSave = merchelloTaxationGatewayService.saveTaxMethod(taxCountry.method);
                        promiseTaxMethodSave.then(function() {
                            notificationsService.success("TaxMethod Saved", "");
                        }, function(reason) {
                            notificationsService.error("TaxMethod Save Failed", reason.message);
                        });

                    } else { // Existed, but set to Not Taxed, so delete it

                        var promiseTaxMethodDelete = merchelloTaxationGatewayService.deleteTaxMethod(taxCountry.method.key);
                        promiseTaxMethodDelete.then(function() {
                            notificationsService.success("TaxMethod Removed", "");
                        }, function(reason) {
                            notificationsService.error("TaxMethod Removal Failed", reason.message);
                        });

                    }

                } else {

                    if (taxCountry.method.providerKey.length > 0) { // Didn't exist before, create it

                        var promiseTaxMethodCreate = merchelloTaxationGatewayService.addTaxMethod(taxCountry.method);
                        promiseTaxMethodCreate.then(function() {
                            notificationsService.success("TaxMethod Created", "");
                        }, function(reason) {
                            notificationsService.error("TaxMethod Created Failed", reason.message);
                        });

                    }

                }

            });

        }

        //--------------------------------------------------------------------------------------
        // Dialogs
        //--------------------------------------------------------------------------------------

        function taxMethodDialogConfirm(country) {
            var promiseSave;

            // Save existing method
            promiseSave = merchelloTaxationGatewayService.saveTaxMethod(country.method);

            promiseSave.then(function () {
                notificationsService.success("Taxation Method Saved");
            }, function (reason) {
                notificationsService.error("Taxation Method Save Failed", reason.message);
            });
        }

        function editTaxMethodProvinces(country) {
            if (country) {
                dialogService.open({
                    template: '/App_Plugins/Merchello/Modules/Settings/Taxation/Dialogs/taxationmethod.html',
                    show: true,
                    callback: $scope.taxMethodDialogConfirm,
                    dialogData: country
                });
            }
        }

        // Initialize the controller
        init();
}]);
