    /**
     * @ngdoc model
     * @name ShipmentRequestDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipmentRequestDisplay object
     */
    var ShipmentRequestDisplay = function() {
        var self = this;
        self.shipmentStatusKey = '';
        self.order = {};
        self.shipMethodKey = '';
        self.trackingNumber = '';
    };

    angular.module('merchello.models').constant('ShipmentRequestDisplay', ShipmentRequestDisplay);
