/**
 * @ngdoc model
 * @name BasketDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's BasketDisplay object
 */
var BasketDisplay = function() {
    var self = this;
    self.customer = {};
    self.items = [];
};

angular.module('merchello.models').constant('BasketDisplay', BasketDisplay);
