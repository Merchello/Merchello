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
    ['$scope', 'localizationService', 'ShipmentRequestDisplay', 'OrderDisplay', function($scope, localizationService, ShipmentRequestDisplay, OrderDisplay) {

        $scope.save = save;
        $scope.currencySymbol = '';
        $scope.loaded = false;
        $scope.updateViewSelectedForShipment = updateViewSelectedForShipment;
        $scope.shipmentSelection = {
            show: false,
            shipments: [],
            selected: {}
        };

        var customLabel = 'Custom';

        function init() {

            //$scope.dialogData.shipMethods.alternatives = _.sortBy($scope.dialogData.shipMethods.alternatives, function(methods) { return method.name; } );
            if($scope.dialogData.shipMethods.selected === null || $scope.dialogData.shipMethods.selected === undefined) {
                $scope.dialogData.shipMethods.selected = $scope.dialogData.shipMethods.alternatives[0];
            }

            localizationService.localize('merchelloGeneral_custom').then(function(data) {
                customLabel = data;
                buildShipmentSelection();
                $scope.dialogData.shipmentRequest = new ShipmentRequestDisplay();
                $scope.dialogData.shipmentRequest.order = angular.extend($scope.dialogData.order, OrderDisplay);
                $scope.currencySymbol = $scope.dialogData.currencySymbol;
                updateViewSelectedForShipment();
                $scope.loaded = true;
            });
        }

        function buildShipmentSelection() {
            if ($scope.dialogData.shipmentLineItems.length > 1) {
                // Figure out which shipments have not been shipped (completely) and make them
                // available to the drop down.
                angular.forEach($scope.dialogData.shipmentLineItems, function(s) {
                    var items = getShipmentItemsNotPackaged(s);
                    //console.info($scope.dialogData.order);
                    //console.info(s);
                    if (items.length > 0) {
                        $scope.shipmentSelection.shipments.push({ key: s.key, name: s.name, shipMethodKey: s.extendedData.getValue('merchShipMethodKey'), items: items });
                    }
                });
            }

            if ($scope.shipmentSelection.shipments.length > 0) {
                $scope.shipmentSelection.show = true;
            }

            $scope.shipmentSelection.shipments.push({ key: '', name: customLabel, shipMethodKey: $scope.dialogData.shipMethods.selected.key, items: $scope.dialogData.order.items });
            $scope.shipmentSelection.selected = $scope.shipmentSelection.shipments[0];
        }


        function updateViewSelectedForShipment() {
            // set the check boxes
            _.each($scope.dialogData.order.items, function(item) {
                var fnd = _.find($scope.shipmentSelection.selected.items, function(i) {
                    if (item.key === i.key) { return i; }
                });
                item.selected = fnd !== undefined;
            });

            // set the shipmethod
            var method = _.find($scope.dialogData.shipMethods.alternatives, function(sm) {
                if ($scope.shipmentSelection.selected.shipMethodKey === sm.key) {
                    return sm;
                }
            });
            if (method !== undefined) {
                $scope.dialogData.shipMethods.selected = method;
            }
        }


        // get the unpackaged items
        function getShipmentItemsNotPackaged(shipment) {

            var skus = getLineItemSkus(shipment.extendedData);
            if (skus.length === 0) {
                return [];
            }
            // get the order line items by matching the skus
            var remainingItems = _.filter($scope.dialogData.order.items, function (oi) {
                var fnd = _.find(skus, function(s) { return oi.sku === s; });
                if (fnd !== undefined) {
                    return oi;
                }
            });

            return remainingItems;
        }

        // parses the merchLineItemCollection xml stored in extended data to find individual skus
        function getLineItemSkus(extendedData) {
            var txt = extendedData.getValue('merchLineItemCollection');
            // get the SKU's of each item in the shipment
            var regex = /<merchSku>(.+?)<\/merchSku>/g;
            var matches = [];
            var match = regex.exec(txt);
            while (match != null) {
                matches.push(match[1]);
                match = regex.exec(txt);
            }

            return matches;
        }


        function save() {
            $scope.dialogData.shipmentRequest.shipMethodKey = $scope.dialogData.shipMethods.selected.key;
            $scope.dialogData.shipmentRequest.shipmentStatusKey = $scope.dialogData.shipmentStatus.key;
            $scope.dialogData.shipmentRequest.carrier = $scope.dialogData.carrier;
            $scope.dialogData.shipmentRequest.trackingNumber = $scope.dialogData.trackingNumber;
            $scope.dialogData.shipmentRequest.trackingUrl = $scope.dialogData.trackingUrl;

            $scope.dialogData.shipmentRequest.order.items = _.filter($scope.dialogData.order.items, function (item) {
                return item.selected === true;
            });
            $scope.submit($scope.dialogData);
        }

        init();

    }]);
