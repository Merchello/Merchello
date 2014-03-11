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

        $scope.availableCountries = [];
        $scope.taxationGatewayProviders = [];

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        $scope.loadAllAvailableCountries = function () {

            var promiseAllCountries = merchelloSettingsService.getAllCountries();
            promiseAllCountries.then(function (allCountries) {

                $scope.availableCountries = _.map(allCountries, function (country) {
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

            }, function (reason) {

                notificationsService.error("Available Taxation Providers Load Failed", reason.message);

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

        };

        $scope.init();






        $scope.taxRates = [];
        $scope.sortProperty = "name";
        $scope.sortOrder = "asc";
        $scope.deleteTaxRate = {};
        $scope.newTaxRate = {};

        $scope.loadTaxRates = function() {

            // Note From Kyle: Mocks for data returned from Tax Rates API call.
            var mockTaxRates = [
                {
                    pk: 0,
                    country: "Canada",
                    rate: 11.5
                },
                {
                    pk: 1,
                    country: "United Kingdom",
                    rate: 4
                },
                {
                    pk: 2,
                    country: "United States",
                    rate: 7.8
                },
                {
                    pk: 3,
                    country: "Everywhere Else",
                    rate: 0
                }
            ];
            $scope.taxRates = _.map(mockTaxRates, function(taxRateFromServer) {
                return taxRateFromServer;
            });
            // End of Tax Rates API mocks.
            $scope.loaded = true;
            $scope.preValuesLoaded = true;

        };

        $scope.loadTaxRates();

    };


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.TaxationController", merchello.Controllers.TaxationController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
