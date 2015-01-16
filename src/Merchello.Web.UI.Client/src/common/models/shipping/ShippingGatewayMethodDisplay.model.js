    /**
     * @ngdoc model
     * @name ShipMethodDisplay
     *
     * @description
     * Represents a JS version of Merchello's ShippingGatewayMethodDisplay object
     */
    var ShippingGatewayMethodDisplay = function() {
        var self = this;
        self.gatewayResource = {};
        self.shipMethod = {};
        self.shipCountry = {};
        self.dialogEditorView = {};
    }

    angular.module('merchello.models').constant('ShippingGatewayMethodDisplay', ShippingGatewayMethodDisplay);
