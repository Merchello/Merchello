(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Order.Dialogs.FulfillShipmentController
     * @function
     * 
     * @description
     * The controller for the fulfillment of shipments on the Order View page
     */
	controllers.FulfillShipmentController = function ($scope, merchelloOrderService, merchelloShipmentService, notificationsService, merchelloSettingsService) {

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
	        $scope.getUnFulfilledItems($scope.dialogData.key);
	        $scope.getShipMethodForOrder($scope.dialogData);
	    };

	    /**
         * @ngdoc method
         * @name buildShipmentStatusDropdown
         * @function
         * 
         * @description - Build a dropdown list of shipment statuses retrieved from the Shipment API.
         */
	    $scope.buildShipmentStatusDropdown = function () {
	        var promise = merchelloShipmentService.getAllShipmentStatuses();
	        promise.then(function (response) {
	            console.info(response);
	            _.each(response, function(option) {
	                $scope.options.status.push(option);
	            });
	            $scope.selected.status = $scope.options.status[0];
	        }, function (reason) {
	            notificationsService.error('Failed to load shipment statuses', reason.message);
	        });
	    };

	    /**
         * @ngdoc method
         * @name getShipMethodForOrder
         * @function
         * 
         * @description - Load the ship methods that will be available for this order.
         */
	    $scope.getShipMethodForOrder = function (order) {
	        var promise = merchelloShipmentService.getShipMethod(order);
	        promise.then(function (method) {
	            $scope.shipMethod = new merchello.Models.ShippingMethod(method);
	        }, function (reason) {
	            notificationsService.error("Shipment Methods Load Failed", reason.message);
	        });
	    };

	    /**
         * @ngdoc method
         * @name getUnFulfilledItems
         * @function
         * 
         * @description - Get all the currently unfulfilled items in the order.
         */
	    $scope.getUnFulfilledItems = function (orderKey) {
	        var promise = merchelloOrderService.getUnFulfilledItems(orderKey);
	        promise.then(function (items) {
	            $scope.dialogData.items = _.map(items, function (item) {
	                return new merchello.Models.OrderLineItem(item);
	            });
	            _.each($scope.dialogData.items, function (item) {
	                if (!item.backOrder) {
	                    item.selected = true;
	                } else {
	                    item.selected = false;
	                }
	            });
	        }, function (reason) {
	            notificationsService.error("Line Items Load Failed", reason.message);
	        });
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
	        $scope.dialogData.trackingNumber = "";
	        $scope.shipMethod = {};
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

	angular.module("umbraco").controller("Merchello.Dashboards.Order.Dialogs.FulfillShipmentController", ['$scope', 'merchelloOrderService', 'merchelloShipmentService', 'notificationsService', 'merchelloSettingsService', merchello.Controllers.FulfillShipmentController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
