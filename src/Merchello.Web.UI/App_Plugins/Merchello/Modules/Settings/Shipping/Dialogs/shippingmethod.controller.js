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

            // If service code is blank, the user has not selected the service, and cannot save.
            if ($scope.filters.gatewayResource.serviceCode !== '' && method.name !== '') {
                method.serviceCode = $scope.filters.gatewayResource.serviceCode;
                method.name = method.name;
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
                promiseSave.then(function() {
                    $scope.submit($scope.dialogData);
                }, function(reason) {
                    notificationsService.error("Shipping Method Save Failed", reason.message);
                });
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
            $scope.dialogData.method.rateTable.addRow($scope.newTier);
            $scope.isAddNewTier = false;
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
            $scope.dialogData.method.rateTable.removeRow(tier);
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
            if ($scope.dialogData.gatewayResources[0].serviceCode !== '') {
                var blankMethod = new merchello.Models.GatewayResource({ name: 'Choose Method', serviceCode: '' });
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

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingMethodController", ['$scope', 'merchelloCatalogFixedRateShippingService', 'merchelloCatalogShippingService', 'notificationsService', merchello.Controllers.ShippingMethodController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
