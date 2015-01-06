(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingRegionsController
     * @function
     * 
     * @description
     * The controller for the adjusting shipping rates for specific regions on the Shipping page
     */
    controllers.ShippingRegionsController = function ($scope, merchelloCatalogShippingService, notificationsService) {

        /**
        * @ngdoc method
        * @name init
        * @function
        * 
        * @description
        * Initializes the controller.
        */
        $scope.init = function() {

            $scope.setVariables();

        };

        /**
        * @ngdoc method
        * @name init
        * @function
        * 
        * @description
        * Save the regional rates and then close the dialog.
        */
        $scope.saveRegionRates = function () {

            var country = $scope.dialogData.country;
            var method = $scope.dialogData.method;
            var promiseSaveMethod;

            // If service code is blank, the user has not selected the service, and cannot save.
                if (method.shipCountryKey == "00000000-0000-0000-0000-000000000000") {
                    method.shipCountryKey = country.key;
                }
                promiseSaveMethod = merchelloCatalogShippingService.saveShipMethod(method);
                promiseSaveMethod.then(function () {
                    $scope.submit($scope.dialogData);
                }, function (reason) {
                    notificationsService.error('Regional Rates Adjustment Save Failed', reason.message);
                });
        }

        /**
        * @ngdoc method
        * @name setVariables
        * @function
        * 
        * @description
        * Sets the scope variables.
        */
        $scope.setVariables = function() {
            $scope.allProvinces = false;
        };

        /**
        * @ngdoc method
        * @name toggleAllProvinces
        * @function
        * 
        * @description
        * Toggle the provinces.
        */
        $scope.toggleAllProvinces = function () {
            _.each($scope.dialogData.method.provinces, function (province) { province.allowShipping = $scope.allProvinces; });
        };

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingRegionsController", ['$scope', 'merchelloCatalogShippingService', 'notificationsService', merchello.Controllers.ShippingRegionsController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
