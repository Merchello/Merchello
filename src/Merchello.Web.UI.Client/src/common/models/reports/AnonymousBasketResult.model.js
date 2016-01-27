/**
 * @ngdoc model
 * @name AbandonedBasketResult
 * @function
 *
 * @description
 * Represents a JS version of Merchello's AbandonedBasketResult object
 */
var AbandonedBasketResult = function() {
    var self = this;
    self.configuredDays = 0;
    self.startDate = '';
    self.endDate = '';
    self.anonymousBasketCount = 0;
    self.anonymousCheckoutCount = 0;
    self.anonymousCheckoutPercent = 0;
    self.customerBasketCount = 0;
    self.customerCheckoutCount = 0;
    self.customerCheckoutPercent = 0;
};

angular.module('merchello.models').constant('AbandonedBasketResult', AbandonedBasketResult);
