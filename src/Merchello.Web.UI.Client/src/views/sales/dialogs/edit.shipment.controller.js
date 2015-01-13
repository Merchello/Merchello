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

