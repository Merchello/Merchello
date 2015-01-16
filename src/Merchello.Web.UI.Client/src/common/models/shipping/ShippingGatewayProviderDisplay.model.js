    /**
     * @ngdoc model
     * @name ShippingGatewayProviderDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipmentDisplay object
     */
    var ShippingGatewayProviderDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.extendedData = {};
        self.shipMethods = [];
        self.availableResources = [];
    };

    ShippingGatewayProviderDisplay.prototype = (function() {

        function addMethod(shippingMethod) {
            this.shipMethods.push(shippingMethod);
        }

        function removeMethod(shippingMethod) {
            this.shipMethods = _.reject(this.shipMethods, function(m) {
              return m.key === shipmethod.key;
            });
        }

        return {
            addMethod: addMethod,
            removeMethod: removeMethod
        };

    }());

    angular.module('merchello.models').constant('ShippingGatewayProviderDisplay', ShippingGatewayProviderDisplay);