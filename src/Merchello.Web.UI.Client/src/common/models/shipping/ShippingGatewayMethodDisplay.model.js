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
    };

    ShippingGatewayMethodDisplay.prototype = (function() {

        function getName() {
            return this.shipMethod.name;
        }

        function getKey() {
            return this.shipMethod.key;
        }

        return {
            getName: getName,
            getKey: getKey
        };
    }());

    angular.module('merchello.models').constant('ShippingGatewayMethodDisplay', ShippingGatewayMethodDisplay);
