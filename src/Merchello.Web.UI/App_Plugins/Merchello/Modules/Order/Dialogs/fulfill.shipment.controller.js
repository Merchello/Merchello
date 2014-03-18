(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Order.Dialogs.FulfillShipmentController
     * @function
     * 
     * @description
     * The controller for the fulfillment of shipments on the Order View page
     */
    controllers.FulfillShipmentController = function ($scope, merchelloOrderService, notificationsService) {

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        $scope.getUnFulfilledItems = function (orderKey) {

            var promise = merchelloOrderService.getUnFulfilledItems(orderKey);

            promise.then(function (items) {

                $scope.dialogData.items = _.map(items, function (item) {
                    return new merchello.Models.OrderLineItem(item);
                });

            }, function (reason) {
                notificationsService.error("Line Items Load Failed", reason.message);
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

            $scope.getUnFulfilledItems($scope.dialogData.key);
            $scope.dialogData.trackingNumber = "";

        };

        $scope.init();
    };

    angular.module("umbraco").controller("Merchello.Dashboards.Order.Dialogs.FulfillShipmentController", merchello.Controllers.FulfillShipmentController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
