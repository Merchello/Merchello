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

        $scope.shipMethod = {};

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        $scope.getUnFulfilledItems = function (orderKey) {

            var promise = merchelloOrderService.getUnFulfilledItems(orderKey);

            promise.then(function (items) {

                $scope.dialogData.items = _.map(items, function (item) {
                    return new merchello.Models.OrderLineItem(item);
                });

                _.each($scope.dialogData.items, function(item) {
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

        $scope.getShipMethodForOrder = function (order) {

            var promise = merchelloShipmentService.getShipMethod(order);

            promise.then(function (method) {

                $scope.shipMethod = new merchello.Models.ShippingMethod(method);

            }, function (reason) {
                notificationsService.error("Shipment Methods Load Failed", reason.message);
            });
        };

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
         * @name init
         * @function
         * 
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        $scope.init = function () {

        	$scope.loadSettings();
            $scope.getUnFulfilledItems($scope.dialogData.key);
            $scope.getShipMethodForOrder($scope.dialogData);
            $scope.dialogData.trackingNumber = "";

        };

        $scope.init();
    };

	angular.module("umbraco").controller("Merchello.Dashboards.Order.Dialogs.FulfillShipmentController", ['$scope', 'merchelloOrderService', 'merchelloShipmentService', 'notificationsService', 'merchelloSettingsService', merchello.Controllers.FulfillShipmentController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
