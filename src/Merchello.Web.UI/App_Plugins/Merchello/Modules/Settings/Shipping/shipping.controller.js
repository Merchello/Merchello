(function (controllers, undefined) {

	/**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.ShippingController
     * @function
     * 
     * @description
     * The controller for the shipping warehouse and providers management page
     */
	controllers.ShippingController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, dialogService, merchelloWarehouseService, merchelloSettingsService, merchelloCatalogShippingService, merchelloCatalogFixedRateShippingService) {

		$scope.sortProperty = "name";
		$scope.availableCountries = [];
		$scope.availableFixedRateGatewayResources = [];
		$scope.countries = [];
		$scope.warehouses = [];
		$scope.providers = [];
		$scope.newWarehouse = new merchello.Models.Warehouse();
		$scope.primaryWarehouse = new merchello.Models.Warehouse();
		$scope.visible = {
            catalogPanel: true,
			shippingMethodPanel: true,
			warehouseInfoPanel: false,
			warehouseListPanel: true
		};
		$scope.countryToAdd = new merchello.Models.Country();
		$scope.providerToAdd = {};
		$scope.currentShipCountry = {};
	    $scope.selectedCatalog = new merchello.Models.WarehouseCatalog();

	    //--------------------------------------------------------------------------------------
	    // Initialization methods
	    //--------------------------------------------------------------------------------------

	    /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
	    $scope.init = function () {

	        $scope.loadWarehouses();
	        $scope.loadAllAvailableCountries();
	        //$scope.loadAllAvailableFixedRateGatewayResources();

	    };

	    /**
         * @ngdoc method
         * @name loadAllAvailableCountries
         * @function
         * 
         * @description
         * Load the countries from the settings service, then wrap the results
         * in Merchello models and add to the scope via the availableCountries collection.
         */
	    $scope.loadAllAvailableCountries = function () {

	        var promiseAllCountries = merchelloSettingsService.getAllCountries();
	        promiseAllCountries.then(function (allCountries) {

	            $scope.availableCountries = _.map(allCountries, function (country) {
	                return new merchello.Models.Country(country);
	            });

	            // Add Everywhere Else as an option
	            var everywhereElseCountry = new merchello.Models.Country();
	            everywhereElseCountry.key = "7501029f-5ab3-4733-935d-1dd37b37bf8";
	            everywhereElseCountry.countryCode = "ELSE";
	            everywhereElseCountry.name = "Everywhere Else";
	            $scope.availableCountries.push(everywhereElseCountry);

	        }, function (reason) {

	            notificationsService.error("Available Countries Load Failed", reason.message);

	        });

	    };

	    /**
         * @ngdoc method
         * @name loadAllAvailableFixedRateGatewayResources
         * @function
         * 
         * @description
         * Load the fixed rate shipping gateway resources from the fixed rate shipping gateway service, then wrap the results
         * in Merchello models and add to the scope via the availableFixedRateGatewayResources collection.
         */
	    $scope.loadAllAvailableFixedRateGatewayResources = function () {

	        var promiseAllResources = merchelloCatalogFixedRateShippingService.getAllFixedRateGatewayResources();
	        promiseAllResources.then(function (allResources) {

	            $scope.availableFixedRateGatewayResources = _.map(allResources, function (resource) {
	                return new merchello.Models.GatewayResource(resource);
	            });

	        }, function (reason) {

	            notificationsService.error("Available Fixed Rate Gateway Resources Load Failed", reason.message);

	        });

	    };

	    /**
         * @ngdoc method
         * @name loadAllAvailableGatewayResources
         * @function
         * 
         * @description
         * Load the shipping gateway resources from the shipping gateway service, then wrap the results
         * in Merchello models and add to the scope via the providers collection in the resources collection.
         */
	    $scope.loadAllAvailableGatewayResources = function (shipProvider) {

	        var promiseAllResources = merchelloCatalogShippingService.getAllShipGatewayResourcesForProvider(shipProvider);
	        promiseAllResources.then(function (allResources) {

	            shipProvider.resources = _.map(allResources, function (resource) {
	                return new merchello.Models.GatewayResource(resource);
	            });

	        }, function (reason) {

	            notificationsService.error("Available Gateway Resources Load Failed", reason.message);

	        });

	    };

	    /**
         * @ngdoc method
         * @name loadAllShipProviders
         * @function
         * 
         * @description
         * Load the shipping gateway providers from the shipping gateway service, then wrap the results
         * in Merchello models and add to the scope via the providers collection.
         */
	    $scope.loadAllShipProviders = function () {

	        var promiseAllProviders = merchelloCatalogShippingService.getAllShipGatewayProviders();
	        promiseAllProviders.then(function (allProviders) {

	            $scope.providers = _.map(allProviders, function (providerFromServer) {
	                return new merchello.Models.GatewayProvider(providerFromServer);
	            });

	            _.each($scope.providers, function (element, index, list) {
	                $scope.loadAllAvailableGatewayResources(element);
	            });

	            $scope.loadCountries();

	        }, function (reason) {

	            notificationsService.error("Available Ship Providers Load Failed", reason.message);

	        });

	    };

	    /**
         * @ngdoc method
         * @name loadCountries
         * @function
         * 
         * @description
         * Load the countries from the shipping service, then wrap the results
         * in Merchello models and add to the scope via the countries collection.
         * Once loaded, it calls the loadFixedRateCountryProviders method for each 
         * country.
         */
	    $scope.loadCountries = function () {

	        if ($scope.primaryWarehouse.warehouseCatalogs.length > 0) {

	            var catalogKey = $scope.selectedCatalog.key;

	            var promiseShipCountries = merchelloCatalogShippingService.getWarehouseCatalogShippingCountries(catalogKey);
	            promiseShipCountries.then(function (shipCountriesFromServer) {

	                $scope.countries = _.map(shipCountriesFromServer, function (shippingCountryFromServer) {
	                    return new merchello.Models.ShippingCountry(shippingCountryFromServer);
	                });

	                _.each($scope.countries, function (element, index, list) {
	                    $scope.loadCountryProviders(element);
	                    //$scope.loadFixedRateCountryProviders(element);
	                });

	                $scope.loaded = true;
	                $scope.preValuesLoaded = true;

	            }, function (reason) {

	                notificationsService.error("Shipping Countries Load Failed", reason.message);

	            });
	        }
	    };

	    /**
         * @ngdoc method
         * @name loadCountryProviders
         * @function
         * 
         * @description
         * Load the shipping gateway providers from the shipping gateway service, then wrap the results
         * in Merchello models and add to the scope via the shippingGatewayProviders collection on the country model.  After
         * load is complete, it calls the loadProviderMethods to load in the methods.
         */
	    $scope.loadCountryProviders = function (country) {

	        if (country) {
	            var promiseProviders = merchelloCatalogShippingService.getAllShipCountryProviders(country);
	            promiseProviders.then(function (providerFromServer) {

	                if (providerFromServer.length > 0) {

	                    _.each(providerFromServer, function (element, index, list) {
	                        var newProvider = new merchello.Models.ShippingGatewayProvider(element);
	                        // Need this to get the name for now.
	                        var tempGlobalProvider = _.find($scope.providers, function (p) { return p.key == newProvider.key; });
	                        newProvider.name = tempGlobalProvider.name;
	                        newProvider.typeFullName = tempGlobalProvider.typeFullName;
	                        newProvider.resources = tempGlobalProvider.resources;
	                        newProvider.shipMethods = [];
	                        country.shippingGatewayProviders.push(newProvider);
	                        $scope.loadProviderMethods(newProvider, country);
	                    });

	                    $scope.loaded = true;
	                    $scope.preValuesLoaded = true;
	                }

	            }, function (reason) {

	                notificationsService.error("Fixed Rate Shipping Countries Providers Load Failed", reason.message);

	            });
	        }
	    };

	    /**
         * @ngdoc method
         * @name loadFixedRateCountryProviders
         * @function
         * 
         * @description
         * Load the fixed rate shipping gateway providers from the fixed rate shipping gateway service, then wrap the results
         * in Merchello models and add to the scope via the shippingGatewayProviders collection on the country model.  After
         * load is complete, it calls the loadFixedRateProviderMethods to load in the methods.
         */
	    $scope.loadFixedRateCountryProviders = function (country) {

	        if (country) {
	            var promiseProviders = merchelloCatalogFixedRateShippingService.getAllShipCountryFixedRateProviders(country.key);
	            promiseProviders.then(function (providerFromServer) {

	                if (providerFromServer.length > 0) {

	                    _.each(providerFromServer, function (element, index, list) {
	                        var newProvider = new merchello.Models.ShippingGatewayProvider(element);
	                        // Need this to get the name for now.
	                        var tempGlobalProvider = _.find($scope.providers, function (p) { return p.key == newProvider.key; });
	                        newProvider.name = tempGlobalProvider.name;
	                        newProvider.typeFullName = tempGlobalProvider.typeFullName;
	                        newProvider.resources = $scope.availableFixedRateGatewayResources;
	                        newProvider.shipMethods = [];
	                        country.shippingGatewayProviders.push(newProvider);
	                        $scope.loadFixedRateProviderMethods(country);
	                    });
	                }

	            }, function (reason) {

	                notificationsService.error("Fixed Rate Shipping Countries Providers Load Failed", reason.message);

	            });
	        }
	    };

	    /**
         * @ngdoc method
         * @name loadFixedRateProviderMethods
         * @function
         * 
         * @description
         * Load the fixed rate shipping gateway methods from the fixed rate shipping gateway service, then wrap the results
         * in Merchello models and add to the scope via the shipMethods collection on the gateway provider model.
         */
	    $scope.loadFixedRateProviderMethods = function (country) {

	        if (country) {
	            var promiseMethods = merchelloCatalogFixedRateShippingService.getAllFixedRateProviderMethods(country.key);
	            promiseMethods.then(function (methodsFromServer) {

	                if (methodsFromServer.length > 0) {

	                    _.each(methodsFromServer, function (element, index, list) {
	                        var newMethod = new merchello.Models.FixedRateShippingMethod(element);

	                        var shippingGatewayProvider = _.find(country.shippingGatewayProviders, function (p) { return p.key == newMethod.shipMethod.providerKey; });
	                        shippingGatewayProvider.shipMethods.push(newMethod);
	                    });
	                }

	            }, function (reason) {

	                notificationsService.error("Fixed Rate Shipping Countries Methods Load Failed", reason.message);

	            });
	        }
	    };

	    /**
         * @ngdoc method
         * @name loadShipMethods
         * @function
         * 
         * @description
         * Load the shipping methods from the shipping gateway service, then wrap the results
         * in Merchello models and add to the scope via the provider in the shipMethods collection.
         */
	    $scope.loadProviderMethods = function (shipProvider, country) {

	        var promiseShipMethods = merchelloCatalogShippingService.getShippingProviderShipMethodsByCountry(shipProvider, country);
	        promiseShipMethods.then(function (shipMethods) {

	            shipProvider.shipMethods = _.map(shipMethods, function (method) {
	                return new merchello.Models.ShippingMethod(method);
	            });

	        }, function (reason) {

	            notificationsService.error("Available Shipping Methods Load Failed", reason.message);

	        });

	    };

	    /**
         * @ngdoc method
         * @name loadWarehouses
         * @function
         * 
         * @description
         * Load the warehouses from the warehouse service, then wrap the results
         * in Merchello models and add to the scope via the warehouses collection.
         * Once loaded, it calls the loadCountries method.
         */
	    $scope.loadWarehouses = function () {

	        var promiseWarehouses = merchelloWarehouseService.getDefaultWarehouse(); // Only a default warehouse in v1
	        promiseWarehouses.then(function (warehouseFromServer) {

	            $scope.warehouses.push(new merchello.Models.Warehouse(warehouseFromServer));

	            $scope.changePrimaryWarehouse();

	            $scope.loadAllShipProviders();

	        }, function (reason) {

	            notificationsService.error("Warehouses Load Failed", reason.message);

	        });

	    };

	    $scope.init();

		//--------------------------------------------------------------------------------------
		// Helper methods
		//--------------------------------------------------------------------------------------

	    /**
         * @ngdoc method
         * @name changePrimaryWarehouse
         * @function
         * 
         * @description
         * Helper method to set the primary warehouse on the scope and to make sure the idDefault flag on 
         * all warehouses is set properly.  If a warehouse is passed in, then it will find that warehouse
         * and set it as the primary and set isDefault to true.  All other warehouses will have their 
         * isDefault flag reset to false.  If no warehouse is passed in (usually on loading data) then the 
         * warehouse that has the isDefault == true will be set as the primary warehouse on the scope.
         */
		$scope.changePrimaryWarehouse = function (warehouse) {
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
		    $scope.changeSelectedCatalog();
		};

	    /**
         * @ngdoc method
         * @name changeSelectedCatalog
         * @function
         * 
         * @description
         * Helper method to change between the selected catalog on the screen. If catalogIndex is passed
         * in, then the function will select the catalog at that index if it exists. If it doesn't, then
         * choose the first warehouse.
         */
		$scope.changeSelectedCatalog = function (catalogIndex) {
		    if ((typeof catalogIndex) != 'undefined') {
		        if ($scope.primaryWarehouse.warehouseCatalogs.length > catalogIndex) {
		            $scope.selectedCatalog = $scope.primaryWarehouse.warehouseCatalogs[catalogIndex];
		        } else {
		            $scope.selectedCatalog = new merchello.Models.WarehouseCatalog();
		        }
		    } else {
		        $scope.selectedCatalog = $scope.primaryWarehouse.warehouseCatalogs[0];
		    }
		};

	    /**
         * @ngdoc method
         * @name countryHasProvinces
         * @function
         * 
         * @description
         * Helper method to determine if the provided country has provinces or not.
         */
        $scope.countryHasProvinces = function(country) {
            var result = false;
            if (country.provinces.length > 0) {
                result = true;
            }
            return result;
        }

	    //--------------------------------------------------------------------------------------
	    // Event Handlers
	    //--------------------------------------------------------------------------------------

	    /**
         * @ngdoc method
         * @name deleteCountry
         * @function
         * 
         * @description
         * Calls the shipping service to delete the country passed in via the country parameter.
         * When complete, the countries are reloaded from the api to get the latest from the database.
         *
         * TODO: make a dialog to confirm delete.  Can a country with methods be deleted?
         */
		$scope.deleteCountry = function (country) {

		    var promiseDelete = merchelloCatalogShippingService.deleteShipCountry(country.key);
		    promiseDelete.then(function () {

		        notificationsService.success("Shipping Country Deleted");

		        $scope.loadCountries();

		    }, function (reason) {

		        notificationsService.error("Shipping Country Delete Failed", reason.message);

		    });
		};

	    /**
         * @ngdoc method
         * @name removeFixedRateMethodFromProvider
         * @function
         * 
         * @description
         * Calls the fixed rate shipping service to delete the method passed in via the method parameter.
         *
         * TODO: make a dialog to confirm delete?
         */
		$scope.removeMethodFromProvider = function (provider, method, country) {

		    var promiseDelete;
		    if (provider.isFixedRate()) {
		        promiseDelete = merchelloCatalogFixedRateShippingService.deleteRateTableShipMethod(method);
		    } else {
		        promiseDelete = merchelloCatalogShippingService.deleteShipMethod(method);
		    }
		    promiseDelete.then(function () {

		        provider.shipMethods = [];
		        $scope.loadProviderMethods(provider, country);
		        $scope.loadFixedRateProviderMethods(country);

		        notificationsService.success("Shipping Method Deleted");

		    }, function (reason) {

		        notificationsService.error("Shipping Method Delete Failed", reason.message);

		    });
		};

	    //--------------------------------------------------------------------------------------
	    // Dialog methods
	    //--------------------------------------------------------------------------------------

	    /**
         * @ngdoc method
         * @name addCountry
         * @function
         * 
         * @description
         * Opens the add country dialog via the Umbraco dialogService.
         */
		$scope.addCountry = function () {
		    var dialogData = {};
		    dialogData.availableCountries = $scope.availableCountries;
		    dialogService.open({
		        template: '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shipping.addcountry.html',
		        show: true,
		        callback: $scope.addCountryDialogConfirm,
		        dialogData: dialogData
		    });
		};

	    /**
         * @ngdoc method
         * @name addCountryDialogConfirm
         * @function
         * 
         * @description
         * Handles the save after recieving the country to add from the dialog view/controller
         */
		$scope.addCountryDialogConfirm = function (dialogData) {
		    var countryOnCatalog = _.find($scope.countries, function (shipCountry) { return shipCountry.countryCode == dialogData.selectedCountry.countryCode; });
		    if (!countryOnCatalog) {

		        var catalogKey = $scope.selectedCatalog.key;

		        var promiseShipCountries = merchelloCatalogShippingService.newWarehouseCatalogShippingCountry(catalogKey, dialogData.selectedCountry.countryCode);
		        promiseShipCountries.then(function (shippingCountryFromServer) {

		            $scope.countries.push(new merchello.Models.ShippingCountry(shippingCountryFromServer));

		        }, function (reason) {

		            notificationsService.error("Shipping Countries Create Failed", reason.message);

		        });
		    }
		};

	    /**
        * @ngdoc method
        * @name addEditShippingMethodDialogOpen
        * @function
        * 
        * @description
        * Opens the add/edit shipping method dialog via the Umbraco dialogService.
        */
		$scope.addEditShippingMethodDialogOpen = function (country, provider, method) {

		    var dialogMethod = method;

		    //if (!method) {
		    //    if (provider.isFixedRate()) {
		    //        dialogMethod = new merchello.Models.FixedRateShippingMethod();
		    //        dialogMethod.shipMethod.shipCountryKey = country.key;
		    //        dialogMethod.shipMethod.providerKey = provider.key;
		    //        dialogMethod.shipMethod.dialogEditorView.editorView = '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shippingmethod.html';
		    //    } else {
		    dialogMethod = new merchello.Models.ShippingMethod();
		    dialogMethod.shipCountryKey = country.key;
		    dialogMethod.providerKey = provider.key;
		    dialogMethod.dialogEditorView.editorView = '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shippingmethod.html';
		    //}
		    //}

		    var availableResources = [];
		    if (provider.isFixedRate()) {
		        availableResources = $scope.availableFixedRateGatewayResources;
		    } else {
		        availableResources = provider.resources;
		    }

		    var templatePage = '';
		    templatePage = '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shippingmethod.html';
		    if (!provider.isFixedRate()) {
		        if (dialogMethod.dialogEditorView.editorView) {
		            templatePage = dialogMethod.dialogEditorView.editorView;
		        }
		    }

		    var myDialogData = {
		        method: dialogMethod,
		        country: country,
		        provider: provider,
		        gatewayResources: availableResources
		    };

		    dialogService.open({
		        template: templatePage,
		        show: true,
		        callback: $scope.shippingMethodDialogConfirm,
		        dialogData: myDialogData
		    });
		};

	    /**
        * @ngdoc method
        * @name addEditShippingProviderDialogOpen
        * @function
        * 
        * @description
        * Opens the shipping provider dialog via the Umbraco dialogService.
        */
		$scope.addEditShippingProviderDialogOpen = function (country, provider) {

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

	    /**
        * @ngdoc method
        * @name addEditWarehouseCatalogDialogOpen
        * @function
        * 
        * @description
        * Opens the warehouse catalog dialog via the Umbraco dialogService.
        */
		$scope.addEditWarehouseCatalogDialogOpen = function (warehouse, catalog) {

		    var dialogCatalog = catalog;
		    if (!catalog) {
		        dialogCatalog = new merchello.Models.WarehouseCatalog();
		    }

		    var myDialogData = {
		        warehouseKey: warehouse.key,
		        catalog: dialogCatalog
		    };

		    dialogService.open({
		        template: '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shipping.addeditcatalog.html',
		        show: true,
		        callback: $scope.warehouseCatalogDialogConfirm,
		        dialogData: myDialogData
		    });

		};

	    /**
         * @ngdoc method
         * @name addWarehouse
         * @function
         * 
         * @description
         * Opens the edit warehouse dialog via the Umbraco dialogService.
         */
		$scope.addWarehouse = function (warehouse) {
		    if (warehouse == undefined) {
		        warehouse = new merchello.Models.Warehouse();
		        warehouse.key = "no key created";
		    }

		    var dialogData = {}
		    dialogData.warehouse = warehouse;
		    dialogData.availableCountries = $scope.availableCountries;

		    dialogService.open({
		        template: '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shipping.editwarehouse.html',
		        show: true,
		        callback: $scope.addWarehouseDialogConfirm,
		        dialogData: dialogData
		    });
		};

	    /**
         * @ngdoc method
         * @name addWarehouseDialogConfirm
         * @function
         * 
         * @description
         * Handles the add/edit after recieving the dialogData from the dialog view/controller
         */
		$scope.addWarehouseDialogConfirm = function (dialogData) {
		    var warehouse = dialogData.warehouse;
		    var promiseWarehouseSave = merchelloWarehouseService.save(warehouse); // Only a default warehouse in v1
		    promiseWarehouseSave.then(function () {

		        notificationsService.success("Warehouse Saved", "");

		        if ($scope.newWarehouse.isDefault) {
		            $scope.changePrimaryWarehouse(warehouse);
		        }

		    }, function (reason) {

		        notificationsService.error("Warehouses Save Failed", reason.message);

		    });
		};

	    /**
         * @ngdoc method
         * @name deleteWarehouse
         * @function
         * 
         * @description
         * Opens the delete warehouse dialog via the Umbraco dialogService.
         */
		$scope.deleteWarehouse = function (warehouse) {
		    if (warehouse != undefined) {
		        dialogService.open({
		            template: '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shipping.deletewarehouse.html',
		            show: true,
		            callback: $scope.deleteWarehouseDialogConfirm,
		            dialogData: warehouse
		        });
		    }
		};

	    /**
         * @ngdoc method
         * @name deleteWarehouseDialogConfirm
         * @function
         * 
         * @description
         * Handles the delete after recieving the warehouse to delete from the dialog view/controller
         */
		$scope.deleteWarehouseDialogConfirm = function (warehouse) {

		    // Todo: call API method to delete warehouse and then reload warehouses from API

		};

	    /**
        * @ngdoc method
        * @name editRegionalShippingRatesDialogOpen
        * @function
        * 
        * @description
        * Opens the edit regional shipping rates dialog via the Umbraco dialogService.
        */
		$scope.editRegionalShippingRatesDialogOpen = function (country, provider, method) {
		    var dialogMethod = method;

		    if (!method) {
		        if (provider.isFixedRate()) {
		            dialogMethod = new merchello.Models.FixedRateShippingMethod();
		            dialogMethod.shipMethod.shipCountryKey = country.key;
		            dialogMethod.shipMethod.providerKey = provider.key;
		            dialogMethod.shipMethod.dialogEditorView.editorView = '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shippingregions.html';
		        } else {
		            dialogMethod = new merchello.Models.ShippingMethod();
		            dialogMethod.shipCountryKey = country.key;
		            dialogMethod.providerKey = provider.key;
		            dialogMethod.dialogEditorView.editorView = '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shippingregions.html';
		        }
		    }

		    var availableResources = [];
		    if (provider.isFixedRate()) {
		        availableResources = $scope.availableFixedRateGatewayResources;
		    } else {
		        availableResources = provider.resources;
		    }

		    var templatePage = '';
		    templatePage = '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shippingregions.html';
		    if (!provider.isFixedRate()) {
		        if (dialogMethod.dialogEditorView.editorView) {
		            templatePage = dialogMethod.dialogEditorView.editorView;
		        }
		    }

		    var myDialogData = {
		        method: dialogMethod,
		        country: country,
		        provider: provider,
		        gatewayResources: availableResources
		    };

		    dialogService.open({
		        template: templatePage,
		        show: true,
		        callback: $scope.shippingMethodDialogConfirm,
		        dialogData: myDialogData
		    });
		};

	    /**
         * @ngdoc method
         * @name selectCatalogDialogConfirm
         * @function
         * 
         * @description
         * Handles the catalog selection after recieving the dialogData from the dialog view/controller
         */
		$scope.selectCatalogDialogConfirm = function (data) {

		    var index = data.filter.id;
		    $scope.selectedCatalog = $scope.primaryWarehouse.warehouseCatalogs[index];
		    $scope.countries = [];
		    // Load the countries associated with this catalog.
		    $scope.loadCountries();
		};

	    /**
        * @ngdoc method
        * @name selectCatalogDialogOpen
        * @function
        * 
        * @description
        * Opens the catalog selection dialog via the Umbraco dialogService.
        */
		$scope.selectCatalogDialogOpen = function () {

		    var availableCatalogs = [];
		    for (var i = 0; i < $scope.primaryWarehouse.warehouseCatalogs.length; i++) {
		        var catalog = {
		            id: i,
		            name: $scope.primaryWarehouse.warehouseCatalogs[i].name
		        };
		        availableCatalogs.push(catalog);
		    }

		    var myDialogData = {
		        availableCatalogs: availableCatalogs,
		        filter: availableCatalogs[0]
		    };
		    dialogService.open({
		        template: '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shipping.selectcatalog.html',
		        show: true,
		        callback: $scope.selectCatalogDialogConfirm,
		        dialogData: myDialogData
		    });

		};

	    /**
         * @ngdoc method
         * @name shippingMethodDialogConfirm
         * @function
         * 
         * @description
         * Handles the edit after recieving the dialogData from the dialog view/controller
         */
		$scope.shippingMethodDialogConfirm = function (data) {

		    data.provider.shipMethods = [];
		    $scope.loadProviderMethods(data.provider, data.country);
		    $scope.loadFixedRateProviderMethods(data.country);

		};

	    /**
         * @ngdoc method
         * @name shippingProviderDialogConfirm
         * @function
         * 
         * @description
         * Handles the edit after recieving the dialogData from the dialog view/controller
         */
		$scope.shippingProviderDialogConfirm = function (data) {

		    var selectedProvider = data.provider;
		    var selectedResource = data.resource;

		    var newShippingMethod = new merchello.Models.ShippingMethod();
		    var newFixedRateShippingMethod = new merchello.Models.FixedRateShippingMethod();
		    newShippingMethod.name = data.country.name + " " + selectedResource.name;
		    newShippingMethod.providerKey = selectedProvider.key;
		    newShippingMethod.serviceCode = selectedResource.serviceCode;
		    newShippingMethod.shipCountryKey = data.country.key;
		    if (selectedProvider.isFixedRate()) {
		        newFixedRateShippingMethod.shipMethod = newShippingMethod;
		    }

		    var promiseAddMethod;
		    if (selectedProvider.isFixedRate()) {
		        promiseAddMethod = merchelloCatalogFixedRateShippingService.createRateTableShipMethod(newFixedRateShippingMethod);
		    } else {
		        promiseAddMethod = merchelloCatalogShippingService.addShipMethod(newShippingMethod);
		    }
		    promiseAddMethod.then(function () {

		        data.country.shippingGatewayProviders = [];
		        $scope.loadCountryProviders(data.country);
		        $scope.loadFixedRateCountryProviders(data.country);

		    }, function (reason) {

		        notificationsService.error("Shipping Provider / Initial Method Create Failed", reason.message);

		    });
		};

	    /**
         * @ngdoc method
         * @name warehouseCatalogDialogConfirm
         * @function
         * 
         * @description
         * Handles the add/edit after recieving the dialogData from the dialog view/controller
         */
		$scope.warehouseCatalogDialogConfirm = function (data) {

		    var selectedCatalog = data.catalog;

		    /* TODO: Add API call functionality to either save an edited catalog or create a new one */

		    if (selectedCatalog.key === "") {
		        // TODO: Remove the following line. It's just there for mocking an unique key */
		        selectedCatalog.key = Math.floor(Math.random() * 1000);
		        selectedCatalog.warehouseKey = $scope.primaryWarehouse.key;
		        $scope.primaryWarehouse.warehouseCatalogs.push(selectedCatalog);
		    }

		};


	};

	angular.module("umbraco").controller("Merchello.Dashboards.Settings.ShippingController", ['$scope', '$routeParams', '$location', 'notificationsService', 'angularHelper', 'serverValidationManager', 'dialogService', 'merchelloWarehouseService', 'merchelloSettingsService', 'merchelloCatalogShippingService', 'merchelloCatalogFixedRateShippingService', merchello.Controllers.ShippingController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
