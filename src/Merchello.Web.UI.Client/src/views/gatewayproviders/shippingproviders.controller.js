/**
 * @ngdoc service
 * @name Merchello.Backoffice.ShippingProvidersController
 *
 * @description
 * The controller for the shipment provider view
 */
angular.module('merchello').controller('Merchello.Backoffice.ShippingProvidersController',
    ['$scope', 'notificationsService', 'dialogService', 'merchelloTabsFactory',
    'settingsResource', 'warehouseResource', 'shippingGatewayProviderResource', 'dialogDataFactory',
    'settingDisplayBuilder', 'warehouseDisplayBuilder', 'warehouseCatalogDisplayBuilder', 'countryDisplayBuilder',
        'shippingGatewayProviderDisplayBuilder', 'shipCountryDisplayBuilder',
    function($scope, notificationsService, dialogService, merchelloTabsFactory,
             settingsResource, warehouseResource, shippingGatewayProviderResource, dialogDataFactory,
             settingDisplayBuilder, warehouseDisplayBuilder, warehouseCatalogDisplayBuilder, countryDisplayBuilder,
             shippingGatewayProviderDisplayBuilder, shipCountryDisplayBuilder) {

        $scope.loaded = true;
        $scope.tabs = [];
        $scope.countries = [];
        $scope.warehouses = [];
        $scope.primaryWarehouse = {};
        $scope.selectedCatalog = {};
        $scope.providers = [];
        $scope.primaryWarehouseAddress = {};
        $scope.visible = {
            catalogPanel: true,
            shippingMethodPanel: true,
            warehouseInfoPanel: false,
            warehouseListPanel: true
        };

        // exposed methods
        $scope.loadCountries = loadCountries;
        $scope.addCountry = addCountry;
        $scope.deleteCountryDialog = deleteCountryDialog;
        $scope.addEditWarehouseDialogOpen = addEditWarehouseDialogOpen;
        $scope.addEditWarehouseCatalogDialogOpen = addEditWarehouseCatalogDialogOpen;
        $scope.changeSelectedCatalogOpen = changeSelectedCatalogOpen;
        $scope.deleteWarehouseCatalogOpen = deleteWarehouseCatalogOpen;

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
        function init() {
            loadWarehouses();
            $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
            $scope.tabs.setActive('shipping');
        }

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
        function loadWarehouses() {
            var promiseWarehouses = warehouseResource.getDefaultWarehouse(); // Only a default warehouse in v1
            promiseWarehouses.then(function (warehouses) {
                $scope.warehouses.push(warehouseDisplayBuilder.transform(warehouses));
                changePrimaryWarehouse();
                loadCountries();
                //loadAllShipProviders();
            }, function (reason) {
                notificationsService.error("Warehouses Load Failed", reason.message);
            });
        }

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
        function loadCountries() {
            if ($scope.primaryWarehouse.warehouseCatalogs.length > 0) {
                var catalogKey = $scope.selectedCatalog.key;
                var promiseShipCountries = shippingGatewayProviderResource.getWarehouseCatalogShippingCountries(catalogKey);
                promiseShipCountries.then(function (shipCountries) {
                    var transformed = shipCountryDisplayBuilder.transform(shipCountries);
                    $scope.countries = _.sortBy(
                        transformed, function(country) {
                        return country.name;
                    });
                    var elseCountry = _.find($scope.countries, function(country) {
                        return country.countryCode === 'ELSE';
                    });
                    if(elseCountry !== null && elseCountry !== undefined) {
                        $scope.countries = _.reject($scope.countries, function(country) {
                            return country.countryCode === 'ELSE';
                        });
                        $scope.countries.push(elseCountry);
                    }
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Shipping Countries Load Failed", reason.message);
                });
            }
        }

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
        function changePrimaryWarehouse(warehouse) {
            $scope.primaryWarehouse = _.find($scope.warehouses, function(warehouse) {
                   return warehouse.isDefault;
            });
            $scope.primaryWarehouseAddress = $scope.primaryWarehouse.getAddress();
            //changeSelectedCatalog();
            $scope.selectedCatalog = _.find($scope.primaryWarehouse.warehouseCatalogs, function(catalog) {
                return catalog.isDefault === true;
            });
        }

        /**
         * @ngdoc method
         * @name changeSelectedCatalog
         * @function
         *
         * @description
         *
         */
        function changeSelectedCatalogOpen() {
            var dialogData = dialogDataFactory.createChangeWarehouseCatalogDialogData();
            dialogData.warehouse = $scope.primaryWarehouse;
            dialogData.selectedWarehouseCatalog = $scope.selectedCatalog;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.warehousecatalog.select.html',
                show: true,
                callback: selectCatalogDialogConfirm,
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
        function addCountry() {
            var promiseAllCountries = settingsResource.getAllCountries();
            promiseAllCountries.then(function (allCountries) {
                var countries = countryDisplayBuilder.transform(allCountries);

                // Add Everywhere Else as an option
                var elseCountry = countryDisplayBuilder.createDefault();
                elseCountry.key = '7501029f-5ab3-4733-935d-1dd37b37bf8';
                elseCountry.countryCode = 'ELSE';
                // TODO this should be localized
                elseCountry.name = 'Everywhere Else';
                countries.push(elseCountry);

                // we only want available countries that are not already in use
                var availableCountries = _.sortBy(
                    _.filter(countries, function(country) {
                        var found = _.find($scope.countries, function(assigned) {
                            return country.countryCode === assigned.countryCode;
                        });
                        return found === undefined || found === null;
                    }), function(country) {
                        return country.name;
                    });

                // construct the dialog data for the add ship country dialog
                var dialogData = dialogDataFactory.createAddShipCountryDialogData();
                dialogData.availableCountries = availableCountries;
                dialogData.selectedCountry = availableCountries[0];

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.addcountry.html',
                    show: true,
                    callback: addCountryDialogConfirm,
                    dialogData: dialogData
                });

            }, function (reason) {
                notificationsService.error("Available Countries Load Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name addCountryDialogConfirm
         * @function
         *
         * @description
         * Handles the save after recieving the country to add from the dialog view/controller
         */
        function addCountryDialogConfirm(dialogData) {
            $scope.preValuesLoaded = false;
            var catalogKey = $scope.selectedCatalog.key;
            var promiseShipCountries = shippingGatewayProviderResource.newWarehouseCatalogShippingCountry(catalogKey, dialogData.selectedCountry.countryCode);
            promiseShipCountries.then(function () {
                loadCountries()
            }, function (reason) {
                notificationsService.error("Shipping Countries Create Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name addEditWarehouseDialogOpen
         * @function
         *
         * @description
         * Handles opening a dialog for adding or editing a warehouse
         */
        function addEditWarehouseDialogOpen(warehouse) {
            // todo this will need to be refactored once we expose multiple warehouse
            var dialogData = dialogDataFactory.createAddEditWarehouseDialogData();
            dialogData.warehouse = warehouse;
            var promise = settingsResource.getAllCountries();
            promise.then(function(countries) {
                dialogData.availableCountries  = countryDisplayBuilder.transform(countries);
                dialogData.selectedCountry = _.find(dialogData.availableCountries, function(country) {
                    return country.countryCode === warehouse.countryCode;
                });
                if (dialogData.selectedCountry === undefined) {
                    dialogData.selectedCountry = dialogData.availableCountries[0];
                }
                if(dialogData.selectedCountry.provinces.length > 0) {
                    dialogData.selectedProvince = _.find(dialogData.selectedCountry.provinces, function(province) {
                        return province.code === warehouse.region;
                    });
                }
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.warehouse.addedit.html',
                    show: true,
                    callback: addEditWarehouseDialogConfirm,
                    dialogData: dialogData
                });
            });
        }

        /**
         * @ngdoc method
         * @name addEditWarehouseDialogConfirm
         * @function
         *
         * @description
         * Handles the saving of warehouse information
         */
        function addEditWarehouseDialogConfirm(dialogData) {
            $scope.preValuesLoaded = false;
            var promise = warehouseResource.save(dialogData.warehouse);
            promise.then(function() {
              loadWarehouses();
            });
        }

        /**
         * @ngdoc method
         * @name addEditWarehouseCatalogDialogOpen
         * @function
         *
         * @description
         * Opens the warehouse catalog dialog via the Umbraco dialogService.
         */
        function addEditWarehouseCatalogDialogOpen(catalog) {
            var dialogData = dialogDataFactory.createAddEditWarehouseCatalogDialogData();
            dialogData.warehouse = $scope.primaryWarehouse;
            if(catalog === undefined || catalog === null) {
                dialogData.warehouseCatalog = warehouseCatalogDisplayBuilder.createDefault();
                dialogData.warehouseCatalog.warehouseKey = dialogData.warehouse.key;
            } else {
                dialogData.warehouseCatalog = catalog;
            }

            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.warehousecatalog.addedit.html',
                show: true,
                callback: warehouseCatalogDialogConfirm,
                dialogData: dialogData
            });

        }

        /**
         * @ngdoc method
         * @name deleteCatalog
         * @function
         *
         * @description
         * Opens the delete catalog dialog via the Umbraco dialogService.
         */
        function deleteWarehouseCatalogOpen() {
            var dialogData = dialogDataFactory.createDeleteWarehouseCatalogDialogData();
            dialogData.name = $scope.selectedCatalog.name;
            dialogData.warehouseCatalog = $scope.selectedCatalog;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                show: true,
                callback: deleteWarehouseCatalogConfirm,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name deleteCatalogConfirm
         * @function
         *
         * @description
         * Handles the delete after recieving the catalog to delete from the dialog view/controller
         */
        function deleteWarehouseCatalogConfirm(dialogData) {
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            if(dialogData.warehouseCatalog.isDefault === false)
            {
                var promiseDeleteCatalog = warehouseResource.deleteWarehouseCatalog(dialogData.warehouseCatalog.key);
                promiseDeleteCatalog.then(function (responseCatalog) {
                    $scope.warehouses = [];
                    init();
                }, function (reason) {
                    notificationsService.error('Catalog Delete Failed', reason.message);
                });
            } else {
                notificationsService.warning('Cannot delete the default catalog.')
            }
        }

        /**
         * @ngdoc method
         * @name addCountry
         * @function
         *
         * @description
         * Opens the add country dialog via the Umbraco dialogService.
         */
        function deleteCountryDialog(country) {
            var dialogData = dialogDataFactory.createDeleteShipCountryDialogData();
            dialogData.country = country;
            dialogData.name = country.name;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                show: true,
                callback: deleteCountryDialogConfirm,
                dialogData: dialogData
            });
        }

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
        function deleteCountryDialogConfirm(dialogData) {
            $scope.preValuesLoaded = true;
            var promiseDelete = shippingGatewayProviderResource.deleteShipCountry(dialogData.country.key);
            promiseDelete.then(function () {
                notificationsService.success("Shipping Country Deleted");
                loadCountries();
            }, function (reason) {
                notificationsService.error("Shipping Country Delete Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name selectCatalogDialogConfirm
         * @function
         *
         * @description
         * Handles the catalog selection after recieving the dialogData from the dialog view/controller
         */
        function selectCatalogDialogConfirm(dialogData) {
            $scope.preValuesLoaded = false;
            $scope.selectedCatalog = _.find($scope.primaryWarehouse.warehouseCatalogs, function(catalog) {
                return catalog.key === dialogData.selectedWarehouseCatalog.key;
            });

            // Load the countries associated with this catalog.
            loadCountries();
        }

        /**
         * @ngdoc method
         * @name warehouseCatalogDialogConfirm
         * @function
         *
         * @description
         * Handles the add/edit after recieving the dialogData from the dialog view/controller.
         * If the selectedCatalog is set to be default, ensure that original default is no longer default.
         */
        function warehouseCatalogDialogConfirm(dialogData) {
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            var promiseUpdateCatalog;
            if (dialogData.warehouseCatalog.key === "") {
                promiseUpdateCatalog = warehouseResource.addWarehouseCatalog(dialogData.warehouseCatalog);
            } else {
                promiseUpdateCatalog = warehouseResource.putWarehouseCatalog(dialogData.warehouseCatalog);
            }
            promiseUpdateCatalog.then(function(response) {
                $scope.warehouses = [];
                init();
            }, function(reason) {
                notificationsService.error('Catalog Update Failed', reason.message);
            });
        }

        // initialize the controller
        init();


    }]);
