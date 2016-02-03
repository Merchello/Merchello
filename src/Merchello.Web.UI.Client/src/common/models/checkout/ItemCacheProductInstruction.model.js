/**
 * @ngdoc model
 * @name ItemCacheProductInstruction
 * @function
 *
 * @description
 * Represents a JS version of Merchello's ItemCacheProductInstruction object
 */
var ItemCacheProductInstruction = function() {
    var self = this;
    var self = this;
    self.customer = {};
    self.productVariant = {};
    self.itemCacheType = '';
};

angular.module('merchello.models').constant('ItemCacheProductInstruction', ItemCacheProductInstruction);
