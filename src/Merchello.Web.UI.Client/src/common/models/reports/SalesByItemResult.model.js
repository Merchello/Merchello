/**
 * @ngdoc model
 * @name SalesByItemResult
 * @function
 *
 * @description
 * Represents a JS version of Merchello's SalesByItemResult object
 */
var SalesByItemResult = function() {
    var self = this;

    self.productVariant = {};
    self.quantitySold = 0;
    self.invoiceCount = 0;
    self.totals = [];

};

angular.module('merchello.models').constant('SalesByItemResult', SalesByItemResult);
