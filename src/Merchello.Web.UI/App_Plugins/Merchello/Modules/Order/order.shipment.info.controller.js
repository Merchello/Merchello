(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Order.ShipmentInfoController
     * @function
     * 
     * @description
     * The controller for the order payment info page
     */
    controllers.ShipmentInfoController = function ($scope, $routeParams, dialogService, localizationService, notificationsService, merchelloInvoiceService, merchelloPaymentService, merchelloSettingsService, merchelloShipmentService) {

        //--------------------------------------------------------------------------------------
        // Initialization Methods
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
            $scope.loadTypeFields(function () { $scope.loadInvoice($routeParams.id); });
            $scope.loadSettings();
            $scope.loadShipmentStatuses();
        };

        /**
         * @ngdoc method
         * @name setVariables
         * @function
         * 
         * @description
         * Sets the $scope variables.
         */
        $scope.setVariables = function () {
            $scope.loaded = false;
            $scope.invoice = {};
            $scope.shipments = [];
            $scope.shipmentStatuses = [];
            $scope.typeFields = [];
        };

        //--------------------------------------------------------------------------------------
        // Event Handler Methods
        //--------------------------------------------------------------------------------------

        /**
        * @ngdoc method
        * @name editShipment
        * @function
        * 
        * @description
        * Edits a shipment record.
        */
        $scope.editShipment = function (shipmentKey) {

            var dialogData = {};
            dialogData.shipmentStatuses = $scope.shipmentStatuses;

            var i = 0;
            var found = false;
            while (i < $scope.shipments.length && !found) {
                if ($scope.shipments[i].key == shipmentKey) {
                    dialogData.shipment = $scope.shipments[i];
                    found = true;
                }
            }

            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Order/Dialogs/shipment.editshipment.html',
                show: true,
                callback: $scope.capturePaymentDialogConfirm,
                dialogData: dialogData
        });
        };

        //--------------------------------------------------------------------------------------
        // Helper Methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name formatDate
         * @function
         * 
         * @description
         * Format the provided date to something more readable.
         */
        $scope.formatDate = function (date) {
            // TODO: Provide localized date formatting.
            return date.split('T')[0];
        }

        /**
         * @ngdoc method
         * @name hasOrder
         * @function
         * 
         * @description
         * Returns false if the invoice has no orders.
         */
        $scope.hasOrder = function () {
            var result = false;
            if ($scope.invoice.orders !== undefined) {
                if ($scope.invoice.orders.length > 0) {
                    result = true;
                }
            }
            return result;
        };

        /**
         * @ngdoc method
         * @name loadInvoice
         * @function
         * 
         * @description
         * Load an invoice with the associated id.
         */
        $scope.loadInvoice = function (id) {
            var promise = merchelloInvoiceService.getByKey(id);
            promise.then(function (invoice) {
                $scope.invoice = new merchello.Models.Invoice(invoice);
                $scope.loaded = true;
                $scope.loadShipments($scope.invoice);
            }, function (reason) {
                notificationsService.error("Invoice Load Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         * 
         * @description
         * Load the Merchello settings.
         */
        $scope.loadSettings = function () {
            var currencySymbolPromise = merchelloSettingsService.getCurrencySymbol();
            currencySymbolPromise.then(function (currencySymbol) {
                $scope.currencySymbol = currencySymbol;

            }, function (reason) {
                alert('Failed: ' + reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name loadShipments
         * @function
         * 
         * @description
         * Load the shipments associated with the provided invoice.
         */
        $scope.loadShipments = function (invoice) {
            if ($scope.hasOrder()) {
                var promise = merchelloShipmentService.getShipmentsByInvoice(invoice);
                promise.then(function (shipments) {
                    $scope.shipments = _.map(shipments, function (shipment) {
                        return new merchello.Models.Shipment(shipment);
                    });
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Shipments Load Failed", reason.message);
                });
            }
        };

        /**
         * @ngdoc method
         * @name loadTypeFields
         * @function
         * 
         * @description
         * Load in the type fields.
         */
        $scope.loadTypeFields = function (nextMethodCall) {
            var promise = merchelloSettingsService.getTypeFields();
            promise.then(function (typeFields) {
                $scope.typeFields = _.map(typeFields, function (type) {
                    return new merchello.Models.TypeField(type);
                });
                if (nextMethodCall != undefined) {
                    nextMethodCall();
                }
            }, function (reason) {
                notificationsService.error("TypeFields Load Failed", reason.message);
            });
        };

        $scope.loadShipmentStatuses = function() {
            var promise = merchelloShipmentService.getAllShipmentStatuses();
            promise.then(function(response) {
                console.info(response);
                _.each(response, function(option) {
                    $scope.shipmentStatuses.push(option);
                });
            });
        };

        //--------------------------------------------------------------------------------------

        $scope.init();

    };


    angular.module("umbraco").controller("Merchello.Editors.Order.ShipmentInfoController", ['$scope', '$routeParams', 'dialogService', 'localizationService', 'notificationsService', 'merchelloInvoiceService', 'merchelloPaymentService', 'merchelloSettingsService', 'merchelloShipmentService', merchello.Controllers.ShipmentInfoController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));

