/**
 * @ngdoc model
 * @name ItemCacheLineItemInstruction
 * @function
 *
 * @description
 * Represents a JS version of Merchello's ItemCacheLineItemInstruction object
 */
var ItemCacheLineItemInstruction = function() {
    var self = this;
    self.customer = {};
    self.lineItem = {};
    self.itemCacheType = '';
};


angular.module('merchello.models').constant('ItemCacheLineItemInstruction', ItemCacheLineItemInstruction);
