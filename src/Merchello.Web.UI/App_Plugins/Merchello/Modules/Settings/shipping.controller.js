(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.ShippingController
     * @function
     * 
     * @description
     * The controller for the reports list page
     */
    controllers.ShippingController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloWarehouseService) {

        $scope.sortProperty = "name";
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
            warehouseInfoPanel: false,
            warehouseListPanel: true
        };

        $scope.countryOptions = {
            placeholder: "United Republic of Tanzania",
            value: "",
            choices: [
                {
                    name: "United States of America"
                },
                {
                    name: "United Kingdom"
                },
                {
                    name: "United Republic of Tanzia"
                }
            ]
        };

        $scope.loadWarehouses = function () {

            var promiseWarehouses = merchelloWarehouseService.getDefaultWarehouse();    // Only a default warehouse in v1
            promiseWarehouses.then(function (warehouseFromServer) {

                warehouseFromServer.isDefault = true;
                $scope.warehouses.push(new merchello.Models.Warehouse(warehouseFromServer));

                $scope.changePrimaryWarehouse();

            }, function (reason) {

                notificationsService.error("Warehouses Load Failed", reason.message);

            });

        };

        $scope.loadCountries = function () {
            // Note From Kyle: This will have to change once the warehouse/catalog functionality is wired in.
            var catalogKey = $scope.primaryWarehouse.pk;

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
            // Note From Kyle: An API call will need to be wired in here to change the primary values in the database.
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
                    if ((typeof self.model.pk) == "undefined") {
                        var newKey = $scope.countries.length;
                        // Note From Kyle: This key-creation logic will need to be modified to fit whatever works for the database.
                        self.model.pk = newKey;
                        self.model.name = $scope.countryOptions.value;
                        self.model.catalogKey = $scope.primaryWarehouse.pk;
                        $scope.countries.push(self.model);
                        // Note From Kyle: An API call will need to be wired in here to add the new Country to the database.
                    } else {
                        // Note From Kyle: An API call will need to be wired in here to edit the existing Country in the database.
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
                $scope.newWarehouse.pk = "no key created";
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
                var idx = -1;
                for (i = 0; i < $scope.warehouses.length; i++) {
                    if ($scope.warehouses[i].pk == $scope.newWarehouse.pk) {
                        idx = i;
                    }
                }
                if (idx > -1) {
                    $scope.warehouses[idx] = $scope.newWarehouse;
                    // Note From Kyle: An API call will need to be wired in here to edit the existing Warehouse in the database.
                } else {
                    var newKey = $scope.warehouses.length;
                    // Note From Kyle: This key-creation logic will need to be modified to fit whatever works for the database.
                    $scope.newWarehouse.pk = newKey;
                    $scope.warehouses.push($scope.newWarehouse);
                    // Note From Kyle: An API call will need to be wired in here to add the new Warehouse to the database.
                }
                if ($scope.newWarehouse.isDefault) {
                    $scope.changePrimaryWarehouse($scope.newWarehouse);
                }
                $scope.addWarehouseFlyout.clear();
                $scope.addWarehouseFlyout.close();
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
                    if ($scope.warehouses[i].pk == $scope.deleteWarehouse.pk) {
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
            open: function () {
                $scope.visible.shippingMethodPanel = true;
            },
            toggle: function () {
                $scope.visible.shippingMethodPanel = !$scope.visible.shippingMethodPanel;
            }
        };

        $scope.loadWarehouses();

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    }

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.ShippingController", merchello.Controllers.ShippingController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
