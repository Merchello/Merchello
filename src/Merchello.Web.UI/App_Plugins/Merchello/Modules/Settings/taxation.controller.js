(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.TaxationController
     * @function
     * 
     * @description
     * The controller for the reports list page
     */
    controllers.TaxationController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.taxRates = [];
        $scope.sortProperty = "name";
        $scope.sortOrder = "asc";
        $scope.deleteTaxRate = new merchello.Models.TaxRate();
        $scope.newTaxRate = new merchello.Models.TaxRate();

        $scope.loadTaxRates = function () {

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
            $scope.taxRates = _.map(mockTaxRates, function (taxRateFromServer) {
                return new merchello.Models.TaxRate(taxRateFromServer);
            });
            // End of Tax Rates API mocks.
            $scope.loaded = true;
            $scope.preValuesLoaded = true;

        };

        $scope.loadTaxRates();

    }


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.TaxationController", merchello.Controllers.TaxationController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
