(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.ShippingController
     * @function
     * 
     * @description
     * The controller for the reports list page
     */
    controllers.ShippingController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloWarehouseService, merchelloSettingsService) {

        $scope.sortProperty = "name";
        $scope.availableCountries = [];
        $scope.countries = [];
        $scope.warehouses = [];
        $scope.newWarehouse = new merchello.Models.Warehouse();
        $scope.deleteWarehouse = new merchello.Models.Warehouse();
        $scope.primaryWarehouse = new merchello.Models.Warehouse();
        $scope.visible = {
            addCountryFlyout: false,
            addWarehouseFlyout: false,
            deleteWarehouseFlyout: false,
            shippingMethodPanel: true,
            addEditShippingMethodFlyout: false,
            warehouseInfoPanel: false,
            warehouseListPanel: true
        };
        $scope.countryToAdd = new merchello.Models.Country();



        $scope.loadAllAvailableCountries = function () {

            var promiseAllCountries = merchelloSettingsService.getAllCountries();
            promiseAllCountries.then(function (allCountries) {

                $scope.availableCountries = _.map(allCountries, function (country) {
                    return new merchello.Models.Country(country)
                });

            }, function (reason) {

                notificationsService.error("Available Countries Load Failed", reason.message);

            });

        };

        $scope.loadWarehouses = function () {

            var promiseWarehouses = merchelloWarehouseService.getDefaultWarehouse();    // Only a default warehouse in v1
            promiseWarehouses.then(function (warehouseFromServer) {

                //warehouseFromServer.isDefault = true;
                $scope.warehouses.push(new merchello.Models.Warehouse(warehouseFromServer));

                $scope.changePrimaryWarehouse();

            }, function (reason) {

                notificationsService.error("Warehouses Load Failed", reason.message);

            });

        };

        $scope.loadCountries = function () {
            // Note From Kyle: This will have to change once the warehouse/catalog functionality is wired in.
            var catalogKey = $scope.primaryWarehouse.key;

            // Note From Kyle: Mocks from data returned from Shipping Country API call, where the countries have the catalogKey as the selected warehouse/catalog.
            var mockCountries = [
            ];
            $scope.countries = _.map(mockCountries, function (shippingCountryFromServer) {
                return new merchello.Models.ShippingCountry(shippingCountryFromServer);
            });
        };

        $scope.changePrimaryWarehouse = function (warehouse) {
            for (i = 0; i < $scope.warehouses.length; i++) {
                if (warehouse) {
                    if (warehouse.key == $scope.warehouses[i].key) {
                        $scope.warehouses[i].isDefault = true;
                        $scope.primaryWarehouse = $scope.warehouses[i];
                    } else {
                        $scope.warehouses[i].isDefault = false;
                    }
                } else {
                    if ($scope.warehouses[i].isDefault == true) {
                        $scope.primaryWarehouse = $scope.warehouses[i];
                    }
                }
            }
        };

        // Functions to control the Add/Edit Country flyout
        $scope.addCountryFlyout = new merchello.Models.Flyout(
            $scope.visible.addCountryFlyout,
            function (isOpen) {
                $scope.visible.addCountryFlyout = isOpen;
            },
            {
                clear: function () {
                    var self = $scope.addCountryFlyout;
                    self.model = new merchello.Models.ShippingCountry();
                },
                confirm: function () {
                    var self = $scope.addCountryFlyout;

                    if (!_.contains($scope.countries, $scope.countryToAdd)) {
                        var newShipCountry = new merchello.Models.ShippingCountry();
                        newShipCountry.catalogKey = $scope.primaryWarehouse.warehouseCatalogs[0].key;
                        newShipCountry.fromCountry($scope.countryToAdd);
                        $scope.countries.push(newShipCountry);
                    }
                    
                    self.clear();
                    self.close();
                },
                open: function (model) {
                    if (!model) {
                        $scope.addCountryFlyout.clear();
                    }
                }
            });

        // Functions to control the Add/Edit Warehouse flyout
        $scope.addWarehouseFlyout = {
            clear: function() {
                $scope.newWarehouse = new merchello.Models.Warehouse();
                $scope.newWarehouse.key = "no key created";
            },
            close: function () {
                $scope.visible.addWarehouseFlyout = false;
            },
            open: function (warehouse) {
                if (warehouse) {
                    $scope.newWarehouse = warehouse;
                } else {
                    $scope.addWarehouseFlyout.clear();
                }
                $scope.deleteWarehouseFlyout.close();
                $scope.visible.addWarehouseFlyout = true;
            },
            save: function () {

                var promiseWarehouseSave = merchelloWarehouseService.save($scope.newWarehouse);    // Only a default warehouse in v1
                promiseWarehouseSave.then(function (result) {

                    notificationsService.success("Warehouse Saved", "");

                    if ($scope.newWarehouse.isDefault) {
                        $scope.changePrimaryWarehouse($scope.newWarehouse);
                    }
                    $scope.addWarehouseFlyout.clear();
                    $scope.addWarehouseFlyout.close();

                }, function (reason) {

                    notificationsService.error("Warehouses Save Failed", reason.message);
                    $scope.addWarehouseFlyout.clear();
                    $scope.addWarehouseFlyout.close();

                });
            },
            toggle: function () {
                $scope.visible.addWarehouseFlyout = !$scope.visible.addWarehouseFlyout;
            }
        };

        // Functions to control the Delete Warehouse flyout
        $scope.deleteWarehouseFlyout = {
            close: function () {
                $scope.visible.deleteWarehouseFlyout = false;
            },
            confirm: function () {
                var idx = -1;
                for (i = 0; i < $scope.warehouses.length; i++) {
                    if ($scope.warehouses[i].key == $scope.deleteWarehouse.key) {
                        idx = i;
                    }
                }
                if (idx > -1) {
                    $scope.warehouses.splice(idx, 1);
                }
                // Note From Kyle: Some sort of logic to confirm this Warehouse isn't the primary, and is ok to delete is needed.
                $scope.deleteWarehouseFlyout.close();
                // Note From Kyle: An API call will need to be wired in here to delete the Warehouse in the database.
            },
            open: function (warehouse) {
                if (warehouse) {
                    $scope.deleteWarehouse = warehouse;
                    $scope.addWarehouseFlyout.clear();
                    $scope.addWarehouseFlyout.close();
                    $scope.visible.deleteWarehouseFlyout = true;
                }
            },
            toggle: function () {
                $scope.visible.deleteWarehouseFlyout = !$scope.visible.deleteWarehouseFlyout;
            }
        };

        // Functions to control the Shipping Methods panel
        $scope.shippingMethodPanel = {
            close: function () {
                $scope.visible.shippingMethodPanel = false;
            },
            open: function (country) {
                $scope.visible.shippingMethodPanel = true;
            },
            toggle: function () {
                $scope.visible.shippingMethodPanel = !$scope.visible.shippingMethodPanel;
            }
        };


        // Functions to control the Shipping Methods flyout
        $scope.addEditShippingMethodFlyout = {
            close: function () {
                $scope.visible.addEditShippingMethodFlyout = false;
            },
            open: function (country) {
                $scope.visible.addEditShippingMethodFlyout = true;
            },
            toggle: function () {
                $scope.visible.addEditShippingMethodFlyout = !$scope.visible.addEditShippingMethodFlyout;
            }
        };

        $scope.loadAllAvailableCountries();
        $scope.loadWarehouses();

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    }

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.ShippingController", merchello.Controllers.ShippingController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
