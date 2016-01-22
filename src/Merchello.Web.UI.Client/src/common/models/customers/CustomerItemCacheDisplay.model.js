/**
 * @ngdoc model
 * @name CustomerItemCacheDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's CustomerItemCacheDisplay object
 */
var CustomerItemCacheDisplay = function() {
    var self = this;
    self.customer = {};
    self.items = [];
};

angular.module('merchello.models').constant('CustomerItemCacheDisplay', CustomerItemCacheDisplay);
