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
        $scope.currencySymbol = '';
        $scope.loaded = false;

        function init() {
            _.each($scope.dialogData.order.items, function(item) {
                item.selected = true;
            });
            //$scope.dialogData.shipMethods.alternatives = _.sortBy($scope.dialogData.shipMethods.alternatives, function(methods) { return method.name; } );
            if($scope.dialogData.shipMethods.selected === null || $scope.dialogData.shipMethods.selected === undefined) {
                $scope.dialogData.shipMethods.selected = $scope.dialogData.shipMethods.alternatives[0];
            }

            $scope.dialogData.shipmentRequest = new ShipmentRequestDisplay();
            $scope.dialogData.shipmentRequest.order = angular.extend($scope.dialogData.order, OrderDisplay);
            $scope.currencySymbol = $scope.dialogData.currencySymbol;
            $scope.loaded = true;
        }

        function save() {
            $scope.dialogData.shipmentRequest.shipMethodKey = $scope.dialogData.shipMethods.selected.key;
            $scope.dialogData.shipmentRequest.shipmentStatusKey = $scope.dialogData.shipmentStatus.key;
            $scope.dialogData.shipmentRequest.trackingNumber = $scope.dialogData.trackingNumber;
            $scope.dialogData.shipmentRequest.trackingUrl = $scope.dialogData.trackingUrl;
            $scope.dialogData.shipmentRequest.order.items = _.filter($scope.dialogData.order.items, function (item) {
                return item.selected === true;
            });
            $scope.submit($scope.dialogData);
        }

        init();

    }]);
