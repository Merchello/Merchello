
var TaxationGatewayProviderDisplay = function () {
    var self = this;
    GatewayProviderDisplay.apply(self, arguments);
    self.taxationByProductProvider = false;
};

TaxationGatewayProviderDisplay.prototype = GatewayProviderDisplay.prototype;
TaxationGatewayProviderDisplay.prototype.constructor = TaxationGatewayProviderDisplay;

angular.module('merchello.models').constant('TaxationGatewayProviderDisplay', TaxationGatewayProviderDisplay);