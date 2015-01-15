/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */

(function() { 

    /**
     * @ngdoc controller
     * @name Merchello.Common.Dialogs.DeleteConfirmationController
     * @function
     *
     * @description
     * The controller for the delete confirmations
     */
    angular.module('merchello')
        .controller('Merchello.Common.Dialogs.DeleteConfirmationController',
        ['$scope', function($scope) {

        }]);

    /**
     * @ngdoc controller
     * @name Merchello.Common.Dialogs.EditAddressController
     * @function
     * 
     * @description
     * The controller for adding a country
     */
    angular.module('merchello')
        .controller('Merchello.Common.Dialogs.EditAddressController',
        function ($scope) {

            // public methods
            $scope.save = save;

            function init() {
                $scope.address = $scope.dialogData.address;
            };

            function save() {
                $scope.dialogData.address.countryCode = $scope.dialogData.selectedCountry.countryCode;
                if($scope.dialogData.selectedCountry.provinces.length > 0) {
                    $scope.dialogData.address.region = $scope.dialogData.selectedProvince.code;
                }
                $scope.submit($scope.dialogData);
            };

        init();
    });


/**
 * @ngdoc controller
 * @name Merchello.Backoffice.SettingsController
 * @function
 *
 * @description
 * The controller for the settings management page
 */
angular.module('merchello').controller('Merchello.Backoffice.SettingsController',
    ['$scope', '$log', 'serverValidationManager', 'notificationsService', 'settingsResource', 'settingDisplayBuilder',
        'currencyDisplayBuilder', 'countryDisplayBuilder',
        function($scope, $log, serverValidationManager, notificationsService, settingsResource, settingDisplayBuilder, currencyDisplayBuilder) {

            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.savingStoreSettings = false;
            $scope.settingsDisplay = settingDisplayBuilder.createDefault();
            $scope.currencies = [];
            $scope.selectedCurrency = {};

            // exposed methods
            $scope.currencyChanged = currencyChanged;
            $scope.save = save;

            function loadCurrency() {
                var promise = settingsResource.getAllCurrencies();
                promise.then(function(currenices) {
                    $scope.currencies = _.sortBy(currencyDisplayBuilder.transform(currenices), function(currency) {
                        return currency.name;
                    });
                    $scope.selectedCurrency = _.find($scope.currencies, function(currency) {
                      return currency.currencyCode === $scope.settingsDisplay.currencyCode;
                    });

                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    alert('Failed: ' + reason.message);
                });
            }

            function loadSettings() {
                var promise = settingsResource.getCurrentSettings();
                promise.then(function (settings) {
                    $scope.settingsDisplay = settingDisplayBuilder.transform(settings);
                    loadCurrency();
                }, function (reason) {
                    alert('Failed: ' + reason.message);
                });
            }

            function save () {
                $scope.preValuesLoaded = false;

                notificationsService.info("Saving...", "");
                $scope.savingStoreSettings = true;
                $scope.$watch($scope.storeSettingsForm, function(value) {
                    var promise = settingsResource.save($scope.settingsDisplay);
                    promise.then(function(settingDisplay) {
                        notificationsService.success("Store Settings Saved", "");
                        $scope.savingStoreSettings = false;
                        $scope.settingDisplay = settingDisplayBuilder.transform(settingDisplay);
                        loadSettings();
                    }, function(reason) {
                        notificationsService.error("Store Settings Save Failed", reason.message);
                    });
                });

            }

            function currencyChanged(currency) {
                $scope.settingsDisplay.currencyCode = currency.currencyCode;
            }

            loadSettings();
}]);

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProvider.Dialogs.NotificationsProviderSettingsSmtpController
     * @function
     *
     * @description
     * The controller for configuring the SMTP provider
     */
    angular.module('merchello').controller('Merchello.GatewayProvider.Dialogs.NotificationsProviderSettingsSmtpController',
        ['$scope', function($scope) {

            $scope.notificationProviderSettings = {};

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {

                //$scope.dialogData.provider.extendedData

                // on initial load extendedData will be empty but we need to populate with key values
                //
                if ($scope.dialogData.provider.extendedData.items.length > 0) {
                    var extendedDataKey = 'merchSmtpProviderSettings';
                    var settingsString = $scope.dialogData.provider.extendedData.getValue(extendedDataKey);
                    $scope.notificationProviderSettings = angular.fromJson(settingsString);
                    console.info($scope.dialogData);
                    console.info($scope.notificationProviderSettings);

                    // Watch with object equality to convert back to a string for the submit() call on the Save button
                    $scope.$watch(function () {
                        return $scope.notificationProviderSettings;
                    }, function (newValue, oldValue) {
                        $scope.dialogData.provider.extendedData.setValue(extendedDataKey, angular.toJson(newValue));
                    }, true);
                }
            }

            // Initialize
            init();

        }]);

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProvider.Dialogs.ShippingAddCountryController
     * @function
     *
     * @description
     * The controller for associating countries with shipping providers and warehouse catalogs
     */
    angular.module('merchello').controller('Merchello.GatewayProvider.Dialogs.ShippingAddCountryController',
        ['$scope', function($scope) {

        }]);

/**
 * @ngdoc controller
 * @name Merchello.Backoffice.GatewayProvidersListController
 * @function
 *
 * @description
 * The controller for the gateway providers list view controller
 */
angular.module("umbraco").controller("Merchello.Backoffice.GatewayProvidersListController",
    ['$scope', 'assetsService', 'notificationsService', 'dialogService',
        'gatewayProviderResource', 'gatewayProviderDisplayBuilder',
        function($scope, assetsService, notificationsService, dialogService, gatewayProviderResource, gatewayProviderDisplayBuilder)
        {
            // load the css file
            assetsService.loadCss('/App_Plugins/Merchello/assets/css/merchello.css');

            $scope.loaded = true;
            $scope.notificationGatewayProviders = [];
            $scope.paymentGatewayProviders = [];
            $scope.shippingGatewayProviders = [];
            $scope.taxationGatewayProviders = [];

            // exposed methods
            $scope.activateProvider = activateProvider;
            $scope.deactivateProvider = deactivateProvider;
            $scope.editProviderConfigDialogOpen = editProviderConfigDialogOpen;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                loadAllNotificationGatwayProviders();
                loadAllPaymentGatewayProviders();
                loadAllShippingGatewayProviders();
                loadAllTaxationGatewayProviders();
            }

            /**
             * @ngdoc method
             * @name loadAllNotificationGatwayProviders
             * @function
             *
             * @description
             * Loads in notification gateway providers from server into the scope.  Called in init().
             */
            function loadAllNotificationGatwayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedNotificationGatewayProviders();
                promiseAllProviders.then(function(allProviders) {
                    $scope.notificationGatewayProviders = gatewayProviderDisplayBuilder.transform(allProviders);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function(reason) {
                    notificationsService.error("Available Notification Gateway Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllPaymentGatewayProviders
             * @function
             *
             * @description
             * Loads in payment gateway providers from server into the scope.  Called in init().
             */
            function loadAllPaymentGatewayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedPaymentGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.paymentGatewayProviders = gatewayProviderDisplayBuilder.transform(allProviders);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Payment Gateway Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllShippingGatewayProviders
             * @function
             *
             * @description
             * Loads in shipping gateway providers from server into the scope.  Called in init().
             */
            function loadAllShippingGatewayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedShippingGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.shippingGatewayProviders = gatewayProviderDisplayBuilder.transform(allProviders);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Shipping Gateway Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllTaxationGatewayProviders
             * @function
             *
             * @description
             * Loads in taxation gateway providers from server into the scope.  Called in init().
             */
            function loadAllTaxationGatewayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedTaxationGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.taxationGatewayProviders = gatewayProviderDisplayBuilder.transform(allProviders);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Taxation Gateway Providers Load Failed", reason.message);
                });
            }

            /* -------------------------------------------------------------------
                Events
            ----------------------------------------------------------------------- */

            /**
             * @ngdoc method
             * @name activateProvider
             * @param {GatewayProvider} provider The GatewayProvider to activate
             * @function
             *
             * @description
             * Calls the merchelloGatewayProviderService to activate the provider.
             */
            function activateProvider(provider) {
                var promiseActivate = gatewayProviderResource.activateGatewayProvider(provider);
                promiseActivate.then(function () {
                    provider.activated = true;
                    init();
                    notificationsService.success("Payment Method Activated");
                }, function (reason) {
                    notificationsService.error("Payment Method Activate Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deactivateProvider
             * @param {GatewayProvider} provider The GatewayProvider to deactivate
             * @function
             *
             * @description
             * Calls the merchelloGatewayProviderService to deactivate the provider.
             */
            function deactivateProvider(provider) {
                var promiseDeactivate = gatewayProviderResource.deactivateGatewayProvider(provider);
                promiseDeactivate.then(function () {
                    provider.activated = false;
                    notificationsService.success("Payment Method Deactivated");
                }, function (reason) {
                    notificationsService.error("Payment Method Deactivate Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name editProviderConfigDialogOpen
             * @param {GatewayProvider} provider The GatewayProvider to configure
             * @function
             *
             * @description
             * Opens the dialog to allow user to add provider configurations
             */
            function editProviderConfigDialogOpen(provider) {
                var dialogProvider = provider;
                if (!provider) {
                    return;
                }
                var myDialogData = {
                    provider: dialogProvider
                };
                dialogService.open({
                    template: provider.dialogEditorView.editorView,
                    show: true,
                    callback: providerConfigDialogConfirm,
                    dialogData: myDialogData
                });
            }

            /**
             * @ngdoc method
             * @name providerConfigDialogConfirm
             * @param {dialogData} model returned from the dialog view
             * @function
             *
             * @description
             * Handles the data passed back from the provider editor dialog and saves it to the database
             */
            function providerConfigDialogConfirm(data) {
                var promise = gatewayProviderResource.saveGatewayProvider(data.provider);
                promise.then(function (provider) {
                        notificationsService.success("Gateway Provider Saved", "");
                    },
                    function (reason) {
                        notificationsService.error("Gateway Provider Save Failed", reason.message);
                    }
                );
            }

            // Initialize the controller

            init();

        }]);







angular.module('merchello').controller('Merchello.Backoffice.ShippingProvidersController',
    ['$scope', 'notificationsService', 'dialogService',
    'settingsResource', 'warehouseResource', 'shippingGatewayProviderResource',
    'settingDisplayBuilder', 'warehouseDisplayBuilder', 'countryDisplayBuilder', 'shippingGatewayProviderDisplayBuilder',
    'gatewayResourceDisplayBuilder', 'shipCountryDisplayBuilder',
    function($scope, notificationsService, dialogService,
             settingsResource, warehouseResource, shippingGatewayProviderResource,
             settingDisplayBuilder, warehouseDisplayBuilder, countryDisplayBuilder, shippingGatewayProviderDisplayBuilder,
             gatewayResourceDisplayBuilder, shipCountryDisplayBuilder) {

        $scope.loaded = true;
        $scope.countries = [];
        $scope.warehouses = [];
        $scope.primaryWarehouse = {};
        $scope.selectedCatalog = {};
        $scope.primaryWarehouseAddress = {};
        $scope.visible = {
            catalogPanel: true,
            shippingMethodPanel: true,
            warehouseInfoPanel: false,
            warehouseListPanel: true
        };

        // exposed methods
        $scope.addCountry = addCountry;

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
                    $scope.countries = _.sortBy(shipCountryDisplayBuilder.transform(shipCountries), function(country) {
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
                    loadAllAvailableCountries();
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Shipping Countries Load Failed", reason.message);
                });
            }
        }

        /**
         * @ngdoc method
         * @name loadAllAvailableCountries
         * @function
         *
         * @description
         * Load the countries from the settings service, then wrap the results
         * in Merchello models and add to the scope via the availableCountries collection.
         */
        function loadAllAvailableCountries() {
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
                $scope.availableCountries = _.sortBy(
                    _.filter(countries, function(country) {
                        var found = _.find($scope.countries, function(assigned) {
                            return country.countryCode === assigned.countryCode;
                        });
                        return found === undefined || found === null;
                    }), function(country) {
                        return country.name;
                    });
                console.info($scope.availableCountries);

            }, function (reason) {
                notificationsService.error("Available Countries Load Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name loadAllAvailableGatewayResources
         * @function
         *
         * @description
         * Load the shipping gateway resources from the shipping gateway service, then wrap the results
         * in Merchello models and add to the scope via the providers collection in the resources collection.

        function loadAllAvailableGatewayResources(shipProvider) {
            var promiseAllResources = shippingGatewayProviderResource.getAllShipGatewayResourcesForProvider(shipProvider);
            promiseAllResources.then(function (allResources) {
                shipProvider.resources = gatewayResourceDisplayBuilder.transform(allResources);
            }, function (reason) {
                notificationsService.error("Available Gateway Resources Load Failed", reason.message);
            });
        };
         */

        /**
         * @ngdoc method
         * @name loadAllShipProviders
         * @function
         *
         * @description
         * Load the shipping gateway providers from the shipping gateway service, then wrap the results
         * in Merchello models and add to the scope via the providers collection.

        function loadAllShipProviders() {
            var promiseAllProviders = shippingGatewayProviderResource.getAllShipGatewayProviders();
            promiseAllProviders.then(function (allProviders) {
                $scope.providers = shippingGatewayProviderDisplayBuilder.transform(allProviders);
                loadCountries();
            }, function (reason) {
                notificationsService.error("Available Ship Providers Load Failed", reason.message);
            });
        }
         */


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
        function deleteCountryDialogConfirm(country) {

            var promiseDelete = merchelloCatalogShippingService.deleteShipCountry(country.key);
            promiseDelete.then(function () {

                notificationsService.success("Shipping Country Deleted");

                $scope.loadCountries();

            }, function (reason) {

                notificationsService.error("Shipping Country Delete Failed", reason.message);

            });
        }

        /**
         * @ngdoc method
         * @name removeMethodFromProviderDialogConfirmation
         * @function
         *
         * @description
         * Calls the fixed rate shipping service to delete the method passed in via the method parameter.
         * After method is deleted, reload the list of methods for that provider in that country.

        function removeMethodFromProviderDialogConfirmation(data) {
            var promiseDelete = merchelloCatalogShippingService.deleteShipMethod(data.method);
            promiseDelete.then(function () {
                data.provider.shipMethods = [];
                $scope.loadProviderMethods(data.provider, data.country);
                notificationsService.success("Shipping Method Deleted");
            }, function (reason) {
                notificationsService.error("Shipping Method Delete Failed", reason.message);
            });
        }
         */

        /**
         * @ngdoc method
         * @name removeMethodFromProviderDialog
         * @function
         *
         * @description
         * Opens the delete confirmation dialog via the Umbraco dialogService.
         * Country and provider passed through dialogService so that on confirm the provider's
         * methods can be reloaded after the method is deleted.

        function removeMethodFromProviderDialog(country, provider, method) {
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
         */

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
            var dialogData = {};
            dialogData.availableCountries = $scope.availableCountries;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.addcountry.html',
                show: true,
                callback: addCountryDialogConfirm,
                dialogData: dialogData
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
        }

        /**
         * @ngdoc method
         * @name addEditShippingMethodDialogOpen
         * @function
         *
         * @description
         * Opens the add/edit shipping method dialog via the Umbraco dialogService.
         */
        function addEditShippingMethodDialogOpen(country, gatewayProvider, method) {

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

            // Get the editor template for the method's dialog.
            var templatePage = dialogMethod.dialogEditorView.editorView;

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
        }

        /**
         * @ngdoc method
         * @name addEditShippingProviderDialogOpen
         * @function
         *
         * @description
         * Opens the shipping provider dialog via the Umbraco dialogService.
         */
        function addEditShippingProviderDialogOpen(country, provider) {
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
                callback: $scope.deleteCatalogConfirm,
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
        function deleteCountryDialog(country) {
            var dialogData = {};
            dialogData = country;
            dialogService.open({
                template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
                show: true,
                callback: $scope.deleteCountryDialogConfirm,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name deleteWarehouse
         * @function
         *
         * @description
         * Opens the delete warehouse dialog via the Umbraco dialogService.
         */
        function deleteWarehouse(warehouse) {
            if (warehouse != undefined) {
                dialogService.open({
                    template: '/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shipping.deletewarehouse.html',
                    show: true,
                    callback: $scope.deleteWarehouseDialogConfirm,
                    dialogData: warehouse
                });
            }
        }

        /**
         * @ngdoc method
         * @name deleteWarehouseDialogConfirm
         * @function
         *
         * @description
         * Handles the delete after recieving the warehouse to delete from the dialog view/controller
         */
        function deleteWarehouseDialogConfirm(warehouse) {

            // Todo: call API method to delete warehouse and then reload warehouses from API

        }

        /**
         * @ngdoc method
         * @name editRegionalShippingRatesDialogOpen
         * @function
         *
         * @description
         * Opens the edit regional shipping rates dialog via the Umbraco dialogService.
         */
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
         * @name shippingMethodDialogConfirm
         * @function
         *
         * @description
         * Handles the edit after recieving the dialogData from the dialog view/controller
         */
        function shippingMethodDialogConfirm(data) {

            var promiseShipMethodSave = merchelloCatalogShippingService.saveShipMethod(data.method);
            promiseShipMethodSave.then(function() {
            }, function (reason) {
                notificationsService.error("Shipping Method Save Failed", reason.message);
            });
            data.provider.shipMethods = [];
            $scope.loadProviderMethods(data.provider, data.country);
        }

        /**
         * @ngdoc method
         * @name shippingProviderDialogConfirm
         * @function
         *
         * @description
         * Handles the edit after recieving the dialogData from the dialog view/controller
         */
        function shippingProviderDialogConfirm(data) {
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

    'use strict';
    /**
     * @ngdoc controller
     * @name Merchello.Sales.Dialog.CapturePaymentController
     * @function
     *
     * @description
     * The controller for the dialog used in capturing payments on the sales overview page
     */
    angular.module('merchello')
        .controller('Merchello.Sales.Dialogs.CapturePaymentController',
        ['$scope', function($scope) {

            function round(num, places) {
                return +(Math.round(num + "e+" + places) + "e-" + places);
            }

            $scope.dialogData.amount = round($scope.dialogData.invoiceBalance, 2)

    }]);
'use strict';
/**
 * @ngdoc controller
 * @name Merchello.Sales.Dialog.CreateShipmentController
 * @function
 *
 * @description
 * The controller for the dialog used in creating a shipment
 */
angular.module('merchello')
    .controller('Merchello.Sales.Dialogs.CreateShipmentController',
    ['$scope', 'ShipmentRequestDisplay', 'OrderDisplay', function($scope, ShipmentRequestDisplay, OrderDisplay) {

        $scope.save = save;
        $scope.loaded = false;

        function init() {
            _.each($scope.dialogData.order.items, function(item) {
                item.selected = true;
            });
            $scope.dialogData.shipmentRequest = new ShipmentRequestDisplay();
            $scope.dialogData.shipmentRequest.order = angular.extend($scope.dialogData.order, OrderDisplay);
            $scope.loaded = true;
        }

        function save() {
            $scope.dialogData.shipmentRequest.shipmentStatusKey = $scope.dialogData.shipmentStatus.key;
            $scope.dialogData.shipmentRequest.trackingNumber = $scope.dialogData.trackingNumber;
            $scope.dialogData.shipmentRequest.order.items = _.filter($scope.dialogData.order.items, function (item) {
                return item.selected === true;
            });
            $scope.submit($scope.dialogData);
        }

        init();

    }]);

'use strict';
/**
 * @ngdoc controller
 * @name Merchello.Sales.Dialog.CreateShipmentController
 * @function
 *
 * @description
 * The controller for the dialog used in creating a shipment
 */
angular.module('merchello')
    .controller('Merchello.Sales.Dialogs.EditShipmentController',
    ['$scope', 'ShipmentRequestDisplay', 'OrderDisplay', function($scope, ShipmentRequestDisplay, OrderDisplay) {

        $scope.save = save;
        $scope.loaded = false;

        $scope.checkboxDisabled = checkboxDisabled;

        function init() {
            _.each($scope.dialogData.shipment.items, function(item) {
                item.selected = true;
            });
            $scope.loaded = true;
        }

        function checkboxDisabled() {
            return $scope.dialogData.shipment.shipmentStatus.name === 'Shipped' || $scope.dialogData.shipment.shipmentStatus.name === 'Delivered'
        }

        function save() {
            $scope.dialogData.shipment.items = _.filter($scope.dialogData.shipment.items, function (item) {
                return item.selected === true;
            });
            $scope.submit($scope.dialogData);
        }

        init();

    }]);


/**
 * @ngdoc controller
 * @name Merchello.Dashboards.InvoicePaymentsController
 * @function
 *
 * @description
 * The controller for the invoice payments view
 */
angular.module('merchello').controller('Merchello.Backoffice.InvoicePaymentsController',
    ['$scope', '$log', '$routeParams',
        'invoiceResource', 'paymentResource', 'settingsResource',
        'invoiceDisplayBuilder', 'paymentDisplayBuilder',
        function($scope, $log, $routeParams, invoiceResource, paymentResource, settingsResource,
        invoiceDisplayBuilder, paymentDisplayBuilder) {

            $scope.loaded = false;
            $scope.invoice = {};
            $scope.payments = [];
            $scope.settings = {};
            $scope.currencySymbol = '';
            $scope.remainingBalance = 0;

        function init() {
            var paymentKey = $routeParams.id;
            loadInvoice(paymentKey);
        }

        /**
         * @ngdoc method
         * @name loadInvoice
         * @function
         *
         * @description - Load an invoice with the associated id.
         */
        function loadInvoice(id) {
            var promise = invoiceResource.getByKey(id);
            promise.then(function (invoice) {
                $scope.invoice = invoiceDisplayBuilder.transform(invoice);
                $scope.billingAddress = $scope.invoice.getBillToAddress();
                loadPayments(id);
                loadSettings();
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                //console.info($scope.invoice);
            }, function (reason) {
                notificationsService.error("Invoice Load Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         *
         * @description - Load the Merchello settings.
         */
        function loadSettings() {

            var settingsPromise = settingsResource.getAllSettings();
            settingsPromise.then(function(settings) {
                $scope.settings = settings;
            }, function(reason) {
                notificationsService.error('Failed to load global settings', reason.message);
            })

            var currencySymbolPromise = settingsResource.getCurrencySymbol();
            currencySymbolPromise.then(function (currencySymbol) {
                $scope.currencySymbol = currencySymbol;
            }, function (reason) {
                alert('Failed: ' + reason.message);
            });
        };

        function loadPayments(key)
        {
            var paymentsPromise = paymentResource.getPaymentsByInvoice(key);
            paymentsPromise.then(function(payments) {
                $scope.payments = paymentDisplayBuilder.transform(payments);
                $scope.remainingBalance = $scope.invoice.remainingBalance($scope.payments);
            }, function(reason) {
                notificationsService.error('Failed to load payments for invoice', reason.message);
            });
        }

        init();
}]);

/**
 * @ngdoc controller
 * @name Merchello.Dashboards.OrderShipmentsController
 * @function
 *
 * @description
 * The controller for the order shipments view
 */
angular.module('merchello').controller('Merchello.Backoffice.OrderShipmentsController',
    ['$scope', '$routeParams', '$log', 'notificationsService', 'dialogService', 'dialogDataFactory',
        'invoiceResource', 'settingsResource', 'shipmentResource',
        'invoiceDisplayBuilder', 'shipmentDisplayBuilder',
        function($scope, $routeParams, $log, notificationsService, dialogService, dialogDataFactory, invoiceResource,
                 settingsResource, shipmentResource, invoiceDisplayBuilder, shipmentDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.invoice = {};
            $scope.settings = {};
            $scope.shipments = [];

            // methods
            $scope.isEditableAddress = isEditableAddress;
            $scope.updateShippingAddressLineItem = updateShippingAddressLineItem;

            // dialogs
            $scope.openShipmentDialog = openShipmentDialog;
            $scope.openAddressDialog = openAddressDialog;
            $scope.openDeleteShipmentDialog = openDeleteShipmentDialog;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description - Controller initialization.
             */
            function init() {
                var key = $routeParams.id;
                loadInvoice(key);
                $scope.loaded = true;
            }

            /**
             * @ngdoc method
             * @name loadInvoice
             * @function
             *
             * @description - responsible for loading the invoice.
             */
            function loadInvoice(key) {
                var invoicePromise = invoiceResource.getByKey(key);
                invoicePromise.then(function(invoice) {
                    $scope.invoice = invoice;
                    loadSettings();
                    var shipmentsPromise = shipmentResource.getShipmentsByInvoice(invoice);
                    shipmentsPromise.then(function(shipments) {
                        $scope.shipments = shipmentDisplayBuilder.transform(shipments);
                        $scope.preValuesLoaded = true;
                    })
                }, function(reason) {
                    notificationsService.error('Failed to load invoice', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {
                var settingsPromise = settingsResource.getAllSettings();
                settingsPromise.then(function (settings) {
                    $scope.settings = settings;
                }, function (reason) {
                    notificationsService.error('Failed to load global settings', reason.message);
                })
            }

            /**
             * @ngdoc method
             * @name isEditableStatus
             * @function
             *
             * @description - Returns a value indicating whether or not the shipment address can be edited.
             */
            function isEditableAddress(shipmentStatus) {
                if (shipmentStatus.name === 'Delivered' || shipmentStatus.name === 'Shipped') {
                    return false;
                }
                return true;
            }

            /*--------------------------------------------------------------------------------
                Dialogs
            ----------------------------------------------------------------------------------*/

            function updateShippingAddressLineItem(shipment) {
                var promise = shipmentResource.updateShippingAddressLineItem(shipment);
                promise.then(function() {
                    loadInvoice($scope.invoice.key);
                    notificationsService.success('Successfully updated sales shipping address.')
                }, function(reason) {
                    notificationsService.error('Failed to update shipping addresses on invoice', reason.message);
                })
            }


            function openDeleteShipmentDialog(shipment) {
                var dialogData = {};
                dialogData.name = 'Shipment #' + shipment.shipmentNumber;
                dialogData.shipment = shipment;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteShipmentDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name openShipmentDialog
             * @function
             *
             * @description - responsible for opening the edit shipment dialog and passing the selected shipment.
             */
            function openShipmentDialog(shipment) {
                var promiseStatuses = shipmentResource.getAllShipmentStatuses();
                promiseStatuses.then(function(statuses) {
                    var dialogData = dialogDataFactory.createEditShipmentDialogData();
                    dialogData.shipment = shipment;
                    dialogData.shipmentStatuses = statuses;
                    dialogData.shipment.shipmentStatus = _.find(statuses, function(status) {
                      return status.key === dialogData.shipment.shipmentStatus.key;
                    });

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.edit.shipment.html',
                        show: true,
                        callback: processUpdateShipment,
                        dialogData: dialogData
                    });
                });
            }

            /**
             * @ngdoc method
             * @name openAddressDialog
             * @function
             *
             * @description - responsible for opening the edit address dialog with the appropriate address to be edited
             */
            function openAddressDialog(shipment, addressType) {
                var dialogData = dialogDataFactory.createEditAddressDialogData();
                if (addressType === 'destination') {
                    dialogData.address = shipment.getDestinationAddress();
                    dialogData.showPhone = true;
                    dialogData.showEmail = true;
                    dialogData.showIsCommercial = true;
                }
                else
                {
                    dialogData.address = shipment.getOriginAddress();
                }

                // add the shipment -- this modifies the EditAddressDialogData model with an extra property
                dialogData.shipment = shipment;

                // get the list of countries to populate the countries drop down
                var countryPromise = settingsResource.getAllCountries();
                countryPromise.then(function(countries) {
                    dialogData.countries = countries;

                    dialogData.selectedCountry = _.find(countries, function(country) {
                        return country.countryCode === dialogData.address.countryCode;
                    });

                    // if this address has a region ... we need to get that too.
                    if(dialogData.address.region !== '' && dialogData.address.region !== null && dialogData.selectedCountry.provinces.length > 0) {
                        dialogData.selectedProvince = _.find(dialogData.selectedCountry.provinces, function(region) {
                            return region.code === dialogData.address.region;
                        });
                    }

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/edit.address.html',
                        show: true,
                        callback: addressType === 'destination' ? processUpdateDestinationAddress : processUpdateOriginAddress,
                        dialogData: dialogData
                    });
                });
            }

            /**
             * @ngdoc method
             * @name processUpdateOriginAddres
             * @function
             *
             * @description - updates the origin address on the shipment.
             */
            function processUpdateOriginAddress(dialogData) {
                $scope.preValuesLoaded = false;
                var shipment = dialogData.shipment;
                shipment.setOriginAddress(dialogData.address);
                saveShipment(shipment);
            }

            /**
             * @ngdoc method
             * @name processUpdateDestinationAddress
             * @function
             *
             * @description - updates the destination address of a shipment.
             */
            function processUpdateDestinationAddress(dialogData) {
                $scope.preValuesLoaded = false;
                var shipment = dialogData.shipment;
                shipment.setDestinationAddress(dialogData.address);
                saveShipment(shipment);
            }

            /**
             * @ngdoc method
             * @name processUpdateShipment
             * @function
             *
             * @description - responsible for handling dialog data for updating a shipment.
             */
            function processUpdateShipment(dialogData) {
                $scope.preValuesLoaded = false;
                saveShipment(dialogData.shipment);
            }

            /**
             * @ngdoc method
             * @name processDeleteShipmentDialog
             * @function
             *
             * @description - responsible for deleting a shipment.
             */
            function processDeleteShipmentDialog(dialogData) {
                var promise = shipmentResource.deleteShipment(dialogData.shipment);
                promise.then(function() {
                    loadInvoice($scope.invoice.key);
                }, function(reason) {
                  notificationsService.error('Failed to delete the invoice.', reason.message)
                });
            }

            /**
             * @ngdoc method
             * @name saveShipment
             * @function
             *
             * @description - responsible for saving a shipment.
             */
            function saveShipment(shipment) {

                var promise = shipmentResource.saveShipment(shipment);
                promise.then(function(shipment) {
                    loadInvoice($scope.invoice.key);
                });
            }


            // initializes the controller
            init();
    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Sales.OverviewController
     * @function
     *
     * @description
     * The controller for the sales overview view
     */
    angular.module('merchello').controller('Merchello.Backoffice.SalesOverviewController',
        ['$scope', '$routeParams', '$timeout', '$log', 'assetsService', 'dialogService', 'localizationService', 'notificationsService',
            'auditLogResource', 'invoiceResource', 'settingsResource', 'paymentResource', 'shipmentResource',
            'orderResource', 'dialogDataFactory', 'addressDisplayBuilder', 'salesHistoryDisplayBuilder',
            'invoiceDisplayBuilder', 'paymentDisplayBuilder', 'shipMethodsQueryDisplayBuilder',
        function($scope, $routeParams, $timeout, $log, assetsService, dialogService, localizationService, notificationsService,
                 auditLogResource, invoiceResource, settingsResource, paymentResource, shipmentResource, orderResource, dialogDataFactory,
                 addressDisplayBuilder, salesHistoryDisplayBuilder, invoiceDisplayBuilder, paymentDisplayBuilder, shipMethodsQueryDisplayBuilder) {

            // exposed properties
            $scope.historyLoaded = false;
            $scope.invoice = {};
            $scope.remainingBalance = 0.0;
            $scope.shippingTotal = 0.0;
            $scope.taxTotal = 0.0;
            $scope.currencySymbol = '';
            $scope.settings = {};
            $scope.salesHistory = {};
            $scope.payments = [];
            $scope.billingAddress = {};
            $scope.hasShippingAddress = false;
            $scope.authorizedCapturedLabel = '';
            $scope.shipmentLineItems = [];

            // exposed methods
            //  dialogs
            $scope.capturePayment = capturePayment;
            $scope.capturePaymentDialogConfirm = capturePaymentDialogConfirm,
            $scope.openDeleteInvoiceDialog = openDeleteInvoiceDialog;
            $scope.processDeleteInvoiceDialog = processDeleteInvoiceDialog,
            $scope.openFulfillShipmentDialog = openFulfillShipmentDialog;
            $scope.processFulfillShipmentDialog = processFulfillShipmentDialog;

            // localize the sales history message
            $scope.localizeMessage = localizeMessage;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description - Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init () {
                loadInvoice($routeParams.id);
                loadSettings();
                $scope.loaded = true;
            };

            function localizeMessage(msg) {
                return msg.localize(localizationService);
            }

            /**
             * @ngdoc method
             * @name loadAuditLog
             * @function
             *
             * @description
             * Load the Audit Log for the invoice via API.
             */
            function loadAuditLog(key) {
                if (key !== undefined) {
                    var promise = auditLogResource.getSalesHistoryByInvoiceKey(key);
                    promise.then(function (response) {
                        var history = salesHistoryDisplayBuilder.transform(response);
                        // TODO this is a patch for a problem in the API
                        if (history.dailyLogs.length) {
                            $scope.salesHistory = history.dailyLogs;
                            angular.forEach(history.dailyLogs, function(daily) {
                              angular.forEach(daily.logs, function(log) {
                                 localizationService.localize(log.message.localizationKey(), log.message.localizationTokens()).then(function(value) {
                                    log.message.formattedMessage = value;
                                 });
                              });
                            });
                        }
                        $scope.historyLoaded = history.dailyLogs.length > 0;
                    }, function (reason) {
                        notificationsService.error('Failed to load sales history', reason.message);
                    });
                }
            };

            /**
             * @ngdoc method
             * @name loadInvoice
             * @function
             *
             * @description - Load an invoice with the associated id.
             */
            function loadInvoice(id) {
                var promise = invoiceResource.getByKey(id);
                promise.then(function (invoice) {
                    $scope.invoice = invoiceDisplayBuilder.transform(invoice);
                    $scope.billingAddress = $scope.invoice.getBillToAddress();
                    $scope.taxTotal = $scope.invoice.getTaxLineItem().price;
                    $scope.shippingTotal = $scope.invoice.shippingTotal();
                    loadPayments(id);
                    loadAuditLog(id);
                    loadShippingAddress(id);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                    var shipmentLineItem = $scope.invoice.getShippingLineItems();
                    if (shipmentLineItem) {
                        $scope.shipmentLineItems.push(shipmentLineItem);
                    }
                   //console.info($scope.invoice);
                }, function (reason) {
                    notificationsService.error("Invoice Load Failed", reason.message);
                });
            };


           /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {

               var settingsPromise = settingsResource.getAllSettings();
               settingsPromise.then(function(settings) {
                   $scope.settings = settings;
               }, function(reason) {
                   notificationsService.error('Failed to load global settings', reason.message);
               })

                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                }, function (reason) {
                    alert('Failed: ' + reason.message);
                });
            };

            /**
             * @ngdoc method
             * @name loadPayments
             * @function
             *
             * @description - Load the Merchello payments for the invoice.
             */
            function loadPayments(key) {
                var paymentsPromise = paymentResource.getPaymentsByInvoice(key);
                paymentsPromise.then(function(payments) {
                    $scope.payments = paymentDisplayBuilder.transform(payments);
                    $scope.remainingBalance = $scope.invoice.remainingBalance($scope.payments);
                    $scope.authorizedCapturedLabel  = $scope.remainingBalance == '0' ? 'merchelloOrderView_captured' : 'merchelloOrderView_authorized';

                }, function(reason) {
                    notificationsService.error('Failed to load payments for invoice', reason.message);
                });
            }

            function loadShippingAddress(key) {
                var shippingAddressPromise = orderResource.getShippingAddress(key);
                shippingAddressPromise.then(function(result) {
                      $scope.shippingAddress = addressDisplayBuilder.transform(result);
                      $scope.hasShippingAddress = true;
                }, function(reason) {
                    notificationsService.error('Failed to load shipping address', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name capturePayment
             * @function
             *
             * @description - Open the capture shipment dialog.
             */
            function capturePayment() {
                var data = dialogDataFactory.createCapturePaymentDialogData();
                data.setPaymentData($scope.payments[0]);
                data.setInvoiceData($scope.payments, $scope.invoice, $scope.currencySymbol);
                if (!data.isValid()) {
                    return false;
                }
                // TODO inject the template for the capture payment dialog so that we can
                // have different fields for other providers
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/capture.payment.html',
                    show: true,
                    callback: $scope.capturePaymentDialogConfirm,
                    dialogData: data
                });
            };

            /**
             * @ngdoc method
             * @name capturePaymentDialogConfirm
             * @function
             *
             * @description - Capture the payment after the confirmation dialog was passed through.
             */
            function capturePaymentDialogConfirm(paymentRequest) {
                $scope.preValuesLoaded = false;
                var promiseSave = paymentResource.capturePayment(paymentRequest);
                promiseSave.then(function (payment) {
                    // added a timeout here to give the examine index
                    $timeout(function() {
                        notificationsService.success("Payment Captured");
                        loadInvoice(paymentRequest.invoiceKey);
                    }, 400);
                }, function (reason) {
                    notificationsService.error("Payment Capture Failed", reason.message);
                });
            };

            /**
             * @ngdoc method
             * @name openDeleteInvoiceDialog
             * @function
             *
             * @description - Open the delete payment dialog.
             */
            function openDeleteInvoiceDialog() {
                var dialogData = {};
                dialogData.name = 'Invoice #' + $scope.invoice.invoiceNumber;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteInvoiceDialog,
                    dialogData: dialogData
                });
            };

            /**
             * @ngdoc method
             * @name openFulfillShipmentDialog
             * @function
             *
             * @description - Open the fufill shipment dialog.
             */
            function openFulfillShipmentDialog() {
                var promiseStatuses = shipmentResource.getAllShipmentStatuses();
                promiseStatuses.then(function(statuses) {
                    var data = dialogDataFactory.createCreateShipmentDialogData();
                    data.order = $scope.invoice.orders[0]; // todo: pull from current order when multiple orders is available
                    data.order.items = data.order.getUnShippedItems();
                    data.shipmentStatuses = statuses;

                    // packaging
                    var quotedKey = '7342dcd6-8113-44b6-bfd0-4555b82f9503';
                    data.shipmentStatus = _.find(data.shipmentStatuses, function(status) {
                        return status.key === quotedKey;
                    });
                    data.invoiceKey = $scope.invoice.key;

                    // TODO this could eventually turn into an array
                    var shipmentLineItem = $scope.invoice.getShippingLineItems();
                    if ($scope.shipmentLineItems[0]) {
                        var shipMethodKey = $scope.shipmentLineItems[0].extendedData.getValue('merchShipMethodKey');
                        var shipMethodPromise = shipmentResource.getShipMethodAndAlternatives(shipMethodKey);
                        shipMethodPromise.then(function(result) {
                            data.shipMethods = shipMethodsQueryDisplayBuilder.transform(result);
                            dialogService.open({
                                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.create.shipment.html',
                                show: true,
                                callback: $scope.processFulfillShipmentDialog,
                                dialogData: data
                            });

                        });
                    }
                });
            };

            /**
             * @ngdoc method
             * @name processDeleteInvoiceDialog
             * @function
             *
             * @description - Delete the invoice.
             */
            function processDeleteInvoiceDialog() {
                var promiseDeleteInvoice = invoiceResource.deleteInvoice($scope.invoice.key);
                promiseDeleteInvoice.then(function (response) {
                    notificationsService.success('Invoice Deleted');
                    window.location.href = '#/merchello/merchello/invoicelist/manage';
                }, function (reason) {
                    notificationsService.error('Failed to Delete Invoice', reason.message);
                });
            };

            /**
             * @ngdoc method
             * @name processFulfillPaymentDialog
             * @function
             *
             * @description - Process the fulfill shipment functionality on callback from the dialog service.
             */
            function processFulfillShipmentDialog(data) {
                $scope.preValuesLoaded = false;
                if(data.shipmentRequest.order.items.length > 0) {
                    var promiseNewShipment = shipmentResource.newShipment(data.shipmentRequest);
                    promiseNewShipment.then(function (shipment) {
                        $timeout(function() {
                            notificationsService.success('Shipment #' + shipment.shipmentNumber + ' created');
                            loadInvoice(data.invoiceKey);
                        }, 400);

                    }, function (reason) {
                        notificationsService.error("New Shipment Failed", reason.message);
                    });
                } else {
                    $scope.preValuesLoaded = true;
                    notificationsService.warning('Shipment would not contain any items', 'The shipment was not created as it would not contain any items.');
                }
            };

            // initialize the controller
            init();
    }]);

'use strict';
/**
 * @ngdoc controller
 * @name Merchello.Dashboards.Sales.ListController
 * @function
 *
 * @description
 * The controller for the orders list page
 */
angular.module('merchello').controller('Merchello.Backoffice.SalesListController',
    ['$scope', '$element', '$log', 'angularHelper', 'assetsService', 'notificationsService',
        'invoiceResource', 'queryDisplayBuilder', 'queryResultDisplayBuilder', 'invoiceDisplayBuilder',
        function($scope, $element, $log, angularHelper, assetsService, notificationService, invoiceResource,
                 queryDisplayBuilder, queryResultDisplayBuilder, invoiceDisplayBuilder)
        {

            // expose on scope
            $scope.loaded = true;
            $scope.currentPage = 0;
            $scope.filterText = '';
            $scope.filterStartDate = '';
            $scope.filterEndDate = '';
            $scope.invoices = [];
            $scope.limitAmount = '25';
            $scope.maxPages = 0;
            $scope.orderIssues = [];
            $scope.salesLoaded = true;
            $scope.selectAllOrders = false;
            $scope.selectedOrderCount = 0;
            $scope.settings = {};
            $scope.sortOrder = "desc";
            $scope.sortProperty = "-invoiceNumber";
            $scope.visible = {};
            $scope.visible.bulkActionDropdown = false;
            $scope.currentFilters = [];

            // for testing
            $scope.itemCount = 0;

            //--------------------------------------------------------------------------------------
            // Event Handlers
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name changePage
             * @function
             *
             * @description
             * Changes the current page.
             */
            $scope.changePage = function (page) {
                $scope.currentPage = page;
                var query = buildQuery($scope.filterText);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name changeSortOrder
             * @function
             *
             * @description
             * Helper function to set the current sort on the table and switch the
             * direction if the property is already the current sort column.
             */
            $scope.changeSortOrder = function (propertyToSort) {
                if ($scope.sortProperty == propertyToSort) {
                    if ($scope.sortOrder == "asc") {
                        $scope.sortProperty = "-" + propertyToSort;
                        $scope.sortOrder = "desc";
                    } else {
                        $scope.sortProperty = propertyToSort;
                        $scope.sortOrder = "asc";
                    }
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "asc";
                }
                var query = buildQuery($scope.filterText);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name limitChanged
             * @function
             *
             * @description
             * Helper function to set the amount of items to show per page for the paging filters and calculations
             */
            $scope.limitChanged = function (newVal) {
                $scope.limitAmount = newVal;
                $scope.currentPage = 0;
                var query = buildQuery($scope.filterText);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name filterWithWildcard
             * @function
             *
             * @description
             * Fired when the filter button next to the filter text box at the top of the page is clicked.
             */
            $scope.filterWithWildcard = function (filterText) {
                var query = buildQuery(filterText);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name filterWithDates
             * @function
             *
             * @description
             * Fired when the filter button next to the filter text box at the top of the page is clicked.
             */
            $scope.filterWithDates = function(filterStartDate, filterEndDate) {
                var query = buildQueryDates(filterStartDate, filterEndDate);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name resetFilters
             * @function
             *
             * @description
             * Fired when the reset filter button is clicked.
             */
            $scope.resetFilters = function () {
                var query = buildQuery();
                $scope.currentFilters = [];
                $scope.filterText = "";
                $scope.filterStartDate = "";
                $scope.filterEndDate = "";
                loadInvoices(query);
                $scope.filterAction = false;
            };



            //--------------------------------------------------------------------------------------
            // Helper Methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name setVariables
             * @function
             *
             * @description
             * Returns sort information based off the current $scope.sortProperty.
             */
            $scope.sortInfo = function() {
                var sortDirection, sortBy;
                // If the sortProperty starts with '-', it's representing a descending value.
                if ($scope.sortProperty.indexOf('-') > -1) {
                    // Get the text after the '-' for sortBy
                    sortBy = $scope.sortProperty.split('-')[1];
                    sortDirection = 'Descending';
                    // Otherwise it is ascending.
                } else {
                    sortBy = $scope.sortProperty;
                    sortDirection = 'Ascending';
                }
                return {
                    sortBy: sortBy.toLowerCase(), // We'll want the sortBy all lower case for API purposes.
                    sortDirection: sortDirection
                }
            };

            /**
             * @ngdoc method
             * @name numberOfPages
             * @function
             *
             * @description
             * Helper function to get the amount of items to show per page for the paging
             */
            $scope.numberOfPages = function () {
                return $scope.maxPages;
                //return Math.ceil($scope.products.length / $scope.limitAmount);
            };

            // PRIVATE
            function init() {
                $scope.currencySymbol = '$';
                loadInvoices(buildQuery());
                $scope.loaded = true;
            }

            function loadInvoices(query) {
                $scope.salesLoaded = false;
                $scope.salesLoaded = false;

                var promise = invoiceResource.searchInvoices(query);
                promise.then(function (response) {
                    var queryResult = queryResultDisplayBuilder.transform(response, invoiceDisplayBuilder);
                    $scope.invoices = queryResult.items;
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                    $scope.salesLoaded = true;
                    $scope.maxPages = queryResult.totalPages;
                    $scope.itemCount = queryResult.totalItems;
                }, function (reason) {
                    notificationsService.error("Failed To Load Invoices", reason.message);
                });

            }

            /**
             * @ngdoc method
             * @name buildQuery
             * @function
             *
             * @description
             * Perpares a new query object for passing to the ApiController
             */
            function buildQuery(filterText) {
                var page = $scope.currentPage;
                var perPage = $scope.limitAmount;
                var sortBy = $scope.sortInfo().sortBy;
                var sortDirection = $scope.sortInfo().sortDirection;
                if (filterText === undefined) {
                    filterText = '';
                }
                if (filterText !== $scope.filterText) {
                    page = 0;
                    $scope.currentPage = 0;
                }
                $scope.filterText = filterText;

                var query = queryDisplayBuilder.createDefault();
                query.currentPage = page;
                query.itemsPerPage = perPage;
                query.sortBy = sortBy;
                query.sortDirection = sortDirection;
                query.addFilterTermParam(filterText)

                if (query.parameters.length > 0) {
                    $scope.currentFilters = query.parameters;
                }
                return query;
            };

            /**
             * @ngdoc method
             * @name buildQueryDates
             * @function
             *
             * @description
             * Perpares a new query object for passing to the ApiController
             */
             function buildQueryDates(startDate, endDate) {
                var page = $scope.currentPage;
                var perPage = $scope.limitAmount;
                var sortBy = $scope.sortInfo().sortBy;
                var sortDirection = $scope.sortInfo().sortDirection;
                if (startDate === undefined && endDate === undefined) {
                    $scope.currentFilters = [];
                } else {
                    $scope.currentFilters = [{
                        fieldName: 'invoiceDateStart',
                        value: startDate
                    }, {
                        fieldName: 'invoiceDateEnd',
                        value: endDate
                    }];
                }
                if (startDate !== $scope.filterStartDate) {
                    page = 0;
                    $scope.currentPage = 0;
                }
                $scope.filterStartDate = startDate;
                var query = buildQuery();
                query.addInvoiceDateParam(startDate, 'start');
                query.addInvoiceDateParam(endDate, 'end');

                return query;
            };

            /**
             * @ngdoc method
             * @name setupDatePicker
             * @function
             *
             * @description
             * Sets up the datepickers
             */
            function setupDatePicker(pickerId) {

                // Open the datepicker and add a changeDate eventlistener
                $element.find(pickerId).datetimepicker({
                    pickTime: false
                });

                //Ensure to remove the event handler when this instance is destroyted
                $scope.$on('$destroy', function () {
                    $element.find(pickerId).datetimepicker("destroy");
                });
            };

            //// Initialize
            assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css').then(function () {
                var filesToLoad = [
                    'lib/moment/moment-with-locales.js',
                    'lib/datetimepicker/bootstrap-datetimepicker.js'];
                assetsService.load(filesToLoad).then(
                    function () {
                        //The Datepicker js and css files are available and all components are ready to use.

                        setupDatePicker("#filterStartDate");
                        $element.find("#filterStartDate").datetimepicker().on("changeDate", $scope.applyDateStart);

                        setupDatePicker("#filterEndDate");
                        $element.find("#filterEndDate").datetimepicker().on("changeDate", $scope.applyDateEnd);
                    });
            });

            init();
        }]);


})();