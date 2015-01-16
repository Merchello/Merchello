/**
 * @ngdoc service
 * @name Merchello.Backoffice.ShippingProvidersController
 *
 * @description
 * The controller for the shipment provider view
 */
angular.module('merchello').controller('Merchello.Backoffice.ShippingProvidersController',
    ['$scope', 'notificationsService', 'dialogService',
    'settingsResource', 'warehouseResource', 'shippingGatewayProviderResource', 'dialogDataFactory',
    'settingDisplayBuilder', 'warehouseDisplayBuilder', 'countryDisplayBuilder',
        'shippingGatewayProviderDisplayBuilder', 'shipCountryDisplayBuilder', 'shipMethodDisplayBuilder',
    function($scope, notificationsService, dialogService,
             settingsResource, warehouseResource, shippingGatewayProviderResource, dialogDataFactory,
             settingDisplayBuilder, warehouseDisplayBuilder, countryDisplayBuilder,
             shippingGatewayProviderDisplayBuilder, shipCountryDisplayBuilder, shipMethodDisplayBuilder) {

        $scope.loaded = true;
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
            changeSelectedCatalog();
        }

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
        function changeSelectedCatalog(catalogIndex) {
            if ((typeof catalogIndex) !== 'undefined') {
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
         * @name addEditWarehouseCatalogDialogOpen
         * @function
         *
         * @description
         * Opens the warehouse catalog dialog via the Umbraco dialogService.
         */
        function addEditWarehouseCatalogDialogOpen(warehouse, catalog) {

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

        }


        /**
         * @ngdoc method
         * @name deleteCatalog
         * @function
         *
         * @description
         * Opens the delete catalog dialog via the Umbraco dialogService.
         */
        function deleteCatalog() {
            var dialogData = {};
            dialogData.name = $scope.selectedCatalog.name;
            dialogData.catalog = $scope.selectedCatalog;
            dialogService.open({
                template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
                show: true,
                callback: deleteCatalogConfirm,
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
        function deleteCatalogConfirm(data) {
            var selectedCatalog = new merchello.Models.WarehouseCatalog(data.catalog);
            var promiseDeleteCatalog = shippingGatewayProviderResource.deleteWarehouseCatalog(selectedCatalog.key);
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
         * @name editRegionalShippingRatesDialogOpen
         * @function
         *
         * @description
         * Opens the edit regional shipping rates dialog via the Umbraco dialogService.

        function editRegionalShippingRatesDialogOpen(country, provider, method) {

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

        }
         */

        /**
         * @ngdoc method
         * @name selectCatalogDialogConfirm
         * @function
         *
         * @description
         * Handles the catalog selection after recieving the dialogData from the dialog view/controller
         */
        function selectCatalogDialogConfirm(data) {

            var index = data.filter.id;
            $scope.selectedCatalog = $scope.primaryWarehouse.warehouseCatalogs[index];
            $scope.countries = [];
            // Load the countries associated with this catalog.
            $scope.loadCountries();
        }

        /**
         * @ngdoc method
         * @name selectCatalogDialogOpen
         * @function
         *
         * @description
         * Opens the catalog selection dialog via the Umbraco dialogService.
         */
        function selectCatalogDialogOpen() {

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
        function warehouseCatalogDialogConfirm(data) {
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
        }

        // initialize the controller
        init();


    }]);
