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

        $scope.checkboxDisabled = checkboxDisabled;

        function init() {
            _.each($scope.dialogData.shipment.items, function(item) {
                item.selected = true;
            });
            $scope.loaded = true;
        }

        function checkboxDisabled() {
            return $scope.dialogData.shipment.shipmentStatus.name === 'Shipped' || $scope.dialogData.shipment.shipmentStatus.name === 'Delivered'
        }

        function save() {
            $scope.dialogData.shipment.items = _.filter($scope.dialogData.shipment.items, function (item) {
                return item.selected === true;
            });
            $scope.submit($scope.dialogData);
        }

        init();

    }]);

