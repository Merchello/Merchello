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
	        $scope.setVariables();
	        $scope.loadWarehouses();
	        $scope.loadAllAvailableCountries();
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
         * Once loaded, it calls the loadCountryProviders method for each 
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

	    /**
         * @ngdoc method
         * @name setVariables
         * @function
         * 
         * @description
         * Set up the new variables for the scope.
         */
	    $scope.setVariables = function () {
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
         * Helper method to set the primary warehouse on the scope and to make sure the isDefault flag on 
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
         * choose the default warehouse.
         */
		$scope.changeSelectedCatalog = function (catalogIndex) {
		    if ((typeof catalogIndex) != 'undefined') {
		        if ($scope.primaryWarehouse.warehouseCatalogs.length > catalogIndex) {
		            $scope.selectedCatalog = $scope.primaryWarehouse.warehouseCatalogs[catalogIndex];
		        } else {
		            $scope.selectedCatalog = new merchello.Models.WarehouseCatalog();
		        }
		    } else {
		        var foundDefault = false;
		        for (var i = 0; i < $scope.primaryWarehouse.warehouseCatalogs.length; i++) {
		            if ($scope.primaryWarehouse.warehouseCatalogs[i].isDefault == true) {
		                $scope.selectedCatalog = $scope.primaryWarehouse.warehouseCatalogs[i];
		                foundDefault = true;
		            }
		        }
                if (!foundDefault) {
                    $scope.selectedCatalog = $scope.primaryWarehouse.warehouseCatalogs[0];
                }
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
	    };

	    /**
         * @ngdoc method
         * @name doesWarehouseHaveAddress
         * @function
         * 
         * @description
         * Returns true if the warehouse has an address. Returns false if it does not.
         */
	    $scope.doesWarehouseHaveAddress = function() {
	        var result = true;
	        var warehouse = $scope.primaryWarehouse;
	        if (warehouse.address1 === '' || warehouse.locality === '' || warehouse.address1 === null || warehouse.locality === null) {
                result = false;
            }
	        return result;
	    };

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
         */
		$scope.deleteCountryDialogConfirm = function (country) {

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
         * @name removeMethodFromProviderDialogConfirmation
         * @function
         * 
         * @description
         * Calls the fixed rate shipping service to delete the method passed in via the method parameter.
         * After method is deleted, reload the list of methods for that provider in that country.
         */
		$scope.removeMethodFromProviderDialogConfirmation = function (data) {
		    var promiseDelete = merchelloCatalogShippingService.deleteShipMethod(data.method);
		    promiseDelete.then(function () {
		        data.provider.shipMethods = [];
		        $scope.loadProviderMethods(data.provider, data.country);
		        notificationsService.success("Shipping Method Deleted");
		    }, function (reason) {
		        notificationsService.error("Shipping Method Delete Failed", reason.message);
		    });
		};
		

	    /**
         * @ngdoc method
         * @name removeMethodFromProviderDialog
         * @function
         * 
         * @description
         * Opens the delete confirmation dialog via the Umbraco dialogService.
         * Country and provider passed through dialogService so that on confirm the provider's 
         * methods can be reloaded after the method is deleted.
         */
		$scope.removeMethodFromProviderDialog = function (country, provider, method) {
		    var dialogData = {
                country: country,
                name: method.name,
                method: method,
                provider: provider
    		};
		    dialogService.open({
		        template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
		        show: true,
		        callback: $scope.removeMethodFromProviderDialogConfirmation,
		        dialogData: dialogData
		    });
		}
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
		$scope.addEditShippingMethodDialogOpen = function (country, gatewayProvider, method) {

		    var dialogMethod = method;
		    var provider;
            for (var i = 0; i < $scope.providers.length; i++) {
                if (gatewayProvider.key == $scope.providers[i].key) {
                    provider = new merchello.Models.GatewayProvider($scope.providers[i]);
                    provider.resources = _.map($scope.providers[i].resources, function (resource) {
                        return new merchello.Models.GatewayResource(resource);
                    });
                }
            }
            // If no method exists, create a new, blank one.
		    if (!method) {
		        dialogMethod = new merchello.Models.ShippingMethod();
		        dialogMethod.shipCountryKey = country.key;
		        dialogMethod.providerKey = gatewayProvider.key;
		        dialogMethod.dialogEditorView.editorView = provider.dialogEditorView.editorView;
		    } else {
		        if (method.shipCountryKey === "00000000-0000-0000-0000-000000000000") {
		            method.shipCountryKey = country.key;
		        }
		    }

            // Acquire the provider's available resources.
		    var availableResources = gatewayProvider.resources;

            // Get the editor template for the provider's dialog.
            var templatePage = provider.dialogEditorView.editorView;

		    var myDialogData = {
		        method: dialogMethod,
		        country: country,
		        provider: gatewayProvider,
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
		    var providers = _.map($scope.providers, function (item) {
		        var newProvider = new merchello.Models.GatewayProvider(item);
		        newProvider.resources = _.map(item.resources, function (resource) {
		                return new merchello.Models.GatewayResource(resource);
		        });
		        return newProvider;
		    });
		    // Clean out any placeholder dropdown values for the providers' resources.
            for (var i = 0; i < providers.length; i++) {
                for (var j = 0; j < providers[i].resources.length; j++) {
                    if (providers[i].resources[j].serviceCode === '') {
                        providers[i].resources.splice(j, 1);
                        j -= 1;
                    }
                }
            }
		    var myDialogData = {
		        country: country,
		        provider: dialogProvider,
		        availableProviders: providers
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
         * @name deleteCatalog
         * @function
         * 
         * @description
         * Opens the delete catalog dialog via the Umbraco dialogService.
         */
	    $scope.deleteCatalog = function() {
	        var dialogData = {};
	        dialogData.name = $scope.selectedCatalog.name;
	        dialogData.catalog = $scope.selectedCatalog;
	        dialogService.open({
	            template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
	            show: true,
	            callback: $scope.deleteCatalogConfirm,
	            dialogData: dialogData
	        });
	    };

	    /**
         * @ngdoc method
         * @name deleteCatalogConfirm
         * @function
         * 
         * @description
         * Handles the delete after recieving the catalog to delete from the dialog view/controller
         */
	    $scope.deleteCatalogConfirm = function (data) {
	        var selectedCatalog = new merchello.Models.WarehouseCatalog(data.catalog);
	        var promiseDeleteCatalog = merchelloWarehouseService.deleteWarehouseCatalog(selectedCatalog.key);
	        promiseDeleteCatalog.then(function (responseCatalog) {
	            $scope.loadWarehouses();
	        }, function (reason) {
	            notificationsService.error('Catalog Delete Failed', reason.message);
	        });
        }

	    /**
         * @ngdoc method
         * @name addCountry
         * @function
         * 
         * @description
         * Opens the add country dialog via the Umbraco dialogService.
         */
	    $scope.deleteCountryDialog = function(country) {
	        var dialogData = {};
	        dialogData = country;
	        dialogService.open({
	            template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
	            show: true,
	            callback: $scope.deleteCountryDialogConfirm,
	            dialogData: dialogData
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
		    var availableResources = provider.resources;
		    var templatePage = '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shippingregions.html';

		    if (!method) {
		        dialogMethod = new merchello.Models.ShippingMethod();
		        dialogMethod.shipCountryKey = country.key;
		        dialogMethod.providerKey = provider.key;
		        dialogMethod.dialogEditorView.editorView = '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shippingregions.html';
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
		    var filter = availableCatalogs[0];
		    for (var i = 0; i < $scope.primaryWarehouse.warehouseCatalogs.length; i++) {
		        var catalog = {
		            id: i,
		            name: $scope.primaryWarehouse.warehouseCatalogs[i].name
		        };
		        availableCatalogs.push(catalog);
		        if ($scope.primaryWarehouse.warehouseCatalogs[i].key == $scope.selectedCatalog.key) {
		            filter = availableCatalogs[i];
		        }
            }
		    var myDialogData = {
		        availableCatalogs: availableCatalogs,
		        filter: filter
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
		    //$scope.loadFixedRateProviderMethods(data.country);

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
		    newShippingMethod.name = data.country.name + " " + selectedResource.name;
		    newShippingMethod.providerKey = selectedProvider.key;
		    newShippingMethod.serviceCode = selectedResource.serviceCode;
		    newShippingMethod.shipCountryKey = data.country.key;
		    var promiseAddMethod;
            promiseAddMethod = merchelloCatalogShippingService.addShipMethod(newShippingMethod);
		    promiseAddMethod.then(function () {
		        data.country.shippingGatewayProviders = [];
		        $scope.loadCountryProviders(data.country);
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
         * Handles the add/edit after recieving the dialogData from the dialog view/controller.
         * If the selectedCatalog is set to be default, ensure that original default is no longer default.
         */
		$scope.warehouseCatalogDialogConfirm = function (data) {
		    var selectedCatalog = new merchello.Models.WarehouseCatalog(data.catalog);
		    var promiseUpdateCatalog;
		    if (selectedCatalog.key === "") {
		        promiseUpdateCatalog = merchelloWarehouseService.addWarehouseCatalog(selectedCatalog);
		        selectedCatalog.warehouseKey = $scope.primaryWarehouse.key;
            } else {
		        promiseUpdateCatalog = merchelloWarehouseService.putWarehouseCatalog(selectedCatalog);
		    }
		    promiseUpdateCatalog.then(function(responseCatalog) {
		        $scope.loadWarehouses();
		    }, function(reason) {
		        notificationsService.error('Catalog Update Failed', reason.message);
		    });
		};

	};

	angular.module("umbraco").controller("Merchello.Dashboards.Settings.ShippingController", ['$scope', '$routeParams', '$location', 'notificationsService', 'angularHelper', 'serverValidationManager', 'dialogService', 'merchelloWarehouseService', 'merchelloSettingsService', 'merchelloCatalogShippingService', 'merchelloCatalogFixedRateShippingService', merchello.Controllers.ShippingController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
