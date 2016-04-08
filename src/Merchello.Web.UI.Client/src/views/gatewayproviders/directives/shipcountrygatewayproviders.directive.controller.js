angular.module('merchello').controller('Merchello.Directives.ShipCountryGatewaysProviderDirectiveController',
    ['$scope', 'notificationsService', 'dialogService', 'settingsResource',
        'shippingGatewayProviderResource', 'shippingGatewayProviderDisplayBuilder', 'shipMethodDisplayBuilder',
        'shippingGatewayMethodDisplayBuilder', 'gatewayResourceDisplayBuilder', 'dialogDataFactory',
        function($scope, notificationsService, dialogService, settingsResource,
                 shippingGatewayProviderResource, shippingGatewayProviderDisplayBuilder, shipMethodDisplayBuilder,
                 shippingGatewayMethodDisplayBuilder, gatewayResourceDisplayBuilder, dialogDataFactory) {

            $scope.providersLoaded = false;
            $scope.allProviders = [];
            $scope.assignedProviders = [];
            $scope.availableProviders = [];
            $scope.currencySymbol = '';

            // exposed methods
            $scope.deleteCountry = deleteCountry;
            $scope.addShippingProviderDialogOpen = addShippingProviderDialogOpen;
            $scope.addAddShipMethodDialogOpen = addAddShipMethodDialogOpen;
            $scope.deleteShipMethodOpen = deleteShipMethodOpen;
            $scope.editShippingMethodDialogOpen = editShippingMethodDialogOpen;
            $scope.editShippingMethodRegionsOpen = editShippingMethodRegionsOpen;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Initializes the controller
             */
            function init() {
                loadSettings();
                loadCountryProviders();
            }

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Loads the currency settings
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function(currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

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
            function loadCountryProviders() {
                var promiseAllProviders = shippingGatewayProviderResource.getAllShipGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.allProviders = shippingGatewayProviderDisplayBuilder.transform(allProviders);

                    var promiseProviders = shippingGatewayProviderResource.getAllShipCountryProviders($scope.country);
                    promiseProviders.then(function (assigned) {
                        if (angular.isArray(assigned)) {
                            $scope.assignedProviders = shippingGatewayProviderDisplayBuilder.transform(assigned);

                            var available = _.filter($scope.allProviders, function(provider) {
                                var found = _.find($scope.assignedProviders, function(ap) {
                                    return ap.key === provider.key;
                                });
                                return found === undefined || found === null;
                            });
                            angular.forEach(available, function(pusher) {
                                $scope.availableProviders.push(pusher);
                            });

                            loadProviderMethods();
                        }
                    }, function (reason) {
                        notificationsService.error("Fixed Rate Shipping Countries Providers Load Failed", reason.message);
                    });
                }, function (reason) {
                    notificationsService.error("Available Ship Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadShipMethods
             * @function
             *
             * @description
             * Load the shipping methods from the shipping gateway service, then wrap the results
             * in Merchello models and add to the scope via the provider in the shipMethods collection.
             */
            function loadProviderMethods() {
                angular.forEach($scope.assignedProviders, function(shipProvider) {
                    var promiseShipMethods = shippingGatewayProviderResource.getShippingGatewayMethodsByCountry(shipProvider, $scope.country);
                    promiseShipMethods.then(function (shipMethods) {
                        var shippingGatewayMethods = shippingGatewayMethodDisplayBuilder.transform(shipMethods);
                        shipProvider.shippingGatewayMethods = _.sortBy(shippingGatewayMethods, function(gatewayMethod) {
                            return gatewayMethod.getName();
                        });
                    }, function (reason) {
                        notificationsService.error("Available Shipping Methods Load Failed", reason.message);
                    });
                });
                $scope.providersLoaded = true;
            }

            /**
             * @ngdoc method
             * @name addEditShippingProviderDialogOpen
             * @function
             *
             * @description
             * Opens the shipping provider dialog via the Umbraco dialogService.
             */
             function addShippingProviderDialogOpen() {
                var dialogData = dialogDataFactory.createAddShipCountryProviderDialogData();
                //dialogData.country = country;
                dialogData.availableProviders = $scope.availableProviders;
                dialogData.selectedProvider = dialogData.availableProviders[0];
                dialogData.selectedResource = dialogData.selectedProvider.availableResources[0];
                dialogData.shipMethodName = 'New ship method';
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.shipcountry.addprovider.html',
                    show: true,
                    callback: shippingProviderDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name addAddShipMethodDialogOpen
             * @function
             *
             * @description
             * Opens the shipping provider dialog via the Umbraco dialogService.
             */
            function addAddShipMethodDialogOpen(provider) {
                var dialogData = dialogDataFactory.createAddShipCountryProviderDialogData();
                dialogData.selectedProvider = provider;
                dialogData.selectedResource = dialogData.selectedProvider.availableResources[0];
                dialogData.shipMethodName = $scope.country.name + " " + dialogData.selectedResource.name;
                dialogData.country = $scope.country;
                dialogData.showProvidersDropDown = false;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.shipcountry.addprovider.html',
                    show: true,
                    callback: shippingProviderDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name editShippingMethodDialogOpen
             * @function
             *
             * @description
             * Opens an injected dialog for editing a the shipping provider's ship method
             */
            function editShippingMethodRegionsOpen(gatewayMethod) {
                var dialogData = dialogDataFactory.createEditShippingGatewayMethodDialogData();
                dialogData.shippingGatewayMethod = gatewayMethod;
                dialogData.currencySymbol = $scope.currencySymbol;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.shipmethod.regions.html',
                    show: true,
                    callback: shippingMethodDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name shippingProviderDialogConfirm
             * @function
             *
             * @description
             * Handles the edit after receiving the dialogData from the dialog view/controller
             */
            function shippingProviderDialogConfirm(dialogData) {
                var newShippingMethod = shipMethodDisplayBuilder.createDefault();
                if(dialogData.shipMethodName ==='') {
                    newShippingMethod.name = $scope.country.name + " " + dialogData.selectedResource.name;
                } else {
                    newShippingMethod.name = dialogData.shipMethodName;
                }
                newShippingMethod.providerKey = dialogData.selectedProvider.key;
                newShippingMethod.serviceCode = dialogData.selectedResource.serviceCode;
                newShippingMethod.shipCountryKey = $scope.country.key;
                var promiseAddMethod;
                promiseAddMethod = shippingGatewayProviderResource.addShipMethod(newShippingMethod);
                promiseAddMethod.then(function () {
                    reload();
                }, function (reason) {
                    notificationsService.error("Shipping Provider / Initial Method Create Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deleteShipMethodOpen
             * @function
             *
             * @description
             * Opens the delete confirmation dialog for deleting ship methods
             */
            function deleteShipMethodOpen(shipMethod) {
                var dialogData = dialogDataFactory.createDeleteShipCountryDialogData();
                dialogData.shipMethod = shipMethod;
                dialogData.name = shipMethod.name;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: deleteShipMethodDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name deleteShipMethodOpen
             * @function
             *
             * @description
             * Processes the deleting of a ship method
             */
            function deleteShipMethodDialogConfirm(dialogData) {
                var shipMethod = dialogData.shipMethod;
                var promise = shippingGatewayProviderResource.deleteShipMethod(shipMethod);
                promise.then(function() {
                    reload();
                });
            }

            // injected dialogs

            /**
             * @ngdoc method
             * @name editShippingMethodDialogOpen
             * @function
             *
             * @description
             * Opens an injected dialog for editing a the shipping provider's ship method
             */
            function editShippingMethodDialogOpen(gatewayMethod) {
                var dialogData = dialogDataFactory.createEditShippingGatewayMethodDialogData();
                dialogData.shippingGatewayMethod = gatewayMethod;
                dialogData.currencySymbol = $scope.currencySymbol;
                var editor = gatewayMethod.dialogEditorView.editorView;
                dialogService.open({
                    template: editor,
                    show: true,
                    callback: shippingMethodDialogConfirm,
                    dialogData: dialogData
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
            function shippingMethodDialogConfirm(dialogData) {
                var promiseShipMethodSave = shippingGatewayProviderResource.saveShipMethod(dialogData.shippingGatewayMethod.shipMethod);
                promiseShipMethodSave.then(function() {
                }, function (reason) {
                    notificationsService.error("Shipping Method Save Failed", reason.message);
                });
                reload();
            }

            /**
             * @ngdoc method
             * @name reload
             * @function
             *
             * @description
             * Handles the reload after receiving the modifying ship country information
             */
            function reload() {
                $scope.reload();
            }

            /**
             * @ngdoc method
             * @name delete
             * @function
             *
             * @description
             * Handles the delete of a ship country view/controller
             */
            function deleteCountry() {
                $scope.delete();
            }

            // initialize the directive
            init();

        }]);
