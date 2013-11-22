(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.ShippingController
     * @function
     * 
     * @description
     * The controller for the reports list page
     */
    controllers.ShippingController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.sortProperty = "name";
        $scope.warehouses = [];
        $scope.newWarehouse = new merchello.Models.Warehouse();
        $scope.deleteWarehouse = new merchello.Models.Warehouse();
        $scope.visible = {
            addWarehouseFlyout: false,
            deleteWarehouseFlyout: false,
            shippingMethodPanel: false,
            warehouseInfoPanel: false,
            warehouseListPanel: true
        };

        $scope.loadWarehouses = function () {
            // Note From Kyle: Mocks for data returned from Warehouse API call
            var mockWarehouses = [
                {
                    pk: 0,
                    name: "Bramble Berry",
                    address1: "114 W. Magnolia St",
                    address2: "Suite 504",
                    locality: "Bellingham",
                    region: "WA",
                    postalCode: "98225",
                    countryCode: "US",
                    phone: "(360) 555-5555",
                    email: "info@brambleberry.com",
                    primary: true
                },
                {
                    pk: 1,
                    name: "Mindfly",
                    address1: "105 W Holly St",
                    address2: "H22",
                    locality: "Bellingham",
                    region: "WA",
                    postalCode: "98225",
                    countryCode: "US",
                    phone: "(360) 555-6666",
                    email: "hello@mindfly.com",
                    primary: false
                }
            ];
            $scope.warehouses = _.map(mockWarehouses, function (warehouseFromServer) {
                return new merchello.Models.Warehouse(warehouseFromServer);
            });
            // End of Warehouse API Mocks

        };

        $scope.changePrimaryWarehouse = function (warehouse) {
            if (warehouse) {
                for (i = 0; i < $scope.warehouses.length; i++) {
                    if (warehouse.pk == $scope.warehouses[i].pk) {
                        $scope.warehouses[i].primary = true;
                    } else {
                        $scope.warehouses[i].primary = false;
                    }
                }
                // Note From Kyle: An API call will need to be wired in here to change the primary values in the database.
            }
        };

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
                if ($scope.newWarehouse.primary) {
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

        $scope.loadWarehouses();

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    }

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.ShippingController", merchello.Controllers.ShippingController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
