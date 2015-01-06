(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Order.Dialogs.FulfillShipmentController
     * @function
     * 
     * @description
     * The controller for editing a shipment on the shipment info page
     */
    controllers.EditShipmentController = function ($scope, merchelloOrderService, merchelloShipmentService, notificationsService, merchelloSettingsService) {

        /*-------------------------------------------------------------------
         * Initialization Methods
         * ------------------------------------------------------------------*/

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        $scope.init = function () {
            console.info($scope.dialogData);
            $scope.setVariables();
            $scope.buildShipmentStatusDropdown();
            $scope.loadSettings();
        };

        /**
         * @ngdoc method
         * @name buildShipmentStatusDropdown
         * @function
         * 
         * @description - Build a dropdown list of shipment statuses retrieved from the Shipment API.
         */
        $scope.buildShipmentStatusDropdown = function () {

            _.each($scope.dialogData.shipmentStatuses, function(option) {
                $scope.options.status.push(option);
            });

            // set the selected
            var i = 0;
            var found = false;
            while (i < $scope.options.status.length && !found) {
                if ($scope.options.status[i].key == $scope.dialogData.shipment.shipmentStatus.key) {
                    $scope.selected.status = $scope.options.status[i];
                    found = true;
                } else {
                    i++;
                }
            }
        };


        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         * 
         * @description - Load settings like currency symbols.
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
         * @name setVariables
         * @function
         * 
         * @description - Set variables to their default/starting state on the $scope.
         */
        $scope.setVariables = function () {
            $scope.options = {
                status: []
            }
            $scope.selected = {
                status: null
            }
        };

        /*-------------------------------------------------------------------
         * Event Handler Methods
         *-------------------------------------------------------------------*/

        /**
         * @ngdoc method
         * @name save
         * @function
         * 
         * @description - Prepare the data to be sent back to the order view controller for saving.
         */
        $scope.save = function () {
            $scope.dialogData.shipmentStatus = $scope.selected.status;
            $scope.dialogData.shipmentStatusKey = $scope.selected.status.key;
            $scope.dialogData.items = _.filter($scope.dialogData.items, function (item) {
                return item.selected == true;
            });
            $scope.submit($scope.dialogData);
        };

        /*-------------------------------------------------------------------
         * Helper Methods
         * ------------------------------------------------------------------*/


        /*-------------------------------------------------------------------*/

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Order.Dialogs.EditShipmentController", ['$scope', 'merchelloOrderService', 'merchelloShipmentService', 'notificationsService', 'merchelloSettingsService', merchello.Controllers.EditShipmentController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
