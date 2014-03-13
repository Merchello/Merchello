(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.TaxationController
     * @function
     * 
     * @description
     * The controller for the reports list page
     */
    controllers.TaxationController = function ($scope, notificationsService, merchelloTaxationGatewayService, merchelloSettingsService) {

        $scope.allCountries = [];
        $scope.availableCountries = [];
        $scope.taxationGatewayProviders = [];

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        $scope.loadAllAvailableCountries = function () {

            var promiseAllCountries = merchelloSettingsService.getAllCountries();
            promiseAllCountries.then(function (allCountries) {

                $scope.allCountries = _.map(allCountries, function (country) {
                    return new merchello.Models.Country(country);
                });

            }, function (reason) {

                notificationsService.error("Available Countries Load Failed", reason.message);

            });

        };

        $scope.loadAllTaxationGatewayProviders = function () {

            var promiseAllProviders = merchelloTaxationGatewayService.getAllGatewayProviders();
            promiseAllProviders.then(function (allProviders) {

                $scope.taxationGatewayProviders = _.map(allProviders, function (providerFromServer) {
                    return new merchello.Models.GatewayProvider(providerFromServer);
                });

                var noTaxProvider = new merchello.Models.GatewayProvider();
                noTaxProvider.name = "Not Taxed";
                $scope.taxationGatewayProviders.push(noTaxProvider);

                if ($scope.taxationGatewayProviders.length > 0) {
                    $scope.loadAvailableCountriesWithoutMethod($scope.taxationGatewayProviders[0]);
                    $scope.loadTaxMethods($scope.taxationGatewayProviders[0]);
                }

            }, function (reason) {

                notificationsService.error("Available Taxation Providers Load Failed", reason.message);

            });

        };

        $scope.loadAvailableCountriesWithoutMethod = function (taxationGatewayProvider) {

            var promiseGwResources = merchelloTaxationGatewayService.getGatewayResources(taxationGatewayProvider.key);
            promiseGwResources.then(function (resources) {

                $scope.availableCountries = _.map(resources, function (resourceFromServer) {
                    var taxCountry = new merchello.Models.TaxCountry(resourceFromServer);

                    taxCountry.country = _.find($scope.allCountries, function (c) { return c.countryCode == taxCountry.serviceCode; });

                    taxCountry.countryName = taxCountry.country.name;

                    return taxCountry;
                });

            }, function (reason) {

                notificationsService.error("Available Gateway Resources Load Failed", reason.message);

            });

        };

        $scope.loadTaxMethods = function (taxationGatewayProvider) {

            var promiseGwResources = merchelloTaxationGatewayService.getTaxationProviderTaxMethods(taxationGatewayProvider.key);
            promiseGwResources.then(function (methods) {

                var newCountries = _.map(methods, function(methodFromServer) {
                    var taxMethod = new merchello.Models.TaxMethod(methodFromServer);
                    taxMethod.provider = taxationGatewayProvider;

                    var taxCountry = new merchello.Models.TaxCountry();

                    taxCountry.method = taxMethod;
                    taxCountry.country = _.find($scope.allCountries, function(c) { return c.countryCode == taxMethod.countryCode; });

                    taxCountry.countryName = taxMethod.name;

                    return taxCountry;
                });

                _.each(newCountries, function(country) {
                    $scope.availableCountries.push(country);
                });

            }, function (reason) {

                notificationsService.error("Available Gateway Resources Load Failed", reason.message);

            });

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

            $scope.loadAllAvailableCountries();
            $scope.loadAllTaxationGatewayProviders();

            $scope.loaded = true;
            $scope.preValuesLoaded = true;


        };

        $scope.init();


        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------

        $scope.providerChange = function (method, providerSelected) {
            method.providerKey = providerSelected.key;
        };

        $scope.save = function () {

            _.each($scope.availableCountries, function(taxCountry) {

                if (taxCountry.method.providerKey.length > 0) {

                    if (taxCountry.method.key.length > 0) {

                        var promiseTaxMethodSave = merchelloTaxationGatewayService.saveTaxMethod(taxCountry.method);
                        promiseTaxMethodSave.then(function() {
                            notificationsService.success("TaxMethod Saved", "");
                        }, function(reason) {
                            notificationsService.error("TaxMethod Save Failed", reason.message);
                        });

                    } else {

                        var promiseTaxMethodCreate = merchelloTaxationGatewayService.addTaxMethod(taxCountry.method);
                        promiseTaxMethodCreate.then(function() {
                            notificationsService.success("TaxMethod Created", "");
                        }, function(reason) {
                            notificationsService.error("TaxMethod Created Failed", reason.message);
                        });

                    }
                }

            });

        };

    };


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.TaxationController", merchello.Controllers.TaxationController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
