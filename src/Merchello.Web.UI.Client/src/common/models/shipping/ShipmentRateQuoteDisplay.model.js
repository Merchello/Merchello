/**
 * @ngdoc model
 * @name ShipmentRateQuoteDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's ShipmentRateQuoteDisplay object
 */
var ShipmentRateQuoteDisplay = function() {
    var self = this;
    self.shipMethod = {};
    self.shipment = {};
    self.rate = 0;
    self.extendedData = {};
};

angular.module('merchello.models').constant('ShipmentRateQuoteDisplay', ShipmentRateQuoteDisplay);