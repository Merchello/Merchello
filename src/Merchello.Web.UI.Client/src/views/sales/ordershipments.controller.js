/**
 * @ngdoc controller
 * @name Merchello.Dashboards.OrderShipmentsController
 * @function
 *
 * @description
 * The controller for the order shipments view
 */
angular.module('merchello').controller('Merchello.Backoffice.OrderShipmentsController',
    ['$scope', '$routeParams', '$log', 'notificationsService', 'dialogService', 'dialogDataFactory', 'merchelloTabsFactory',
        'invoiceResource', 'settingsResource', 'shipmentResource',
        'invoiceDisplayBuilder', 'shipmentDisplayBuilder',
        function($scope, $routeParams, $log, notificationsService, dialogService, dialogDataFactory, merchelloTabsFactory, invoiceResource,
                 settingsResource, shipmentResource, invoiceDisplayBuilder, shipmentDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
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
                $scope.tabs = merchelloTabsFactory.createSalesTabs(key);
                $scope.tabs.setActive('shipments');
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
                    // append the customer tab
                    $scope.tabs.appendCustomerTab($scope.invoice.customerKey);
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
                        notificationsService.success('Successfully updated sales shipping address.');
                    },
                    function(reason) {
                        notificationsService.error('Failed to update shipping addresses on invoice', reason.message);
                    });
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
                if(dialogData.shipment.items.length > 0) {
                    saveShipment(dialogData.shipment);
                } else {
                    notificationsService.warning('Cannot remove all items from the shipment.  Instead, consider deleting the shipment.');
                    loadInvoice($scope.invoice.key);
                };
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
                    notificationsService.success('Shipment deleted');
                }, function(reason) {
                    notificationsService.error('Failed to delete the invoice.', reason.message);
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
                    notificationsService.success('Shipment saved');
                });
            }


            // initializes the controller
            init();
    }]);
