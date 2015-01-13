angular.module('merchello').controller('Merchello.Backoffice.OrderShipmentsController',
    ['$scope', '$routeParams', 'notificationsService', 'dialogService', 'dialogDataFactory',
        'invoiceResource', 'settingsResource', 'shipmentResource',
        'invoiceDisplayBuilder', 'shipmentDisplayBuilder',
        function($scope, $routeParams, notificationsService, dialogService, dialogDataFactory, invoiceResource,
                 settingsResource, shipmentResource, invoiceDisplayBuilder, shipmentDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.invoice = {};
            $scope.settings = {};
            $scope.shipments = [];

            // methods
            $scope.isEditableAddress = isEditableAddress;

            // dialogs
            $scope.openShipmentDialog = openShipmentDialog;
            $scope.processUpdateShipment = processUpdateShipment;
            $scope.openAddressDialog = openAddressDialog;
            $scope.processUpdateOriginAddress = processUpdateOriginAddress;
            $scope.processUpdateDestinationAddress = processUpdateDestinationAddress;


            function init() {
                var key = $routeParams.id;
                loadInvoice(key);
                $scope.loaded = true;
            }

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

            function getActionButtons(shipment) {
                var actions = [
                    { }
                ];
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

            /*
                Dialogs
            */

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

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/edit.shipment.html',
                        show: true,
                        callback: $scope.processUpdateShipment,
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
                dialogData.address = addressType === 'destination' ? shipment.getDestinationAddress() : shipment.getOriginAddress();

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
                        callback: addressType === 'destination' ? $scope.processUpdateDestinationAddress : $scope.processUpdateOriginAddress,
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
