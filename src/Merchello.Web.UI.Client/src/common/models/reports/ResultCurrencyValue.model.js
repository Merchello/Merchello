/**
 * @ngdoc model
 * @name ResultCurrencyValue
 * @function
 *
 * @description
 * Represents a JS version of Merchello's ResultCurrencyValue object
 */
var ResultCurrencyValue = function() {
    var self = this;

    self.currency = {};
    self.value = 0;
};

angular.module('merchello.models').constant('ResultCurrencyValue', ResultCurrencyValue);
