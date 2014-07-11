(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingMethodController
     * @function
     * 
     * @description
     * The controller for the adding / editing shipping methods on the Shipping page
     */
    controllers.ShippingMethodController = function ($scope, merchelloCatalogFixedRateShippingService, merchelloCatalogShippingService, notificationsService) {

        $scope.isAddNewTier = false;
        $scope.newTier = {};

        /**
        * @ngdoc method
        * @name addOrUpdateShippingMethod
        * @function
        * 
        * @description
        * Add or update a new shipping method to the database before submitting.
        */
        $scope.addOrUpdateShippingMethod = function () {
            var country = $scope.dialogData.country;
            var method = $scope.dialogData.method;
            var provider = $scope.dialogData.provider;
            var promiseSave;
            if (provider.isFixedRate()) {
                if (method.shipMethod.key.length > 0) {
                    // Save existing method
                    promiseSave = merchelloCatalogFixedRateShippingService.saveRateTableShipMethod(method);
                } else {
                    // Create new method
                    promiseSave = merchelloCatalogFixedRateShippingService.createRateTableShipMethod(method);
                }
            } else {
                method.serviceCode = method.gatewayResource.serviceCode;
                if (method.shipMethod != undefined) {
                    method.name = method.shipMethod.name;
                }
                if (method.shipCountryKey == "00000000-0000-0000-0000-000000000000") {
                    method.shipCountryKey = country.key;
                }
                if (method.key.length > 0) {
                    // Save existing method
                    promiseSave = merchelloCatalogShippingService.saveShipMethod(method);
                } else {
                    // Create new method
                    promiseSave = merchelloCatalogShippingService.addShipMethod(method);
                }
            }
            promiseSave.then(function () {
                $scope.submit($scope.dialogData);
            }, function (reason) {
                notificationsService.error("Shipping Method Save Failed", reason.message);
            });
        };

        $scope.removeRateTier = function(tier) {
            $scope.dialogData.method.rateTable.removeRow(tier);
        };

        $scope.addRateTier = function() {
            $scope.dialogData.method.rateTable.addRow($scope.newTier);
            $scope.isAddNewTier = false;
        };

        $scope.insertRateTier = function() {
            $scope.isAddNewTier = true;
            $scope.newTier = merchello.Models.ShippingRateTier();
        };

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingMethodController", ['$scope', 'merchelloCatalogFixedRateShippingService', 'merchelloCatalogShippingService', 'notificationsService', merchello.Controllers.ShippingMethodController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
