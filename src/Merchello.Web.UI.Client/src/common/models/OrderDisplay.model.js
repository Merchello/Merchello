    /**
     * @ngdoc model
     * @name OrderDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's OrderDisplay object
     */
    var OrderDisplay = function() {
        var self = this;
        self.key = '';
        self.versionKey = '';
        self.invoiceKey = '';
        self.orderNumberPrefix = '';
        self.orderNumber = '';
        self.orderDate = '';
        self.orderStatusKey = ''; // to do this is not needed since we have the OrderStatusDisplay
        self.orderStatus = {};
        self.exported = false;
        self.items = [];
    };

    OrderDisplay.prototype = (function() {
        // private
        // TODO this could address the issue in current merchello.view.controller
        // $scope.processFulfillShipmentDialog
        function createShipment(shipmentStatus, origin, destination, lineItems) {
            if (shipmentStatus === undefined) {
                return;
            }
            var shipment = new ShipmentDisplay();
            shipment.setOriginAddress(origin);
            shipment.setDestinationAddress(destination);
            shipment.shipmentStatus = shipmentStatus;
            if (lineItems === undefined) {
                shipment.items = this.items;
            }
            else {
                shipment.items = lineItems;
            }
            return shipment;
        }

        // public
        return {
            createShipment: createShipment
        };

    }());

    angular.module('merchello.models').constant('OrderDisplay', OrderDisplay);