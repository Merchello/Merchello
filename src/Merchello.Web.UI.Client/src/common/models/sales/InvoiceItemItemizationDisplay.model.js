/**
 * @ngdoc model
 * @name InvoiceItemItemizationDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's InvoiceItemItemizationDisplay object
 */
var InvoiceItemItemizationDisplay = function() {
    var self = this;
    self.adjustments = [];
    self.custom = [];
    self.discounts = [];
    self.products = [];
    self.shipping = [];
    self.tax = [];
    self.reconciles = false;
    self.productTotal = 0.00;
    self.shippingTotal = 0.00;
    self.taxTotal = 0.00;
    self.adjustmentTotal = 0.00;
    self.discountTotal = 0.00;
    self.customTotal = 0.00;
    self.invoiceTotal = 0.00;
    self.itemizationTotal = 0.00;
};

angular.module('merchello.models').constant('InvoiceItemItemizationDisplay', InvoiceItemItemizationDisplay);