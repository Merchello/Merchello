(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.ShippingController
     * @function
     * 
     * @description
     * The controller for the shipping warehouse and providers management page
     */
    controllers.ShippingController = function($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, dialogService, merchelloWarehouseService, merchelloSettingsService, merchelloCatalogShippingService, merchelloCatalogFixedRateShippingService) {

        $scope.sortProperty = "name";
        $scope.availableCountries = [];
        $scope.availableFixedRateGatewayResources = [];
        $scope.countries = [];
        $scope.warehouses = [];
        $scope.providers = [];
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
        $scope.countryToAdd = new merchello.Models.Country();
        $scope.providerToAdd = {};
        $scope.currentShipCountry = {};

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        $scope.loadAllAvailableFixedRateGatewayResources = function() {

            var promiseAllResources = merchelloCatalogFixedRateShippingService.getAllFixedRateGatewayResources();
            promiseAllResources.then(function(allResources) {

                $scope.availableFixedRateGatewayResources = _.map(allResources, function(resource) {
                    return new merchello.Models.GatewayResource(resource);
                });

            }, function(reason) {

                notificationsService.error("Available Gateway Resources Load Failed", reason.message);

            });

        };

        $scope.loadAllAvailableCountries = function() {

            var promiseAllCountries = merchelloSettingsService.getAllCountries();
            promiseAllCountries.then(function(allCountries) {

                $scope.availableCountries = _.map(allCountries, function(country) {
                    return new merchello.Models.Country(country);
                });

            }, function(reason) {

                notificationsService.error("Available Countries Load Failed", reason.message);

            });

        };

        $scope.loadAllShipProviders = function() {

            var promiseAllProviders = merchelloCatalogShippingService.getAllShipGatewayProviders();
            promiseAllProviders.then(function(allProviders) {

                $scope.providers = _.map(allProviders, function(providerFromServer) {
                    return new merchello.Models.GatewayProvider(providerFromServer);
                });

            }, function(reason) {

                notificationsService.error("Available Ship Providers Load Failed", reason.message);

            });

        };

        $scope.loadWarehouses = function() {

            var promiseWarehouses = merchelloWarehouseService.getDefaultWarehouse(); // Only a default warehouse in v1
            promiseWarehouses.then(function(warehouseFromServer) {

                $scope.warehouses.push(new merchello.Models.Warehouse(warehouseFromServer));

                $scope.changePrimaryWarehouse();

                $scope.loadCountries();

            }, function(reason) {

                notificationsService.error("Warehouses Load Failed", reason.message);

            });

        };

        $scope.loadCountries = function() {

            if ($scope.primaryWarehouse.warehouseCatalogs.length > 0) {
                var catalogKey = $scope.primaryWarehouse.warehouseCatalogs[0].key;

                var promiseShipCountries = merchelloCatalogShippingService.getWarehouseCatalogShippingCountries(catalogKey);
                promiseShipCountries.then(function(shipCountriesFromServer) {

                    $scope.countries = _.map(shipCountriesFromServer, function(shippingCountryFromServer) {
                        return new merchello.Models.ShippingCountry(shippingCountryFromServer);
                    });

                    _.each($scope.countries, function(element, index, list) {
                        $scope.loadFixedRateCountryProviders(element);
                    });

                }, function(reason) {

                    notificationsService.error("Shipping Countries Load Failed", reason.message);

                });
            }
        };

        $scope.loadFixedRateCountryProviders = function(country) {

            if (country) {
                var promiseProviders = merchelloCatalogFixedRateShippingService.getAllShipCountryFixedRateProviders(country.key);
                promiseProviders.then(function(providerFromServer) {

                    if (providerFromServer.length > 0) {

                        _.each(providerFromServer, function(element, index, list) {
                            var newProvider = new merchello.Models.ShippingGatewayProvider(element);
                            newProvider.shipMethods = [];
                            country.shippingGatewayProviders.push(newProvider);
                            $scope.loadFixedRateProviderMethods(country);
                        });
                    }

                }, function(reason) {

                    notificationsService.error("Shipping Countries Providers Load Failed", reason.message);

                });
            }
        };

        $scope.loadFixedRateProviderMethods = function(country) {

            if (country) {
                var promiseMethods = merchelloCatalogFixedRateShippingService.getAllFixedRateProviderMethods(country.key);
                promiseMethods.then(function(methodsFromServer) {

                    if (methodsFromServer.length > 0) {

                        _.each(methodsFromServer, function(element, index, list) {
                            var newMethod = new merchello.Models.FixedRateShippingMethod(element);

                            var shippingGatewayProvider = _.find(country.shippingGatewayProviders, function(p) { return p.key == newMethod.shipMethod.providerKey; });
                            shippingGatewayProvider.shipMethods.push(newMethod);
                        });
                    }

                }, function(reason) {

                    notificationsService.error("Shipping Countries Methods Load Failed", reason.message);

                });
            }
        };

        //--------------------------------------------------------------------------------------
        // Warehouse methods
        //--------------------------------------------------------------------------------------

        $scope.changePrimaryWarehouse = function(warehouse) {
            for (var i = 0; i < $scope.warehouses.length; i++) {
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

        //--------------------------------------------------------------------------------------
        // Flyout methods
        //--------------------------------------------------------------------------------------

        // Functions to control the Add/Edit Country flyout
        $scope.addCountryFlyout = new merchello.Models.Flyout(
            $scope.visible.addCountryFlyout,
            function(isOpen) {
                $scope.visible.addCountryFlyout = isOpen;
            },
            {
                confirm: function() {
                    var self = $scope.addCountryFlyout;

                    var countryOnCatalog = _.find($scope.countries, function(shipCountry) { return shipCountry.countryCode == self.model.countryCode; });
                    if (!countryOnCatalog) {

                        var catalogKey = $scope.primaryWarehouse.warehouseCatalogs[0].key;

                        var promiseShipCountries = merchelloCatalogShippingService.newWarehouseCatalogShippingCountry(catalogKey, self.model.countryCode);
                        promiseShipCountries.then(function(shippingCountryFromServer) {

                            $scope.countries.push(new merchello.Models.ShippingCountry(shippingCountryFromServer));

                        }, function(reason) {

                            notificationsService.error("Shipping Countries Create Failed", reason.message);

                        });
                    }

                    self.clear();
                    self.close();
                }
            });

        $scope.deleteCountry = function(country) {

            var promiseDelete = merchelloCatalogShippingService.deleteShipCountry(country.key);
            promiseDelete.then(function () {

                notificationsService.success("Shipping Country Deleted");

                $scope.loadCountries();

            }, function (reason) {

                notificationsService.error("Shipping Country Delete Failed", reason.message);

            });
        };

        // Functions to control the Add/Edit Warehouse flyout
        $scope.addWarehouseFlyout = {
            clear: function() {
                $scope.newWarehouse = new merchello.Models.Warehouse();
                $scope.newWarehouse.key = "no key created";
            },
            close: function() {
                $scope.visible.addWarehouseFlyout = false;
            },
            open: function(warehouse) {
                if (warehouse) {
                    $scope.newWarehouse = warehouse;
                } else {
                    $scope.addWarehouseFlyout.clear();
                }
                $scope.deleteWarehouseFlyout.close();
                $scope.visible.addWarehouseFlyout = true;
            },
            save: function() {

                var promiseWarehouseSave = merchelloWarehouseService.save($scope.newWarehouse); // Only a default warehouse in v1
                promiseWarehouseSave.then(function() {

                    notificationsService.success("Warehouse Saved", "");

                    if ($scope.newWarehouse.isDefault) {
                        $scope.changePrimaryWarehouse($scope.newWarehouse);
                    }
                    $scope.addWarehouseFlyout.clear();
                    $scope.addWarehouseFlyout.close();

                }, function(reason) {

                    notificationsService.error("Warehouses Save Failed", reason.message);
                    $scope.addWarehouseFlyout.clear();
                    $scope.addWarehouseFlyout.close();

                });
            },
            toggle: function() {
                $scope.visible.addWarehouseFlyout = !$scope.visible.addWarehouseFlyout;
            }
        };

        // Functions to control the Delete Warehouse flyout
        $scope.deleteWarehouseFlyout = {
            close: function() {
                $scope.visible.deleteWarehouseFlyout = false;
            },
            confirm: function() {
                var idx = -1;
                for (var i = 0; i < $scope.warehouses.length; i++) {
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
            open: function(warehouse) {
                if (warehouse) {
                    $scope.deleteWarehouse = warehouse;
                    $scope.addWarehouseFlyout.clear();
                    $scope.addWarehouseFlyout.close();
                    $scope.visible.deleteWarehouseFlyout = true;
                }
            },
            toggle: function() {
                $scope.visible.deleteWarehouseFlyout = !$scope.visible.deleteWarehouseFlyout;
            }
        };

        $scope.shippingProviderDialogConfirm = function(data) {

            var selectedProvider = data.provider;

            var newShippingMethod = new merchello.Models.FixedRateShippingMethod();
            newShippingMethod.shipMethod.name = data.country.name + " Fixed Rate";
            newShippingMethod.shipMethod.providerKey = selectedProvider.key;
            newShippingMethod.shipMethod.shipCountryKey = data.country.key;

            var promiseAddMethod = merchelloCatalogFixedRateShippingService.createRateTableShipMethod(newShippingMethod);
            promiseAddMethod.then(function() {

                $scope.loadFixedRateCountryProviders(data.country);

            }, function(reason) {

                notificationsService.error("Shipping Provider / Initial Method Create Failed", reason.message);

            });

        };

        $scope.addEditShippingProviderDialogOpen = function(country, provider) {

            var dialogProvider = provider;
            if (!provider) {
                dialogProvider = new merchello.Models.ShippingGatewayProvider();
            }

            var myDialogData = {
                country: country,
                provider: dialogProvider,
                availableProviders: $scope.providers
            };

            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shippingprovider.html',
                show: true,
                callback: $scope.shippingProviderDialogConfirm,
                dialogData: myDialogData
            });
        };

        $scope.shippingMethodDialogConfirm = function (data) {

            var promiseSave;
            if (data.method.shipMethod.key.length > 0) {
                // Save existing method
                promiseSave = merchelloCatalogFixedRateShippingService.saveRateTableShipMethod(data.method);
            } else {
                // Create new method
                promiseSave = merchelloCatalogFixedRateShippingService.createRateTableShipMethod(data.method);
            }

            promiseSave.then(function() {
                data.provider.shipMethods = [];
                $scope.loadFixedRateProviderMethods(data.country);
            }, function(reason) {
                notificationsService.error("Shipping Method Save Failed", reason.message);
            });
        };

        $scope.addEditShippingMethodDialogOpen = function(country, provider, method) {

            var dialogMethod = method;
            if (!method) {
                dialogMethod = new merchello.Models.FixedRateShippingMethod();
                dialogMethod.shipMethod.shipCountryKey = country.key;
                dialogMethod.shipMethod.providerKey = provider.key;
            }

            var myDialogData = {
                method: dialogMethod,
                country: country,
                provider: provider,
                gatewayResources: $scope.availableFixedRateGatewayResources
            };

            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shippingmethod.html',
                show: true,
                callback: $scope.shippingMethodDialogConfirm,
                dialogData: myDialogData
            });
        };

        $scope.removeFixedRateMethodFromProvider = function(provider, method) {
            provider.removeFixedRateShippingMethod(method);

            var promiseDelete = merchelloCatalogFixedRateShippingService.deleteRateTableShipMethod(method);
            promiseDelete.then(function() {

                notificationsService.success("Shipping Method Deleted");

            }, function(reason) {

                notificationsService.error("Shipping Method Delete Failed", reason.message);

            });
        };

        $scope.loadAllAvailableCountries();
        $scope.loadWarehouses();
        $scope.loadAllShipProviders();
        $scope.loadAllAvailableFixedRateGatewayResources();

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.ShippingController", ['$scope', '$routeParams', '$location', 'notificationsService', 'angularHelper', 'serverValidationManager', 'dialogService', 'merchelloWarehouseService', 'merchelloSettingsService', 'merchelloCatalogShippingService', 'merchelloCatalogFixedRateShippingService', merchello.Controllers.ShippingController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
