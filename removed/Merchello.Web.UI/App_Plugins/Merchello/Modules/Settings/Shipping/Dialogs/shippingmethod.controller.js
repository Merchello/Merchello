(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingMethodController
     * @function
     * 
     * @description
     * The controller for the adding / editing shipping methods on the Shipping page
     */
    controllers.ShippingMethodController = function ($scope, merchelloCatalogFixedRateShippingService, merchelloCatalogShippingService, merchelloFixedRateShippingService, notificationsService) {

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
            //var provider = $scope.dialogData.provider;
            var rateTable = $scope.rateTable;
            var promiseSaveMethod, promiseSaveRateTable;

            // If service code is blank, the user has not selected the service, and cannot save.
            if ($scope.filters.gatewayResource.serviceCode !== '' && method.name !== '') {
                method.serviceCode = $scope.filters.gatewayResource.serviceCode;
                method.name = method.name;
                if (method.shipCountryKey == "00000000-0000-0000-0000-000000000000") {
                    method.shipCountryKey = country.key;
                }
                if (method.key.length > 0) {
                    // Save existing method
                    promiseSaveMethod = merchelloCatalogShippingService.saveShipMethod(method);
                } else {
                    // Create new method
                    promiseSaveMethod = merchelloCatalogShippingService.addShipMethod(method);
                }
                promiseSaveMethod.then(function (methodResponse) {
                    rateTable.shipMethodKey = methodResponse.key;
                    promiseSaveRateTable = merchelloFixedRateShippingService.saveRateTable(rateTable);
                    promiseSaveRateTable.then(function() {
                        $scope.submit($scope.dialogData);
                    }, function(reason) {
                        notificationsService.error('Rate Table Save Failed', reason.message);
                    });
                }, function(reason) {
                    notificationsService.error("Shipping Method Save Failed", reason.message);
                });
            } else {
                notificationsService.error('Cannot Save Method without first having a name and selecting a method', '');
            }
        };

        /**
        * @ngdoc method
        * @name addRateTier
        * @function
        * 
        * @description
        * Adds the edited, new rate tier to the method.
        */
        $scope.addRateTier = function () {
            $scope.rateTable.addRow($scope.newTier);
            $scope.isAddNewTier = false;
        };

        /**
        * @ngdoc method
        * @name getRateTableIfRequired
        * @function
        * 
        * @description
        * Get the rate table if it exists.
        */
        $scope.getRateTable = function() {
            if ($scope.dialogData.method.key !== "") {
                var promise = merchelloFixedRateShippingService.getRateTable($scope.dialogData.method);
                promise.then(function(rateTable) {
                    $scope.rateTable = new merchello.Models.ShipRateTable(rateTable);
                }, function(reason) {
                    notificationsService.error('Could not retrieve rate table', reason.message);
                });
            }
        };

        /**
        * @ngdoc method
        * @name init
        * @function
        * 
        * @description
        * Runs when the scope is initialized.
        */
        $scope.init = function () {
            $scope.setVariables();
            $scope.getRateTable();
        }

        /**
        * @ngdoc method
        * @name insertRateTier
        * @function
        * 
        * @description
        * Inserts a new, blank row in the rate table.
        */
        $scope.insertRateTier = function () {
            $scope.isAddNewTier = true;
            $scope.newTier = merchello.Models.ShippingRateTier();
        };

        /**
        * @ngdoc method
        * @name removeRateTier
        * @function
        * 
        * @description
        * Remove a rate tier from the method.
        */
        $scope.removeRateTier = function (tier) {
            $scope.rateTable.removeRow(tier);
        };

        /**
        * @ngdoc method
        * @name setVariables
        * @function
        * 
        * @description
        * Set up the new variables for the scope upon init.
        */
        $scope.setVariables = function() {
            $scope.isAddNewTier = false;
            $scope.newTier = {};
            $scope.filters = {};
            $scope.rateTable = new merchello.Models.ShipRateTable();
            $scope.rateTable.shipMethodKey = $scope.dialogData.method.key;
            $scope.rateTable.shipCountryKey = $scope.dialogData.method.shipCountryKey;
            if ($scope.dialogData.gatewayResources[0].serviceCode !== '') {
                var blankMethod = new merchello.Models.GatewayResource({ name: '------', serviceCode: '' });
                $scope.dialogData.gatewayResources.unshift(blankMethod);
            }
            var serviceCode = $scope.dialogData.method.serviceCode;
            for (var i = 0; i < $scope.dialogData.gatewayResources.length; i++) {
                var resourceServiceCode = $scope.dialogData.gatewayResources[i].serviceCode;
                if (serviceCode.indexOf('VBP') == 0 || serviceCode.indexOf('VBW') == 0) {
                    serviceCode = serviceCode.split('-')[0];
                }
                if (resourceServiceCode == serviceCode) {
                    $scope.filters.gatewayResource = $scope.dialogData.gatewayResources[i];
                }
            }
        }

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingMethodController", ['$scope', 'merchelloCatalogFixedRateShippingService', 'merchelloCatalogShippingService', 'merchelloFixedRateShippingService', 'notificationsService', merchello.Controllers.ShippingMethodController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
